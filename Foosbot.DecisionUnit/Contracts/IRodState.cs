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
    public interface IRodState
    {
        /// <summary>
        /// Assumed for last known DC position (in mm)
        /// </summary>
        int DcPosition { get; set; }

        /// <summary>
        /// Assumed for last known Servo position
        /// </summary>
        eRotationalMove ServoPosition { get; set; }
    }
}
