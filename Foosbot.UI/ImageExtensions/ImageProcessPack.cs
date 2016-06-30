// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.DevelopmentDemo;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Process.Core;
using Foosbot.ImageProcessingUnit.Streamer.Core;
using System.Windows.Threading;

namespace Foosbot.UI.ImageExtensions
{
    /// <summary>
    /// The Actual image processing pack contains all the elements for Image Processing
    /// </summary>
    public class ImageProcessPack : IImageProcessingPack
    {
        /// <summary>
        /// Is Image Processing Pack Instance Already exists flag
        /// </summary>
        private static bool _isCreated = false;

        /// <summary>
        /// Is Image Processing Pack Is Already Started and Working flag
        /// </summary>
        private bool _isWorking = false;

        /// <summary>
        /// Streamer to get all the frames to work with
        /// </summary>
        public FramePublisher Streamer { get; private set; }

        /// <summary>
        /// User Interface Monitor to publish the received frames
        /// </summary>
        public FrameObserver UiMonitor { get; private set; }

        /// <summary>
        /// Actual Image processing unit
        /// </summary>
        public ImagingProcess ImageProcessUnit { get; private set; }

        /// <summary>
        /// Circle Detection Pre-Processing Gray Threshold
        /// </summary>
        public int CircleDetectionGrayThreshold
        {
            get
            {
                return ImageProcessUnit.CircleDetectionGrayThreshold;
            }
            set
            {
                ImageProcessUnit.CircleDetectionGrayThreshold = value;
            }
        }

        /// <summary>
        /// Circle Detection Canny Threshold for Circle Edge Detection
        /// </summary>
        public double CircleDetectionCannyThreshold
        {
            get
            {
                return ImageProcessUnit.CircleDetectionCannyThreshold;
            }
            set
            {
                ImageProcessUnit.CircleDetectionCannyThreshold = value;
            }
        }

        /// <summary>
        /// Circle Accumulator Threshold for Circle Detection
        /// </summary>
        public double CircleDetectionAccumulatorThreshold
        {
            get
            {
                return ImageProcessUnit.CircleDetectionAccumulatorThreshold;
            }
            set
            {
                ImageProcessUnit.CircleDetectionAccumulatorThreshold = value;
            }
        }

        /// <summary>
        /// Circle Detection Method Inverse Ration
        /// </summary>
        public double CircleDetectionInverseRatio
        {
            get
            {
                return ImageProcessUnit.CircleDetectionInverseRatio;
            }
            set
            {
                ImageProcessUnit.CircleDetectionInverseRatio = value;
            }
        }

        /// <summary>
        /// Motion Detection Pre-Processing Gray Threshold
        /// </summary>
        public int MotionDetectionGrayThreshold
        {
            get
            {
                return ImageProcessUnit.MotionDetectionGrayThreshold;
            }
            set
            {
                ImageProcessUnit.MotionDetectionGrayThreshold = value;
            }
        }

        /// <summary>
        /// Minimal Motion Area Threshold
        /// </summary>
        public double MinimalMotionAreaThreshold
        {
            get
            {
                return ImageProcessUnit.MinimalMotionAreaThreshold;
            }
            set
            {
                ImageProcessUnit.MinimalMotionAreaThreshold = value;
            }
        }

        /// <summary>
        /// Factor for Minimal Motion Pixels in Motion Area
        /// </summary>
        public double MinimalMotionPixelsFactor
        {
            get
            {
                return ImageProcessUnit.MinimalMotionPixelsFactor;
            }
            set
            {
                ImageProcessUnit.MinimalMotionPixelsFactor = value;
            }
        }

        /// <summary>
        /// Private Constructor for Image Processing Pack to be called in factory method
        /// </summary>
        /// <param name="framePublisher">Frame Publisher (streamer)</param>
        /// <param name="uiMonitor">User Interface Frame monitor to show frames</param>
        /// <param name="imagingProcess">Image Processing Unit</param>
        private ImageProcessPack(FramePublisher framePublisher, FrameUiMonitor uiMonitor, ImagingProcess imagingProcess)
        {
            Streamer = framePublisher;
            UiMonitor = uiMonitor;
            ImageProcessUnit = imagingProcess;
            _isCreated = true;
        }

        /// <summary>
        /// Destructor for Image Processing Pack
        /// </summary>
        ~ImageProcessPack()
        {
            _isCreated = false;
        }

        /// <summary>
        /// Start working the image processing pack
        /// </summary>
        public void Start()
        {
            if (!_isWorking)
            {
                Streamer.Start();
                UiMonitor.Start();
                ImageProcessUnit.Start();
                _isWorking = true;
            }
        }

        /// <summary>
        /// Is currently flow paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
               return ImageProcessUnit.IsPaused;
            }
        }

        /// <summary>
        /// Pause image processing
        /// </summary>
        public void Pause()
        {
            if (_isWorking)
            {
                ImageProcessUnit.Pause();
            }
        }

        /// <summary>
        /// Resume Image Processing
        /// </summary>
        public void Resume()
        {
            if (_isWorking)
            {
                ImageProcessUnit.Resume();
            }
        }

        /// <summary>
        /// Selected mode in configuration file property
        /// </summary>
        public static bool IsDemoMode { get; private set; }

        /// <summary>
        /// Factory method - Builds Image Processing Unit and all relevant components
        /// </summary>
        /// <param name="dispatcher">Dispatcher used to present frames in GUI relevant thread</param>
        /// <param name="screen">Canvas to draw frames on</param>
        /// <returns>ImageProcessPack with all components</returns>
        public static ImageProcessPack Create(Dispatcher dispatcher, System.Windows.Controls.Canvas screen)
        {
            if (!_isCreated)
            {
                IsDemoMode = Configuration.Attributes.GetValue<bool>(Configuration.Names.KEY_IS_DEMO_MODE);

                //Frame/Demo streamer
                FramePublisher framePublisher;
                if (!IsDemoMode)
                {
                    framePublisher = new FrameStreamer();
                }
                else
                {
                    framePublisher = new DemoStreamer();
                }

                FrameUiMonitor uiMonitor = new FrameUiMonitor(framePublisher, dispatcher, screen);

                //Initialize Marks
                Marks.Initialize(dispatcher, screen, framePublisher.FrameWidth, framePublisher.FrameHeight);

                //Image/Demo processing unit
                ImagingProcess imagingProcess;
                if (!IsDemoMode)
                {
                    imagingProcess = new FrameProcessingUnit(framePublisher);
                }
                else
                {
                    imagingProcess = new DemoProcessingUnit(framePublisher);
                    imagingProcess.Initialize();
                }

                return new ImageProcessPack(framePublisher, uiMonitor, imagingProcess);
            }
            return null;
        }
    }
}
