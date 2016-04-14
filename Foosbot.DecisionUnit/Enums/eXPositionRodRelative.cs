// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.DecisionUnit.Enums
{
    /// <summary>
    /// Ball position relative to current rod
    /// </summary>
    public enum eXPositionRodRelative
    {
        /// <summary>
        /// Ball is in Front of Current Rod
        /// </summary>
        FRONT,

        /// <summary>
        /// Ball and Rod X are Identical
        /// </summary>
        CENTER,

        /// <summary>
        /// Ball is Behind Current Rod
        /// </summary>
        BACK
    }
}
