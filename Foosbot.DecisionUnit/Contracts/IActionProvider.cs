// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Contracts
{
    /// <summary>
    /// Implementing Class will represent decision manager which decides on actions to be taken
    /// </summary>
    public interface IActionProvider
    {
        /// <summary>
        /// Decide on action to be taken for each rod
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates and vector</param>
        /// <returns>List of actions per each rod</returns>
        List<RodAction> Decide(BallCoordinates currentCoordinates);

        void UpdateRealTimeState(eRod rodType, eRotationalMove servoState);
    }
}
