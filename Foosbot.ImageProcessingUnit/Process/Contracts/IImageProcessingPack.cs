// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.ImageProcessingUnit.Process.Core;
using Foosbot.ImageProcessingUnit.Streamer.Core;

namespace Foosbot.ImageProcessingUnit.Process.Contracts
{
    /// <summary>
    /// Interface for Image Processing Pack
    /// Container for all Image Processing Elements
    /// </summary>
    public interface IImageProcessingPack
    {
        /// <summary>
        /// Streamer to get frames from
        /// </summary>
        FramePublisher Streamer { get; }

        /// <summary>
        /// User Interface Frame Monitor
        /// </summary>
        FrameObserver UiMonitor { get; }

        /// <summary>
        /// Image Processing Unit to processing frames from the streamer
        /// </summary>
        ImagingProcess ImageProcessUnit { get; }

        /// <summary>
        /// Start the Image Processing Pack work
        /// </summary>
        void Start();
    }
}
