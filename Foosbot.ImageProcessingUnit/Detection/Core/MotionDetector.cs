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
using Emgu.CV.Util;
using Emgu.CV.VideoSurveillance;
using Foosbot.ImageProcessingUnit.Detection.Contracts;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Core;
using Foosbot.ImageProcessingUnit.Tools.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading;

namespace Foosbot.ImageProcessingUnit.Detection.Core
{
    /// <summary>
    /// Motion Detection for Ball
    /// Here we assume the ball is WHITE and reject all other gray/black motions
    /// </summary>
    public class MotionDetector : BackgroundFlow, IMotionDetector
    {
        #region constants

        /// <summary>
        /// Factor to reduce image size on image preparation
        /// </summary>
        public const double SCALE_ON_PREPARE = 0.25;

        /// <summary>
        /// Default threshold to define a motion area, reduce the value to detect smaller motion
        /// </summary>
        public const double DEFAULT_MIN_MOTION_AREA_THRESHOLD = 20;

        /// <summary>
        /// Default factor to reject the area that contains too few motion
        /// </summary>
        public const double DEFAULT_MIN_MOTION_PIXEL_FACTOR = 0.1;

        /// <summary>
        /// The duration of motion history you wants to keep (Seconds)
        /// </summary>
        public const double MOTION_HISTORY_DURATION = 0.05;

        /// <summary>
        /// Maximum Delta for cvCalcMotionGradient for motion history(Seconds)
        /// </summary>
        public const double MOTION_HISTORY_MAX_DELTA = 0.05;

        /// <summary>
        /// Minimum Delta for cvCalcMotionGradient for motion history(Seconds)
        /// </summary>
        public const double MOTION_HISTORY_MIN_DELTA = 0.5;

        /// <summary>
        /// Default gray Threshold for image Pre-Processing before motion detection
        /// </summary>
        public const int DEFAULT_GRAY_THRESHOLD = 240;

        #endregion constants

        #region Properties

        /// <summary>
        /// Threshold to define a motion area, reduce the value to detect smaller motion
        /// </summary>
        public double MinMotionAreaThreshold { get; set;}

        /// <summary>
        /// Factor to reject the area that contains too few motion
        /// </summary>
        public double MinMotionPixelFactor { get; set; }

        /// <summary>
        /// Gray Threshold for image Pre-Processing before motion detection
        /// </summary>
        public int GrayThreshold { get; set; }

        /// <summary>
        /// Detected Motion Location on provided to Detect method image
        /// </summary>
        public Point DetectedLocation { get; private set; }

        /// <summary>
        /// Common Imaging Data used in Image Processing Unit
        /// </summary>
        public IImageData ImagingData { get; set; }

        /// <summary>
        /// Computer Vision Monitor Dictionary used to show processed frames on the screen
        /// </summary>
        public Dictionary<eComputerVisionMonitor, IComputerVisionMonitor> ComputerVisionMonitors { get; private set; }

        #endregion Properties

        #region Private Members

        /// <summary>
        /// Motion history to update
        /// </summary>
        private MotionHistory _motionHistory;

        /// <summary>
        /// Background Substractor to detect image foreground
        /// </summary>
        private BackgroundSubtractor _forgroundDetector;

        /// <summary>
        /// Sequence of motion components to get from motion history
        /// </summary>
        private Mat _segMask;

        /// <summary>
        /// Foreground - motion picture
        /// </summary>
        private Mat _foreground;

        /// <summary>
        /// Current Received Image to Process
        /// </summary>
        private Image<Gray, byte> _currentImage;

        /// <summary>
        /// Defines if motion was detected on last try
        /// </summary>
        private bool _isDetected;

        #endregion Private Members

        /// <summary>
        /// Motion Detector Constructor
        /// </summary>
        /// <param name="imagingData">Common Image Processing Imaging Data</param>
        public MotionDetector(IImageData imagingData )
        {
            ImagingData = imagingData;

            //Set values for properties
            MinMotionAreaThreshold = DEFAULT_MIN_MOTION_AREA_THRESHOLD;
            MinMotionPixelFactor = DEFAULT_MIN_MOTION_PIXEL_FACTOR;
            GrayThreshold = DEFAULT_GRAY_THRESHOLD;

            //Instantiate private members
            _motionHistory = new MotionHistory(MOTION_HISTORY_DURATION, MOTION_HISTORY_MAX_DELTA, MOTION_HISTORY_MIN_DELTA);
            _forgroundDetector = new BackgroundSubtractorMOG2();
            _segMask = new Mat();
            _foreground = new Mat();

            ComputerVisionMonitors = new Dictionary<eComputerVisionMonitor, IComputerVisionMonitor>();
        }

