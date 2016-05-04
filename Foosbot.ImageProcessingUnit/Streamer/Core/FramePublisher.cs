// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Multithreading;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using System;

namespace Foosbot.ImageProcessingUnit.Streamer.Core
{
    /// <summary>
    /// Abstract derived class from Publisher is used to publish
    /// frames to GUI monitor and Image Processing Unit
    /// </summary>
    public abstract class FramePublisher : Publisher<IFrame>, IStreamer
    {
        /// <summary>
        /// Frame Width
        /// </summary>
        public int FrameWidth { get; protected set; }

        /// <summary>
        /// Frame Height
        /// </summary>
        public int FrameHeight { get; protected set; }

        /// <summary>
        /// Frame Rate (FPS)
        /// </summary>
        public int FrameRate { get; protected set; }

        /// <summary>
        /// Start Method - run publisher in thread
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Process frame method used in start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void ProcessFrame(object sender, EventArgs e);

        /// <summary>
        /// Update Diagnostic Info method for GUI
        /// </summary>
        protected abstract void UpdateDiagnosticInfo();
        
        /// <summary>
        /// Sets requested camera configuration from Configuration File
        /// </summary>
        /// <param name="frameWidth">Frame Width to use [default is -1 then parameter from configuration file will be used]</param>
        /// <param name="frameHeight">Frame Height to use [default is -1 then parameter from configuration file will be used]</param>
        /// <param name="frameRate">Frame Rate to use [default is -1 then parameter from configuration file will be used]</param>
        protected virtual void SetCameraConfiguration(int frameWidth = -1, int frameHeight = -1, int frameRate = -1)
        {
            FrameWidth = (frameWidth > 0) ? frameWidth : Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_IPU_FRAME_WIDTH);
            FrameHeight = (frameHeight > 0) ? frameHeight : Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_IPU_FRAME_HEIGHT);
            FrameRate = (frameRate > 0) ? frameRate : Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_IPU_FRAME_RATE);
        }
    }
}
