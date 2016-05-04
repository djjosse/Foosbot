// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using System;

namespace Foosbot.ImageProcessingUnit.Process.Contracts
{
    /// <summary>
    /// Interface for Definable Location with Time Stamp
    /// </summary>
    public interface ITimedLocation
    {
        /// <summary>
        /// Current X
        /// </summary>
        int X { get; }
        
        /// <summary>
        /// Current Y
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Current Time Stamp
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Is Defined property
        /// </summary>
        bool IsDefined { get; }

    }
}
