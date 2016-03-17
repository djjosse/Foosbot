using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    public class RodAction
    {
        public eRod Type { get; private set; }

        public eRotationalMove Rotation { get; private set; }

        public eLinearMove Linear { get; private set; }

        public RodAction(eRod type, eRotationalMove rotation, eLinearMove linear)
        {
            Type = type;
            Rotation = rotation;
            Linear = linear;
        }


    }
}
