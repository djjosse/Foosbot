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

namespace Foosbot.ImageProcessingUnit.Process.Core
{
    /// <summary>
    /// Frame Observer
    /// This is abstract class to be implemented by Image Processing Class or GUI Screen Monitor
    /// </summary>
    public abstract class FrameObserver : Observer<IFrame>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="streamer">Streamer to get frames from</param>
        public FrameObserver(Publisher<IFrame> streamer) : base(streamer) { }
    }
}
