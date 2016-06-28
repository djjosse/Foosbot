// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Publisher Class for Rod Action
    /// </summary>
    public class RodActionPublisher : Publisher<RodAction>
    {
        /// <summary>
        /// Update and notify method
        /// </summary>
        /// <param name="action">Latest desired action</param>
        public void UpdateAndNotify(RodAction action)
        {
            Data = action;
            NotifyAll();
        }
    }
}
