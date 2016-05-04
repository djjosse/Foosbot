// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.Common;
using Foosbot.Common.Exceptions;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Foosbot.Common.Contracts;

namespace Foosbot.ImageProcessingUnit.Tools.Core
{
    /// <summary>
    /// Calibration Unit Tool
    /// </summary>
    public class CalibrationUnit : ICalibration, IInitializable
    {
        #region IInitializable and Set parameters

        /// <summary>
        /// Foosbot World AXE X length (Length between calibration marks left and right)
        /// </summary>
        private int AXE_X_LENGTH;

        /// <summary>
        /// Foosbot World AXE Y length (Length between calibration marks top and bottom)
        /// </summary>
        private int AXE_Y_LENGTH;

        /// <summary>
        /// Ball Radius in MM
        /// </summary>
        private double BALL_RADIUS_MM;

        /// <summary>
        /// Is Initialized property
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initialization method
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                AXE_X_LENGTH = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
                AXE_Y_LENGTH = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
                BALL_RADIUS_MM = Configuration.Attributes.GetValue<double>(Configuration.Names.BALL_DIAMETR) / 2;
                IsInitialized = true;
            }
        }

        #endregion IInitializable and Set parameters

        #region Calibration Constants

        /// <summary>
        /// Approx. Inner Radius of Calibration Circle
        /// </summary>
        public const int INNER_RADIUS = 30;

        /// <summary>
        /// Approx. Outer Radius of Calibration Circle
        /// </summary>
        public const int OUTER_RADIUS = 50;

        /// <summary>
        /// Possible error for inner and outer radius
        /// </summary>
        public const double RADIUS_THRESHOLD = 1;

        /// <summary>
        /// Possible Error for distance between inner and outer mark radius
        /// </summary>
        public const double DISTANCE_ERROR = 0.1;

        /// <summary>
        /// Number of frames to skip in case of unsuccessful calibration before next re-try
        /// </summary>
        public const int FRAMES_TO_SKIP = 30;

        #endregion Callibration Constants

        #region Private members

        /// <summary>
        /// Number of frames skipped
        /// </summary>
        private int _skippedFrames;

        #endregion Private members

        #region Properties of ICalibration

        /// <summary>
        /// Common Imaging Data for common set/updated/use
        /// </summary>
        public IImageData ImagingData { get; private set; }

        /// <summary>
        /// Circle Detection Tool Instance
        /// </summary>
        public ICircleDetector CircleDetector { get; private set; }

        /// <summary>
        /// Calibration Helper
        /// </summary>
        public ICalibrationHelper CalibrationUtils { get; private set; }

        /// <summary>
        /// Current Calibration State
        /// </summary>
        public eCalibrationState CurrentState { get; private set; }

        /// <summary>
        /// Dictionary of Computer Vision Monitors to display image processing data on the screen
        /// Where key is an enum of Computer Vision Monitor type and value is its Instance
        /// </summary>
        public Dictionary<eComputerVisionMonitor, IComputerVisionMonitor> ComputerVisionMonitors { get; private set; }

        #endregion Properties of ICalibration

        /// <summary>
        /// Calibration Unit Constructor
        /// </summary>
        /// <param name="commonData">Common Image Processing Data to be used/set/updated by calibration unit</param>
        /// <param name="circleDetector">Circle detection tool [default parameter is null - will be created in constructor]</param>
        /// <param name="calibrationHelper">Calibration Helper Instance [default parameter is null - will be created in constructor]</param>
        public CalibrationUnit(IImageData commonData, ICircleDetector circleDetector = null, ICalibrationHelper calibrationHelper = null)
        {
            ImagingData = commonData;
            CircleDetector = (circleDetector != null) ? circleDetector : new CircleDetector();
            ComputerVisionMonitors = new Dictionary<eComputerVisionMonitor, IComputerVisionMonitor>();
            CalibrationUtils = (calibrationHelper != null) ? calibrationHelper : new CalibrationHelper();
        }

        /// <summary>
        /// Main calibration method
        /// This method should to be called each time we haven't finished calibration.
        /// Different calibration Phase will be called depending on current calibration state.
        /// At the end this method sets CurrentState of calibration as Finished
        /// </summary>
        /// <param name="frame">Frame to perform calibration on</param>
        public void Calibrate(IFrame frame)
        {
            Initialize();
            switch(CurrentState)
            {
                case eCalibrationState.NotStarted:
                    Begin(frame);
                    break;
                case eCalibrationState.Performing:
                    Finalize(frame);
                    break;
                case eCalibrationState.Finished:
                    return;
            }
        }

        #region Private Methods

        /// <summary>
        /// First Calibration Phase
        /// Performing:
        /// 1. Find and Sort Calibration marks
        /// 2. Calculate and set transformation matrix based on calibration marks
        /// 3. Calculate Ball radius and possible error
        /// </summary>
        /// <param name="frame"></param>
        private void Begin(IFrame frame)
        {
            //ignore first frames
            if (_skippedFrames < FRAMES_TO_SKIP)
            {
                _skippedFrames++;
            }
            else
            {
                try
                {
                    _skippedFrames = 0;

                    Log.Image.Info("***** Starting calibration Phase I... *****");
                    Image<Gray, byte> image = frame.Image.Clone();

                    //Remove Noise from picture
                    image = CalibrationUtils.PrepareFrame(image);
                    ComputerVisionMonitors[eComputerVisionMonitor.MonitorA].ShowFrame(image);

                    //Find Calibration Marks
                    List<CircleF> circles = FindCalibrationMarks(image);
                    CalibrationUtils.VerifyMarksFound(circles);

                    //Sort Calibration Marks and set value to property
                    SortCalibrationMarks(circles);

                    ShowAllCalibrationMarks();
                    StringBuilder str = new StringBuilder("4 calibration Marks found and sorted: \n\t\t\t\t");
                    foreach (var mark in ImagingData.CalibrationMarks)
                        str.Append(String.Format("{0}:[{1}x{2}] ", mark.Key, mark.Value.Center.X, mark.Value.Center.Y));
                    Log.Image.Info(str.ToString());

                    CalibrationUtils.SetTransformationMatrix(AXE_X_LENGTH, AXE_Y_LENGTH, ImagingData.CalibrationMarks);
                    Log.Image.Info("Homography matrix calculated.");

                    //Calculate Ball Radius and Error
                    int ballRadius; 
                    double ballError;
                    CalibrationUtils.CalculateBallRadiusAndError((float)BALL_RADIUS_MM, out ballRadius, out ballError);

                    ImagingData.BallRadius = ballRadius;
                    ImagingData.BallRadiusError = ballError;

                    Log.Image.Info(String.Format("Expected ball radius is {0} +/- {1}",
                        ImagingData.BallRadius, ImagingData.BallRadiusError));

                    CurrentState = eCalibrationState.Performing;
                    Log.Image.Info("***** Finished calibration Phase I *****");
                }
                catch (CalibrationException ex)
                {
                    Log.Image.Warning(String.Format("Calibration failed in first phase. Will retry after [{0}] frames. Reason: {1}",
                           FRAMES_TO_SKIP, ex.Message));
                }
            }
        }

        /// <summary>
        /// Second Calibration Phase
        /// Performing:
        /// 1. Find Calibration Marks to verify image is fine
        /// 2. Update Table Image Processing Coverage
        /// 3. Recalculate Homography matrices
        /// </summary>
        /// <param name="frame"></param>
        private void Finalize(IFrame frame)
        {
            //ignore first frames
            if (_skippedFrames < FRAMES_TO_SKIP)
            {
                _skippedFrames++;
            }
            else
            {
                try
                {
                    _skippedFrames = 0;

                    Log.Image.Info("***** Starting calibration Phase II... *****");
                    Image<Gray, byte> image = frame.Image.Clone();

                    //Remove Noise from picture
                    image = CalibrationUtils.PrepareFrame(image);
                    ComputerVisionMonitors[eComputerVisionMonitor.MonitorA].ShowFrame(image);

                    //Find Calibration Marks
                    List<CircleF> circles = FindCalibrationMarks(image);
                    CalibrationUtils.VerifyMarksFound(circles);

                    //Update coverage
                    ImagingData.CalibrationMarks = CalibrationUtils.UpdateCoverage(ImagingData.CalibrationMarks);
                    Log.Image.Info("Coverage and marks updated.");

                    //Show table border marks
                    Marks.DrawTableBorders(ImagingData.CalibrationMarks);

                    //Recalculate Homography Matrix
                    CalibrationUtils.SetTransformationMatrix(AXE_X_LENGTH, AXE_Y_LENGTH, ImagingData.CalibrationMarks);
                    Log.Image.Info("Homography matrix re-calculated.");

                    CurrentState = eCalibrationState.Finished;
                    Log.Image.Info("***** Finished calibration Phase II *****");
                }
                catch (CalibrationException ex)
                {
                    Log.Image.Warning(String.Format("Calibration failed in second phase. Will retry after [{0}] frames. Reason: {1}",
                           FRAMES_TO_SKIP, ex.Message));
                }
            }
        }

        /// <summary>
        /// Find Calibration Marks on image
        /// </summary>
        /// <param name="image">Source image to find circles on</param>
        /// <returns>List of calibration marks as circles</returns>
        private List<CircleF> FindCalibrationMarks(Image<Gray, byte> image)
        {
            List<CircleF> circles = new List<CircleF>();

            //Find Calibration Big Circles
            CircleF[] possibleInnerCircles = CircleDetector.DetectCircles(image, INNER_RADIUS, RADIUS_THRESHOLD, (double)OUTER_RADIUS * 5);

            //Find Calibration Small Circles
            CircleF[] possibleOuterCircles = CircleDetector.DetectCircles(image, OUTER_RADIUS, RADIUS_THRESHOLD, (double)OUTER_RADIUS * 5);

            //Find corresponding circles
            foreach (CircleF innerCircle in possibleInnerCircles)
            {
                foreach (CircleF outerCircle in possibleOuterCircles)
                {
                    if (Utils.Distance(innerCircle.Center, outerCircle.Center) < DISTANCE_ERROR * OUTER_RADIUS)
                    {
                        circles.Add(outerCircle);
                    }
                }
            }

            return circles;
        }

        /// <summary>
        /// Sort marks and fill the property
        /// CallibrationMarks
        /// </summary>
        /// <param name="unsortedMarks">Unsorted calibration marks list</param>
        private void SortCalibrationMarks(List<CircleF> unsortedMarks)
        {
            if (unsortedMarks.Count != 4)
                throw new NotSupportedException("To find diagonal mark pairs exactly 4 marks must be detected!");

            Dictionary<CircleF, CircleF> diagonalPairs = CalibrationUtils.FindDiagonalMarkPairs(unsortedMarks);

            //In each pair find left point, right, bottom and top and set corresponding marks
            foreach (CircleF key in diagonalPairs.Keys)
            {
                CircleF buttom = (key.Center.Y > diagonalPairs[key].Center.Y) ? key : diagonalPairs[key];
                CircleF top = (key.Center.Y < diagonalPairs[key].Center.Y) ? key : diagonalPairs[key];
                CircleF left = (key.Center.X < diagonalPairs[key].Center.X) ? key : diagonalPairs[key];
                CircleF right = (key.Center.X > diagonalPairs[key].Center.X) ? key : diagonalPairs[key];

                if (buttom.Equals(left))
                    ImagingData.CalibrationMarks.Add(eCallibrationMark.BL, left);
                if (top.Equals(left))
                    ImagingData.CalibrationMarks.Add(eCallibrationMark.TL, left);
                if (top.Equals(right))
                    ImagingData.CalibrationMarks.Add(eCallibrationMark.TR, right);
                if (buttom.Equals(right))
                    ImagingData.CalibrationMarks.Add(eCallibrationMark.BR, right);
            }
        }

        /// <summary>
        /// Show all calibration marks on screen
        /// </summary>
        private void ShowAllCalibrationMarks()
        {
            foreach (KeyValuePair<eCallibrationMark, CircleF> mark in ImagingData.CalibrationMarks)
            {
                CalibrationUtils.ShowCalibrationMark(mark.Value, mark.Key);
            }
        }

        #endregion Private Methods
    }
}
