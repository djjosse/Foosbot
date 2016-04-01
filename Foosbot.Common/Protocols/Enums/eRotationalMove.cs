// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Player rotational position/move.
    /// </summary>
    public enum eRotationalMove
    {
        /// <summary>
        /// Undefined player rotational position
        /// </summary>
        NA,

        /// <summary>
        /// Player is in 0 degrees (Legs back)
        /// </summary>
        RISE, 

        /// <summary>
        /// Player is in 90 degrees (Legs down)
        /// </summary>
        DEFENCE,

        /// <summary>
        /// Player is in 180 degrees (Legs ahead to the competitors gate)
        /// Also called Reverse-Rise
        /// </summary>
        KICK
    }
}
