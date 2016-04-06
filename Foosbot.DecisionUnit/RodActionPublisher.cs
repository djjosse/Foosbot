using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    public class RodActionPublisher : Publisher<RodAction>
    {

        public void UpdateAndNotify(RodAction action)
        {
            Data = action;
            NotifyAll();
        }
    }
}
