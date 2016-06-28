// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.Common.Enums
{
    /// <summary>
    /// Keys used to represent statistics in GUI
    /// </summary>
    public enum eStatisticsKey
    {
        /// <summary>
        /// Current FPS, Width and Heigth
        /// </summary>
        FrameInfo = 1,

        /// <summary>
        /// Foosbot Memory and CPU Info
        /// </summary>
        ProccessInfo = 2,

        /// <summary>
        /// Percentage of successful ball detection and average detection time
        /// </summary>
        BasicImageProcessingInfo = 3,

        /// <summary>
        /// Ball Coordinates
        /// </summary>
        BallCoordinates = 4
    }
}