        /// <summary>
        /// Update Motion History is used to update history without motion detection
        /// This will call another background thread to update.
        /// </summary>
        /// <param name="inputImage">Image to update motion history with</param>
        public void BeginInvokeUpdateMotionHisitory(Image<Gray, byte> inputImage)
        {
            _isDetected = false;
            _currentImage = inputImage.Resize(SCALE_ON_PREPARE, Emgu.CV.CvEnum.Inter.Linear);
            Restart();
        }

        /// <summary>
        /// Detect ball on last image provided to BeginInvokeUpdateMotionHisitory method using motion detection
        /// This will update DetectedLocation property
        /// </summary>
        /// <returns>[True] if found, [False] otherwise (also if image is null)</returns>
        public bool WaitForDetectionResult()
        {
            if (_thread != null && _thread.IsAlive)
            {
                Log.Image.Debug(String.Format("[{0}] Waiting motion detection thread to finish.",
                    MethodBase.GetCurrentMethod().Name));
                try
                {
                    _thread.Join();
                }
                catch(ThreadStateException)
                {
                    //Thread already finished.
                }
            }
            return _isDetected;
        }

        /// <summary>
        /// Detect the ball sets new location if found and value of "is detected" member
        /// </summary>
        private void Detect()
        {
            if (_currentImage == null) return;

            ComputerVisionMonitors[eComputerVisionMonitor.MonitorD].ShowFrame(_foreground.ToImage<Gray, byte>());

            //iterate through each of the motion component
            foreach (Rectangle rectangle in GetMotionAreas())
            {
                int area = rectangle.Width * rectangle.Height;

                //reject the components that have small area;
                if (area < MinMotionAreaThreshold)
                {
                    //Area is to small to process and is rejected, go to next area
                    continue;
                }

                //reject the area that contains too few motion
                if (GetMotionPixelsCount(rectangle) < area * MinMotionPixelFactor)
                {
                    //Area contains to few motion is rejected, go to next region
                    continue;
                }

                //Calculate motion center location, must resize back to original size
                //NOTE: Image was flipped on Horizontally this is the reason calculation differs
                int x = Convert.ToInt32((rectangle.X - (1.5 * rectangle.Width)) / SCALE_ON_PREPARE);
                int y = Convert.ToInt32((rectangle.Y + (rectangle.Height >> 1)) / SCALE_ON_PREPARE);

                DetectedLocation = new Point(x, y);
                _isDetected = true;
                break;
            }
        }

        /// <summary>
        /// Actual update history method 
        /// Runs in background thread on Start() method called
        /// </summary>
        public override void Flow()
        {
            Stopwatch watch = Stopwatch.StartNew();
            UpdateMotionHistory();
            Detect();
            watch.Stop();
            Log.Image.Debug(String.Format("[{0}] Motion detection took: {1} milliseconds",
                    MethodBase.GetCurrentMethod().Name, watch.Elapsed.ToString("fff")));
        }

        /// <summary>
        /// Updates motion history with current image
        /// </summary>
        private void UpdateMotionHistory()
        {
            if (_currentImage != null)
            {
                Image<Gray, byte> image;
                using (image = _currentImage.Clone())
                {
                    image = Prepare(image);
                    ComputerVisionMonitors[eComputerVisionMonitor.MonitorC].ShowFrame(image);
                    Mat motion = new Mat();
                    motion = image.Mat;
                    _forgroundDetector.Apply(motion, _foreground);
                    _motionHistory.Update(_foreground);
                }
            }
        }

        /// <summary>
        /// Get Motion Areas from history
        /// </summary>
        /// <returns>Array of motion areas</returns>
        private Rectangle[] GetMotionAreas()
        {
            Rectangle[] rects;
            using (VectorOfRect boundingRect = new VectorOfRect())
            {
                _motionHistory.GetMotionComponents(_segMask, boundingRect);
                rects = boundingRect.ToArray();
            }
            return rects;
        }

        /// <summary>
        /// Find motion pixel count of the specific area
        /// </summary>
        private double GetMotionPixelsCount(Rectangle rectangle)
        {
            double angle, motionPixelCount;
            _motionHistory.MotionInfo(_foreground, rectangle, out angle, out motionPixelCount);
            return motionPixelCount;
        }

        /// <summary>
        /// Prepare image for motion detection of WHITE ball
        /// </summary>
        /// <param name="image">Current image to prepare</param>
        /// <returns>Prepared Image</returns>
        public Image<Gray, byte> Prepare(Image<Gray, byte> image)
        {
            image = image.ThresholdToZero(new Gray(GrayThreshold));
            image._Dilate(1);
            return image;
        }

        /// <summary>
        /// Abort motion history thread if it is running used in case we received new data
        /// and start it again
        /// </summary>
        private void Restart()
        {
            try
            {
                if (_thread != null && _thread.IsAlive)
                {
                    _thread.Abort();
                }
            }
            catch (Exception e)
            {
                Log.Image.Error(String.Format("[{0}] Error Aborting thread! Reason: {1}",
                    MethodBase.GetCurrentMethod().Name, e.Message));
            }
            finally
            {
                Start();
            }
        }
    }
}
