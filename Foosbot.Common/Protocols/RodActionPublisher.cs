using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Publisher Class for Rod Action
    /// </summary>
    public class RodActionPublisher : Publisher<RodAction>
    {
        /// <summary>
        /// Update and notify method
        /// </summary>
        /// <param name="action">Latest desired action</param>
        public void UpdateAndNotify(RodAction action)
        {
            Data = action;
            NotifyAll();
        }
    }
}
