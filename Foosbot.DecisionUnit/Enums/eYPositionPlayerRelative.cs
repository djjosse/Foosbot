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
    /// Ball Y Relative Position to Player in Current Rod
    /// </summary>
    public enum eYPositionPlayerRelative
    {
        /// <summary>
        /// Y ball coordinate less than Y Player coordinate
        /// (needs negative movement)
        /// </summary>
        LEFT,

        /// <summary>
        /// Y ball coordinate is equal to Y Player coordinate
        /// (no need to move)
        /// </summary>
        CENTER,

        /// <summary>
        /// Y ball coordinate greater than Y Player coordinate
        /// (needs positive movement)
        /// </summary>
        RIGHT
    }
}
