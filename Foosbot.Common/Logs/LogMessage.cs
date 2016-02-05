using System;
namespace Foosbot.Common.Logs
{
    /// <summary>
    /// Log Message
    /// </summary>
    /// <author>Joseph Gleyzer</author>
    /// <date>04.02.2016</date>
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
        public string Category
        { 
            get
            {
                return _category.ToString().ToUpper();
            }
        }
    }
}
