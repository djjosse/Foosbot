﻿// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;

namespace Foosbot.DecisionUnit.Contracts
{
    /// <summary>
    /// Rod State (DC and Servo positions)
    /// </summary>
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
