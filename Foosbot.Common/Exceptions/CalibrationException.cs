using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
