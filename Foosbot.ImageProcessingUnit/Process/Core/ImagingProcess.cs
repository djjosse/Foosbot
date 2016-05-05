// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Core;
using Foosbot.ImageProcessingUnit.Tools.Core;

namespace Foosbot.ImageProcessingUnit.Process.Core
{
    /// <summary>
    /// Abstract image processing unit model 
    /// to be implemented by Demo or Real Image Processing Unit
    /// </summary>
    public abstract class ImagingProcess : FrameObserver, IInitializable
    {
        /// <summary>
        /// Constructor for abstract image processing unit
        /// </summary>
        /// <param name="streamer">Publisher to get frames from</param>
        /// <param name="imagingData">Imaging Data [default is null - will be created inside this class]</param>
        public ImagingProcess(FramePublisher streamer, IImageData imagingData = null) : base(streamer) 
        {
            ImagingData = (imagingData != null) ? imagingData : new ImageData();
            BallLocationUpdater = new BallPublisher(ImagingData);

            //create computer vision monitor to display image processing data
            ImageProcessingMonitorA = new ComputerVisionFramesPublisher();
            ImageProcessingMonitorB = new ComputerVisionFramesPublisher();
            ImageProcessingMonitorC = new ComputerVisionFramesPublisher();
            ImageProcessingMonitorD = new ComputerVisionFramesPublisher();
        }

        /// <summary>
        /// Publisher of Image Processing Frames - modified frames to see
        /// what image processing unit does with frame and how computer vision works.
        /// </summary>
        public ComputerVisionFramesPublisher ImageProcessingMonitorA { get; protected set; }

        /// <summary>
        /// Publisher of Image Processing Frames - modified frames to see
        /// what image processing unit does with frame and how computer vision works.
        /// </summary>
        public ComputerVisionFramesPublisher ImageProcessingMonitorB { get; protected set; }

        /// <summary>
        /// Publisher of Image Processing Frames - modified frames to see
        /// what image processing unit does with frame and how computer vision works.
        /// </summary>
        public ComputerVisionFramesPublisher ImageProcessingMonitorC { get; protected set; }

        /// <summary>
        /// Publisher of Image Processing Frames - modified frames to see
        /// what image processing unit does with frame and how computer vision works.
        /// </summary>
        public ComputerVisionFramesPublisher ImageProcessingMonitorD { get; protected set; }

        /// <summary>
        /// Last Known Ball Coordinates Data To publish
        /// </summary>
        public BallPublisher BallLocationUpdater { get; protected set; }

        /// <summary>
        /// Image Data relevant to all units
        /// </summary>
        public IImageData ImagingData { get; protected set; }

        /// <summary>
        /// Is Initialized Property
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Abstract initialization method
        /// </summary>
        public abstract void Initialize();
    }
}
