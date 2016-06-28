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
    /// Linear move type for rod
    /// </summary>
    public enum eLinearMove
    {
        /// <summary>
        /// Linear move undefined
        /// </summary>
        NA,

        /// <summary>
        /// Best Effort state for the rod (Center/Best rod position)
        /// </summary>
        BEST_EFFORT,

        /// <summary>
        /// Rod movement should be set based on vector intersection with current rod
        /// </summary>
        VECTOR_BASED,

        /// <summary>
        /// Rod movement should be set to have a player in front of ball Y coordinate
        /// </summary>
        BALL_Y,

        /// <summary>
        /// Move ball diameter left
        /// </summary>
        LEFT_BALL_DIAMETER,

        /// <summary>
        /// Move ball diameter right
        /// </summary>
        RIGHT_BALL_DIAMETER
    }
}
