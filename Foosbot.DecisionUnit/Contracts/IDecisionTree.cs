// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Contracts
{
    /// <summary>
    /// Interface for Decision Class
    /// </summary>
    public interface IDecisionTree
    {
        /// <summary>
        /// Is Subtree Defined property
        /// </summary>
        bool IsSubtreeDefined { get; }

        /// <summary>
        /// SubTree Property
        /// </summary>
        IDecisionTree SubTree { get; }

        /// <summary>
        /// Responding Player index (1 based)
        /// </summary>
        int RespondingPlayer { get; }

        /// <summary>
        /// Main Decision Flow Method
        /// </summary>
        /// <param name="rod">Rod to use for decision</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <returns>Rod Action to perform</returns>
        RodAction Decide(IRod rod, BallCoordinates bfc);
    }
}
