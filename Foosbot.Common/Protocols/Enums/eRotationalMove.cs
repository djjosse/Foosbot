using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Player rotational position/move.
    /// </summary>
    public enum eRotationalMove
    {
        /// <summary>
        /// Undefined player rotational position
        /// </summary>
        NA,

        /// <summary>
        /// Player is in 0 degrees (Legs back)
        /// </summary>
        RISE, 

        /// <summary>
        /// Player is in 90 degrees (Legs down)
        /// </summary>
        DEFENCE,

        /// <summary>
        /// Player is in 180 degrees (Legs ahead to the competitors gate)
        /// Also called Reverse-Rise
        /// </summary>
        KICK
    }
}
