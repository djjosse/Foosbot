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
    /// Calibration State Enum
    /// </summary>
    public enum eCalibrationState : int
    {
        /// <summary>
        /// Calibration current state - calibration not started
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Calibration current state - calibration phase I finished
        /// </summary>
        FinishedPhaseI = 1,

        /// <summary>
        /// Calibration current state - calibration phase II finished
        /// </summary>
        Finished = 2
    }
}
