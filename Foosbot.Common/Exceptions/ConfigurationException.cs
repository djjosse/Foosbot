using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot
{
    /// <summary>
    /// Exception Thrown in case configuration error appeared
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public ConfigurationException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner Exception</param>
        public ConfigurationException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
