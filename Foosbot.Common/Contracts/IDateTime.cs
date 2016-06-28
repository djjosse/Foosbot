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

namespace Foosbot.Common.Contracts
{
    /// <summary>
    /// Interface for Date Time
    /// </summary>
    public interface ITime
    {
        /// <summary>
        /// Current Date Time
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Start stopwatch
        /// </summary>
        void Start();

        /// <summary>
        /// Stop stopwatch
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets Elapsed Time from the stopwatch
        /// </summary>
        /// <returns>Time passed since started the stopwatch TimeSpan</returns>
        TimeSpan Elapsed { get; }
    }
}
