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
    /// Wrapper for exceptions occurred due to issues with camera calibration and needs retry
    /// </summary>
    public class CalibrationException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Reason of exception</param>
        public CalibrationException(string message) : base(message) {  }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Reason of exception</param>
        /// <param name="innerException">Inner exception responsible for this exception</param>
        public CalibrationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
