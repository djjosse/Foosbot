// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot
{
    /// <summary>
    /// Represents a flow that can run in Separate Thread
    /// </summary>
    public interface IFlow
    {
        /// <summary>
        /// Function that will run in Separate Thread
        /// </summary>
        void Flow();

        /// <summary>
        /// Run the flow in Thread
        /// </summary>
        void Start();
    }
}
