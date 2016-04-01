// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Ball Relative Position to Current Rod's sector
    /// </summary>
    public enum eXPositionSectorRelative
    {
        /// <summary>
        /// Ball is Behind Current Sector
        /// </summary>
        BEHIND_SECTOR,

        /// <summary>
        /// Ball is In Current Sector
        /// </summary>
        IN_SECTOR,

        /// <summary>
        /// Ball is Ahead of Current Sector
        /// </summary>
        AHEAD_SECTOR
    }
}
