// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.ImageProcessingUnit.Tools.Enums
{
    /// <summary>
    /// Calibration State for calibration tool
    /// </summary>
    public enum eCalibrationState
    {
        /// <summary>
        /// Calibration Not Started
        /// </summary>
        NotStarted,

        /// <summary>
        /// Calibration First Phase Finished
        /// </summary>
        Performing,

        /// <summary>
        /// Calibration Finished at all
        /// </summary>
        Finished
    }
}
