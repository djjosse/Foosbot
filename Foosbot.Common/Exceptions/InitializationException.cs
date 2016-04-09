using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
