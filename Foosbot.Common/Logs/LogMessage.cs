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
namespace Foosbot.Common.Logs
{
    /// <summary>
    /// Log Message
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Logging Messages used in logs
        /// </summary>
        /// <param name="message">Message description</param>
        /// <param name="category">Message Category</param>
        /// <param name="timeStamp">Message Time Stamp</param>
        public LogMessage(string message, eLogCategory category, DateTime timeStamp)
        {
            Description = message;
            _category = category;
            TimeStamp = timeStamp;
        }

        /// <summary>
        /// Time Stamp of message
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Message description
        /// </summary>
        public  string Description { get; private set; }

        /// <summary>
        /// Categoty private member
        /// </summary>
        private eLogCategory _category;

        /// <summary>
        /// Message Category as string in Upper Case
        /// </summary>
        public string CategoryAsString
        { 
            get
            {
                return _category.ToString().ToUpper();
            }
        }

        public eLogCategory Category
        {
            get
            {
                return _category;
            }
        }
    }
}
