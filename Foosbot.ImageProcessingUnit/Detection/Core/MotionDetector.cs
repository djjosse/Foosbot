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
using System.Drawing;

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
        /// Default threshold to define a motion area, reduce the value to detect smaller motion
        /// </summary>
        public const double DEFAULT_MIN_MOTION_AREA_THRESHOLD = 1000;

        /// <summary>
        /// Default factor to reject the area that contains too few motion
        /// </summary>
        public const double DEFAULT_MIN_MOTION_PIXEL_FACTOR = 0.5;

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
        public const int DEFAULT_GRAY_THRESHOLD = 230;

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
        public int ImagePreProcessingGrayThreshold { get; set; }

        /// <summary>
        /// Detected Motion Location on provided to Detect method image
        /// </summary>
        public Point DetectedLocation { get; private set; }

        /// <summary>
        /// Common Imaging Data used in Image Processing Unit
        /// </summary>
        public IImageData ImagingData { get; set; }

        /// <summary>
        /// Computer Vision Monitor to show Pre-Processed Image
        /// </summary>
        public IComputerVisionMonitor MotionMonitor { get; set; }

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

        #endregion Private Members

        /// <summary>
        /// Motion Detector Constructor
        /// </summary>
        /// <param name="imagingData">Common Image Processing Imaging Data</param>
        public MotionDetector(IImageData imagingData)
        {
            ImagingData = imagingData;

            //Set values for properties
            MinMotionAreaThreshold = DEFAULT_MIN_MOTION_AREA_THRESHOLD;
            MinMotionPixelFactor = DEFAULT_MIN_MOTION_PIXEL_FACTOR;
            ImagePreProcessingGrayThreshold = DEFAULT_GRAY_THRESHOLD;

            //Instantiate private members
            _motionHistory = new MotionHistory(MOTION_HISTORY_DURATION, MOTION_HISTORY_MAX_DELTA, MOTION_HISTORY_MIN_DELTA);
            _forgroundDetector = new BackgroundSubtractorMOG2();
            _segMask = new Mat();
            _foreground = new Mat();
        }

        /// <summary>
        /// Update Motion History is used to update history without motion detection
        /// This will call another background thread to update.
        /// </summary>
        /// <param name="image">Image to update motion history with</param>
        public void UpdateMotionHisitory(Image<Gray, byte> image)
        {
            _currentImage = image;
            AbortMotionHistoryThreadIfRunning();
            Start();
        }

        /// <summary>
        /// Detect ball on image using motion detection
        /// This will update DetectedLocation property
        /// </summary>
        /// <param name="image">Image to find ball on</param>
        /// <returns>[True] if found, [False] otherwise</returns>
        public bool Detect(Image<Gray, byte> image)
        {
            AbortMotionHistoryThreadIfRunning();
            using(image = image.Clone())
            {
                image = Prepare(image);
                Mat motion = new Mat();
                motion = image.Clone().Mat;
                _forgroundDetector.Apply(motion, _foreground);

                //update the motion history
                _motionHistory.Update(_foreground);
                Rectangle[] rects;
                using (VectorOfRect boundingRect = new VectorOfRect())
                {
                    _motionHistory.GetMotionComponents(_segMask, boundingRect);
                    rects = boundingRect.ToArray();
                }

                MotionMonitor.ShowFrame(_foreground.ToImage<Gray, byte>());

                //iterate through each of the motion component
                foreach (Rectangle comp in rects)
                {
                    int area = comp.Width * comp.Height; 
                    //reject the components that have small area;
                    if (area < MinMotionAreaThreshold) continue;

                    // find the angle and motion pixel count of the specific area
                    double angle, motionPixelCount;
                    _motionHistory.MotionInfo(_foreground, comp, out angle, out motionPixelCount);

                    //reject the area that contains too few motion
                    if (motionPixelCount < area * MinMotionPixelFactor) 
                        continue;

                    DetectedLocation = new Point(comp.X + (comp.Width >> 1), comp.Y + (comp.Height >> 1));
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Actual update history method 
        /// Runs in background thread on Start() method called
        /// </summary>
        public override void Flow()
        {
            if (_currentImage != null)
            {
                Image<Gray, byte> image;
                using (image = _currentImage.Clone())
                {
                    image = Prepare(image);

                    Mat motion = new Mat();

                    motion = image.Mat;

                    _forgroundDetector.Apply(motion, _foreground);

                    //update the motion history
                    _motionHistory.Update(_foreground);
                }
            }
        }

        /// <summary>
        /// Prepare image for motion detection of WHITE ball
        /// </summary>
        /// <param name="image">Current image to prepare</param>
        /// <returns>Prepared Image</returns>
        public Image<Gray, byte> Prepare(Image<Gray, byte> image)
        {
            image = image.ThresholdToZero(new Gray(ImagePreProcessingGrayThreshold));
            image._Dilate(1);
            return image;
        }

        /// <summary>
        /// Abort motion history thread if it is running used in case we received new data.
        /// </summary>
        private void AbortMotionHistoryThreadIfRunning()
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
                Log.Image.Debug("Motion History Thread Aborted as motion detection received new data");
            }
        }
    }
}
