using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
