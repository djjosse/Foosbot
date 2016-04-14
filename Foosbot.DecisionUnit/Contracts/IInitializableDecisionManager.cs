// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Contracts
{
    /// <summary>
    /// Initializable Action Provider Interface
    /// </summary>
    public interface IInitializableDecisionManager : IInitializable, IActionProvider
    {
        /// <summary>
        /// Initialize with parameters
        /// </summary>
        /// <param name="systemDelays">Mechanical, Calculation and Networking system delays in ms</param>
        void Initialize(int systemDelays);
    }
}
