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

namespace Foosbot.Common.Exceptions
{
    /// <summary>
    /// Exception is thrown in case throwing object was not initialized
    /// </summary>
    public class InitializationException : Exception
    {
        /// <summary>
        /// Constructor for initialization exception
        /// </summary>
        /// <param name="message">Reason for this exception</param>
        public InitializationException(string message) : base(message)  {   }
    }
}
