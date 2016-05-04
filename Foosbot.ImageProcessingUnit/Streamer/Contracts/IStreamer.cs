// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.ImageProcessingUnit.Streamer.Contracts
{
    /// <summary>
    /// Interface for class implementing streamer provides streamer relevant data
    /// </summary>
    public interface IStreamer
    {
        /// <summary>
        /// Frame Width
        /// </summary>
        int FrameWidth { get; }

        /// <summary>
        /// Frame Height
        /// </summary>
        int FrameHeight { get; }

        /// <summary>
        /// Frame Rate (FPS)
        /// </summary>
        int FrameRate { get; }
    }
}
