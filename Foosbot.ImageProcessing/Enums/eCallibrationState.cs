// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Callibration State Enum
    /// </summary>
    public enum eCallibrationState : int
    {
        /// <summary>
        /// Callibration current state - callibration not started
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Callibration current state - callibration phase I finished
        /// </summary>
        FinishedPhaseI = 1,

        /// <summary>
        /// Callibration current state - callibration phase II finished
        /// </summary>
        Finished = 2
    }
}
