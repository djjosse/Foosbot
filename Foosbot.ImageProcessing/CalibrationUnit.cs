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
using Foosbot.Common.Contracts;
using Foosbot.Common.Exceptions;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Calibration Unit Class
    /// Responsible for camera and Image Calibration in Image Processing Unit
    /// </summary>
    public class CalibrationUnit : Detector, ICalibration, IInitializable
    {
        #region IInitializable and Set parameters

        /// <summary>
        /// Foosbot World AXE X length
        /// </summary>
        private int AXE_X_LENGTH;

        /// <summary>
        /// Foosbot World AXE Y length
        /// </summary>
        private int AXE_Y_LENGTH;

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
                IsInitialized = true;
            }
        }
        
        #endregion IInitializable and Set parameters

        #region Calibration Constants

        /// <summary>
        /// Approx. Inner Radius of Calibration Circle
        /// </summary>
        public const int INNER_RADIUS = 30;//60;

        /// <summary>
        /// Approx. Outer Radius of Calibration Circle
        /// </summary>
        public const int OUTER_RADIUS = 50;//80;

        /// <summary>
        /// Possible error for inner and outer radius
        /// </summary>
        public const double RADIUS_THRESHOLD = 1;

        /// <summary>
        /// Possible Error for distance between inner and outer mark radius
        /// </summary>
        public const double DISTANCE_ERROR = 0.1;

        /// <summary>
        /// Frames to skip in case of unsuccessful calibration before retry
        /// </summary>
        public const int FRAMES_TO_SKIP = 10;

        #endregion Callibration Constants

        #region private members

        /// <summary>
        /// Calibration mark coordinates on table by names in dictionary
        /// </summary>
        private Dictionary<eCallibrationMark, System.Drawing.PointF> _markCoordinates;

        /// <summary>
        /// Frames to skip counter in case of unsuccessful calibration
        /// </summary>
        private int _skipFrames = 0;

        #endregion private members

        #region ICalibration

        /// <summary>
        /// Current Calibration State
        /// </summary>
        public eCalibrationState CalibrationState { get; private set; }

        /// <summary>
        /// Sorted Calibration Marks Coordinates on original image
        /// </summary>
        public Dictionary<eCallibrationMark, CircleF> CalibrationMarks { get; set; }

        /// <summary>
        /// Ball Radius
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        /// Ball Radius Error Rate
        /// </summary>
        public double ErrorRate { get; set; }

        #endregion ICallibration

        /// <summary>
        /// Calibration Unit Constructor
        /// </summary>
        public CalibrationUnit() { }

        /// <summary>
        /// Start calibration process
        /// </summary>
        /// <param name="source">Image to perform calibration on it</param>
        /// <returns>[True] if calibration finished, [False] otherwise</returns>
        public bool InvokeCallibration(Image<Gray, byte> source)
        {
            Initialize();
            switch (CalibrationState)
            {
                case eCalibrationState.NotStarted:
                    CalibrationPhaseI(source);
                    break;
                case eCalibrationState.FinishedPhaseI:
                    CalibrationPhaseII(source);
                    break;
                case eCalibrationState.Finished:
                    Log.Image.Debug("Calibration already finished.");
                    break;
            }
            return CalibrationState.Equals(eCalibrationState.Finished);
        }

        /// <summary>
        /// First Phase of calibration
        /// * Skip unstable frames
        /// * Detect Calibration Circles and sort those
        /// * Calculate Transformation Homography and Invert matrix
        /// * Calculate Ball Radius and possible Error
        /// </summary>
        /// <param name="source">Image from camera to perform calibration on</param>
        private void CalibrationPhaseI(Image<Gray, byte> source)
        {
            //ignore first frames
            if (_skipFrames < FRAMES_TO_SKIP)
            {
                _skipFrames++;
            }
            else
            {
                try
                {
                    _skipFrames = 0;

                    Log.Image.Debug("Starting calibration Phase I...");
                    Image<Gray, byte> image = source.Clone();

                    //Remove Noise from picture
                    CvInvoke.Canny(image, image, 100, 60);
                    image = NoiseRemove(image);

                    //Find Calibration Marks
                    List<CircleF> circles = FindCalibrationMarks(image);
                    VerifyMarksFound(circles);

                    //Sort Calibration Marks and set value to property
                    SortCalibrationMarks(circles);

                    ShowAllCalibrationMarks();
                    StringBuilder str = new StringBuilder("4 calibration Marks found and sorted: \n\t\t\t\t");
                    foreach (var mark in CalibrationMarks)
                        str.Append(String.Format("{0}:[{1}x{2}] ", mark.Key, mark.Value.Center.X, mark.Value.Center.Y));
                    Log.Image.Info(str.ToString());

                    SetTransformationMatrix(AXE_X_LENGTH, AXE_Y_LENGTH);
                    Log.Image.Info("Homography matrix calculated.");

                    //Calculate Ball Radius and Error
                    double originalBallRadius = Configuration.Attributes.GetValue<double>("BallDiameter") / 2;
                    CalculateBallRadiusAndError((float)originalBallRadius);
                    Log.Image.Info(String.Format("Expected ball radius is {0} +/- {1}", Radius, ErrorRate));

                    CalibrationState = eCalibrationState.FinishedPhaseI;
                }
                catch(CalibrationException ex)
                {
                    Log.Image.Warning(String.Format("Calibration failed in phase I. Will retry after [{0}] frames. Reason: {1}",
                           FRAMES_TO_SKIP, ex.Message));
                }
            }
        }

        /// <summary>
        /// Second Phase of calibration
        /// * Skip unstable frames
        /// * Find Calibration Circles
        /// * Extract Background Image
        /// * Crop background Image based o calibration marks
        /// </summary>
        /// <param name="source">Image from camera to perform calibration on</param>
        private void CalibrationPhaseII(Image<Gray, byte> source)
        {
            if (_skipFrames < FRAMES_TO_SKIP)
            {
                _skipFrames++;
            }
            else
            {
                try
                {
                    _skipFrames = 0;

                    Log.Image.Debug("Starting calibration Phase II...");
                    Image<Gray, byte> image = source.Clone();

                    Image<Gray, byte> tempImage = image.Clone();

                    //Remove Noise from picture
                    CvInvoke.Canny(tempImage, tempImage, 100, 60);
                    tempImage = NoiseRemove(tempImage);

                    //Find Calibration Marks
                    List<CircleF> circles = FindCalibrationMarks(tempImage);
                    VerifyMarksFound(circles);

                    //Update coverage
                    UpdateCalibrationMarks();

                    CalibrationState = eCalibrationState.Finished;
                    Log.Image.Info("Calibration finished!");
                }
                catch (CalibrationException ex)
                {
                    Log.Image.Warning(String.Format("Calibration failed in phase II. Will retry after [{0}] frames. Reason: {1}",
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
            CircleF[] possibleInnerCircles = DetectCircles(image, INNER_RADIUS, RADIUS_THRESHOLD, (double)OUTER_RADIUS * 5
                , 180.0, 120.0, 2.0);

            //Find Calibration Small Circles
            CircleF[] possibleOuterCircles = DetectCircles(image, OUTER_RADIUS, RADIUS_THRESHOLD, (double)OUTER_RADIUS * 5
                , 180.0, 120.0, 2.0);

            //Find corresponding circles
            foreach (CircleF innerCircle in possibleInnerCircles)
            {
                foreach (CircleF outerCircle in possibleOuterCircles)
                {
                    if (Utils.Distance(innerCircle.Center, outerCircle.Center) < DISTANCE_ERROR * OUTER_RADIUS)
                    {
                        circles.Add(innerCircle);
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

            CalibrationMarks = new Dictionary<eCallibrationMark, CircleF>();

            Dictionary<CircleF, CircleF> diagonalPairs = FindDiagonalMarkPairs(unsortedMarks);

            //In each pair find left point, right, bottom and top and set corresponding marks
            foreach (CircleF key in diagonalPairs.Keys)
            {
                CircleF buttom = (key.Center.Y > diagonalPairs[key].Center.Y) ? key : diagonalPairs[key];
                CircleF top = (key.Center.Y < diagonalPairs[key].Center.Y) ? key : diagonalPairs[key];
                CircleF left = (key.Center.X < diagonalPairs[key].Center.X) ? key : diagonalPairs[key];
                CircleF right = (key.Center.X > diagonalPairs[key].Center.X) ? key : diagonalPairs[key];

                if (buttom.Equals(left))
                    CalibrationMarks.Add(eCallibrationMark.BL, left);
                if (top.Equals(left))
                    CalibrationMarks.Add(eCallibrationMark.TL, left);
                if (top.Equals(right))
                    CalibrationMarks.Add(eCallibrationMark.TR, right);
                if (buttom.Equals(right))
                    CalibrationMarks.Add(eCallibrationMark.BR, right);
            }
        }

        /// <summary>
        /// Find Diagonal pairs of detected calibration marks.
        /// Based on assumption those pairs have largest distance.
        /// </summary>
        /// <param name="unsortedMarks">Unsorted marks list</param>
        /// <returns>Dictionary of 2 diagonal pairs of marks</returns>
        private Dictionary<CircleF, CircleF> FindDiagonalMarkPairs(List<CircleF> unsortedMarks)
        {
            //Find All Pairs based on distance
            Dictionary<CircleF, CircleF> pairs = new Dictionary<CircleF, CircleF>();

            //Go over all unsorted marks 
            for (int i = 0; i < 4; i++)
            {
                //Add mark to dictionary as key and add as value mark with largest distance
                pairs.Add(unsortedMarks[i], default(CircleF));
                double largestDist = 0;
                for (int j = 0; j < 4; j++)
                {
                    double distance = Utils.Distance(unsortedMarks[i].Center, unsortedMarks[j].Center);
                    if (i != j && distance > largestDist)
                    {
                        pairs[unsortedMarks[i]] = unsortedMarks[j];
                        largestDist = distance;
                    }
                }
            }

            //Verify found 4 pairs while two of those are identical
            for (int i = 0; i < 4; i++)
            {
                CircleF expectedMark = unsortedMarks[i];
                CircleF keyMark = pairs[unsortedMarks[i]];
                CircleF valueMark = pairs[keyMark];
                if (!valueMark.Equals(expectedMark))
                    throw new CalibrationException("Pairs Calculated Wrong!");
            }

            //Find and Remove duplicates
            List<CircleF> keysToRemove = new List<CircleF>();
            foreach (CircleF mark in pairs.Keys)
                if (!keysToRemove.Contains(pairs[mark]))
                    keysToRemove.Add(mark);

            foreach (CircleF key in keysToRemove)
                pairs.Remove(key);

            return pairs;
        }

        private void SetTransformationMatrix(int axeXlength, int axeYlength)
        {
            _markCoordinates = new Dictionary<eCallibrationMark, System.Drawing.PointF>();
            _markCoordinates.Add(eCallibrationMark.BL, new System.Drawing.PointF(0, axeYlength));             //ButtomLeft
            _markCoordinates.Add(eCallibrationMark.TL, new System.Drawing.PointF(0, 0));                         //TopLeft
            _markCoordinates.Add(eCallibrationMark.TR, new System.Drawing.PointF(axeXlength, 0));               //TopRight
            _markCoordinates.Add(eCallibrationMark.BR, new System.Drawing.PointF(axeXlength, axeYlength));   //ButtomRight

            System.Drawing.PointF[] orriginalPointArray = _markCoordinates.Values.ToArray();
            System.Drawing.PointF[] arrangedPoints = new System.Drawing.PointF[4];

            arrangedPoints[0] = CalibrationMarks[eCallibrationMark.BL].Center;
            arrangedPoints[1] = CalibrationMarks[eCallibrationMark.TL].Center;
            arrangedPoints[2] = CalibrationMarks[eCallibrationMark.TR].Center;
            arrangedPoints[3] = CalibrationMarks[eCallibrationMark.BR].Center;

            _transformer = new Transformation();
            _transformer.FindHomographyMatrix(arrangedPoints, orriginalPointArray);
        }

        Transformation _transformer;

        /// <summary>
        /// Show all calibration marks on screen
        /// </summary>
        private void ShowAllCalibrationMarks()
        {
            foreach (KeyValuePair<eCallibrationMark, CircleF> mark in CalibrationMarks)
            {
                ShowCalibrationMark(mark.Value, mark.Key);
            }
        }

        /// <summary>
        /// Show calibration mark on screen - circle and coordinates
        /// </summary>
        /// <param name="mark">Calibration mark as CircleF</param>
        /// <param name="key">Calibration mark type</param>
        private void ShowCalibrationMark(CircleF mark, eCallibrationMark key)
        {
            switch (key)
            {
                case eCallibrationMark.BL:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BL,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.TL:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.TL,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.TR:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.TR,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.BR:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BR,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
            }

        }

        /// <summary>
        /// Calculate ball radius and possible error 
        /// </summary>
        /// <param name="origRadius">Original Ball Radius in mm</param>
        private void CalculateBallRadiusAndError(float origRadius)
        {
            double minRadius = origRadius * 100;
            double maxRadius = 0;
            foreach (System.Drawing.PointF start in _markCoordinates.Values)
            {
                System.Drawing.PointF end = new System.Drawing.PointF(start.X + origRadius, start.Y);

                double check = Utils.Distance(start, end);

                System.Drawing.PointF transformedStart = _transformer.InvertTransform(start);
                System.Drawing.PointF transformedEnd = _transformer.InvertTransform(end);

                double radius = Utils.Distance(transformedStart, transformedEnd);

                if (radius > maxRadius)
                    maxRadius = radius;
                if (radius < minRadius)
                    minRadius = radius;
            }

            Radius = Convert.ToInt32((minRadius + maxRadius) / 2);
            double possibleError = ((maxRadius - minRadius) / 2) + 1;
            ErrorRate = possibleError / Radius;
        }

        /// <summary>
        /// Verifies exactly 4 calibration marks found
        /// </summary>
        /// <param name="circles">Calibration marks circles</param>
        /// <exception cref="CalibrationException">Thrown in case not exactly 4 calibration marks found</exception>
        private void VerifyMarksFound(List<CircleF> circles)
        {
            if (circles.Count != 4)
            {
                throw new CalibrationException(String.Format(
                    "Number of marks found in calibration is [{0}], while expected [4] marks.", circles.Count));
            }
        }

        /// <summary>
        /// Update Calibration Marks in order to get better coverage
        /// </summary>
        private void UpdateCalibrationMarks()
        {
            //Create the updated marks
            Dictionary<eCallibrationMark, CircleF> updatedMaks = new Dictionary<eCallibrationMark, CircleF>();
            foreach (var mark in CalibrationMarks)
            {
                switch (mark.Key)
                {
                    case eCallibrationMark.BL:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X - mark.Value.Radius,
                            mark.Value.Center.Y + mark.Value.Radius), mark.Value.Radius));
                        break;
                    case eCallibrationMark.BR:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X + mark.Value.Radius,
                            mark.Value.Center.Y + mark.Value.Radius), mark.Value.Radius));
                        break;
                    case eCallibrationMark.TL:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X - mark.Value.Radius,
                            mark.Value.Center.Y - mark.Value.Radius), mark.Value.Radius));
                        break;
                    case eCallibrationMark.TR:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X + mark.Value.Radius,
                            mark.Value.Center.Y - mark.Value.Radius), mark.Value.Radius));
                        break;
                }
            }

            //set updated marks
            foreach (var mark in updatedMaks)
                CalibrationMarks[mark.Key] = mark.Value;

            //Show updated marks
            ShowAllCalibrationMarks();

            //Recalculate Homography Matrix
            SetTransformationMatrix(AXE_X_LENGTH, AXE_Y_LENGTH);
            Log.Image.Info("Homography matrix re-calculated.");
        }
    }
}
