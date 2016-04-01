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
using System;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Video Streamer Abstract class
    /// </summary>
    public abstract class Streamer : Publisher<Frame>
    {
        /// <summary>
        /// Frame Rate 
        /// </summary>
        public int FrameRate { get; protected set; }

        /// <summary>
        /// Frame Width
        /// </summary>
        public int FrameWidth { get; protected set; }

        /// <summary>
        /// Frame Height
        /// </summary>
        public int FrameHeight { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Streamer() { }

        /// <summary>
        /// Start streaming
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Frame Process Function called on Image Grabbed Event
        /// </summary>
        protected abstract void ProcessFrame(object sender, EventArgs e);

        /// <summary>
        /// Update Diagnostic Info abstract function
        /// </summary>
        protected abstract void UpdateDiagnosticInfo();
    }
}
