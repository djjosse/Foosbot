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
using Foosbot.Common.Data;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessingUnit.Detection.Contracts;
using Foosbot.ImageProcessingUnit.Detection.Enums;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Process.Core;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Core;
using Foosbot.ImageProcessingUnit.Tools.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Foosbot.ImageProcessingUnit.Detection.Core
{
    /// <summary>
    /// Ball Tracker Class
    /// </summary>
    public class BallTracker : Tracker
    {
        #region constants

        /// <summary>
        /// Default Canny Threshold for circle detection
        /// </summary>
        public const double DEFAULT_CANNY_THRESHOLD = 250;

        /// <summary>
        /// Default Circle Accumulator Threshold for circle detection
        /// </summary>
        public const double DEFAULT_CIRCLE_ACCUMULATOR_THRESHOLD = 37;

        /// <summary>
        /// Default Inverse Ratio for circle detection
        /// </summary>
        public const double DEFAULT_INVERSE_RATIO = 1.5;

        /// <summary>
        /// Maximum possible ball speed in pixels per seconds
        /// This will affect partial aria calculation in case of stored location from previous frame
        /// </summary>
        public const double MAX_BALL_SPEED = 2000;

        #endregion constants

        /// <summary>
        /// Streamer instance is used to verify if we have new frames
        /// </summary>
        private FramePublisher _streamer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imagingData">Common imaging data for all Image Processing Unit</param>
        /// <param name="streamer">Frame Streamer used to check if we received new frames</param>
        /// <param name="preprocessor">Image Preparation unit [default is null - will be created]</param>
        /// <param name="motionInspector">Motion Detection unit [default is null - will be created</param>
        public BallTracker(IImageData imagingData, FramePublisher streamer, IImagePreparation preprocessor = null, IMotionDetector motionInspector = null)
            : base(imagingData, preprocessor, motionInspector)
        {
            _streamer = streamer;
            CannyThreshold = DEFAULT_CANNY_THRESHOLD;
            CircleAccumulator = DEFAULT_CIRCLE_ACCUMULATOR_THRESHOLD;
            InverseRatio = DEFAULT_INVERSE_RATIO;
        }

        /// <summary>
        /// Maint Ball Detection method
        /// This method should be called each time for each frame
        /// </summary>
        /// <param name="frame">Frame to search for ball on</param>
        /// <returns>[True] if found, [False] otherwise</returns>
        public override bool Detect(IFrame frame)
        {
            //Detection Flag
            bool isDetected = false;

            //Has new frame in Streamer Flag
            bool hasNewFrame = false;

            //Prepare Image
            Image<Gray, byte> image = frame.Image;
            image = PreProcessor.Prepare(frame.Image);
            ComputerVisionMonitors[eComputerVisionMonitor.MonitorA].ShowFrame(image);

            //Crop Image and Store Offsets
            image = CropAndUpdate(image, ImagingData.CalibrationMarks.Values.ToList());
            ComputerVisionMonitors[eComputerVisionMonitor.MonitorB].ShowFrame(image);

            //If there are stored coordinates and they defined
            if (ImagingData.LastKnownBallLocation != null && ImagingData.LastKnownBallLocation.IsDefined)
            {
                isDetected = FindBallLocationInFrame(image, frame.Timestamp, eDetectionArea.Selected);
                if (!isDetected)
                {
                    //Set location to undefined if we failed to detect
                    ImagingData.BallCoords = new BallCoordinates(frame.Timestamp);

                    //Check if there are new frames in streamer
                    hasNewFrame = IsStreamerHasNewFrame(frame.Timestamp);
                    if (hasNewFrame)
                    {
                        //Skip current Frame
                        Log.Image.Warning(String.Format("[{0}] Skipping current frame - already have new image",
                                MethodBase.GetCurrentMethod().Name));
                    }
                    else
                    {
                        Log.Image.Info(String.Format("[{0}] Unable to find in small area, searching full area: {1}x{2}",
                                MethodBase.GetCurrentMethod().Name, image.Width, image.Height));
                        isDetected = FindBallLocationInFrame(image, frame.Timestamp);
                    }
                }
                else
                {
                    Log.Image.Info(String.Format("[{0}] Found based on stored location. Updated to: [{1}x{2}]",
                        MethodBase.GetCurrentMethod().Name, ImagingData.LastKnownBallLocation.X, ImagingData.LastKnownBallLocation.Y));
                }
            }
            //No stored location
            else 
            {
                isDetected = FindBallLocationInFrame(image, frame.Timestamp);
            }

            if (!isDetected && !hasNewFrame)
            {
                //Use Motion detection
                isDetected = MotionInspector.Detect(image);
                if (isDetected)
                {
                    double xLocation = MotionInspector.DetectedLocation.X;
                    double yLocation = MotionInspector.DetectedLocation.Y;
                    UpdateCoordinates(xLocation, yLocation, frame.Timestamp, Brushes.Green);
                }
            }
            else
            {
                //Update Motion history
                MotionInspector.UpdateMotionHisitory(image);
            }

            return isDetected;
        }

        /// <summary>
        /// Find ball location in frame in defined area
        /// </summary>
        /// <param name="image">Image to detect ball</param>
        /// <param name="timeStamp">Time Stamp of an image</param>
        /// <param name="detectionArea">Area to search ball in. If Selected:
        /// area will be defined based on last stored location and maximum possible speed.
        /// [Default is Full to search in all frame]
        /// </param>
        /// <returns>[True] if ball location found, [False] otherwise</returns>
        public override bool FindBallLocationInFrame(Image<Gray, byte> image, DateTime timeStamp, eDetectionArea detectionArea = eDetectionArea.Full)
        {
            using (image = image.Clone())
            {
                int additionalOffsetX = 0;
                int additionalOffsetY = 0;

                if (detectionArea.Equals(eDetectionArea.Selected))
                {
                    TimeSpan deltaT = timeStamp - ImagingData.LastKnownBallLocation.Timestamp;
                    double searchRadius = MAX_BALL_SPEED * deltaT.TotalSeconds;

                    int maxX = (Convert.ToInt32(ImagingData.LastKnownBallLocation.X + searchRadius) > image.Width) ?
                        image.Width : Convert.ToInt32(ImagingData.LastKnownBallLocation.X + searchRadius);
                    int maxY = (Convert.ToInt32(ImagingData.LastKnownBallLocation.Y + searchRadius) > image.Height) ?
                        image.Height : Convert.ToInt32(ImagingData.LastKnownBallLocation.Y + searchRadius);
                    additionalOffsetX = (Convert.ToInt32(ImagingData.LastKnownBallLocation.X - searchRadius) < 0) ?
                        0 : Convert.ToInt32(ImagingData.LastKnownBallLocation.X - searchRadius);
                    additionalOffsetY = (Convert.ToInt32(ImagingData.LastKnownBallLocation.Y - searchRadius) < 0) ?
                        0 : Convert.ToInt32(ImagingData.LastKnownBallLocation.Y - searchRadius);

                    List<System.Drawing.PointF> croppingPoints = new List<System.Drawing.PointF>()
                    {
                        new System.Drawing.PointF(maxX, maxY),
                        new System.Drawing.PointF(maxX, additionalOffsetY),
                        new System.Drawing.PointF(additionalOffsetX, maxY),
                        new System.Drawing.PointF(additionalOffsetX, additionalOffsetY)
                    };
                    
                    image = Crop(image, croppingPoints);
                    ComputerVisionMonitors[eComputerVisionMonitor.MonitorC].ShowFrame(image);
                }

                CircleF[] pos = DetectCircles(image, ImagingData.BallRadius, ImagingData.BallRadiusError * 2, ImagingData.BallRadius * 5);
                if (pos.Length > 0)
                {
                    double xLocation = pos[0].Center.X + additionalOffsetX;
                    double yLocation = pos[0].Center.Y + additionalOffsetY;
                    UpdateCoordinates(xLocation, yLocation, timeStamp, Brushes.Red);
                    return true;
                }
                Log.Image.Debug(String.Format("[{0}] Ball not found in {1} area", MethodBase.GetCurrentMethod().Name, detectionArea.ToString()));
                ImagingData.BallCoords = new BallCoordinates(timeStamp);
                return false;
            }
        }

        /// <summary>
        /// Check if streamer has already a new frame
        /// </summary>
        /// <param name="currentTimeStamp">Current frame time stamp</param>
        /// <returns>[True] if streamer already has new frame to get, [False] otherwise</returns>
        private bool IsStreamerHasNewFrame(DateTime currentTimeStamp)
        {
            //Last Frame in streamer Time Stamp
            DateTime stamp = _streamer.Data.Timestamp;
            return !(stamp.Equals(currentTimeStamp));
        }

        /// <summary>
        /// Update Last Know Location and Calculate new ball coordinates
        /// </summary>
        /// <param name="xLocation">Calculated X location [Foosbot world coordinates]</param>
        /// <param name="yLocation">Calculated Y location [Foosbot world coordinates]</param>
        /// <param name="timeStamp">Current image time stamp</param>
        /// <param name="detectionColor">Color to draw ball on screen</param>
        private void UpdateCoordinates(double xLocation, double yLocation, DateTime timeStamp, SolidColorBrush detectionColor)
        {
            ImagingData.LastKnownBallLocation = new BallLocation(xLocation, yLocation, timeStamp);

            int x = ImagingData.LastKnownBallLocation.X + OffsetX;
            int y = ImagingData.LastKnownBallLocation.Y + OffsetY;

            Marks.DrawBall(new System.Windows.Point(x, y), Convert.ToInt32(20), detectionColor);

            System.Drawing.PointF coordinates = TransformAgent.Data.Transform(new System.Drawing.PointF(x, y));

            ImagingData.BallCoords = new BallCoordinates(Convert.ToInt32(coordinates.X), Convert.ToInt32(coordinates.Y), timeStamp);

            Statistics.UpdateBallCoordinates(
                String.Format("[IP Unit] Ball coordinates: {0}x{1}", ImagingData.BallCoords.X, ImagingData.BallCoords.Y));
        }
    }
}
