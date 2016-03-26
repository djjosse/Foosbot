using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Rod Action to be performed on current rod
    /// </summary>
    public class RodAction
    {
        /// <summary>
        /// Actual linear movement private member
        /// </summary>
        private int _movement;

        /// <summary>
        /// Current Rod type for action to perform
        /// </summary>
        public eRod Type { get; private set; }

        /// <summary>
        /// Rotation movement type to be performed
        /// </summary>
        public eRotationalMove Rotation { get; private set; }

        /// <summary>
        /// Linear movement type to be performed
        /// </summary>
        public eLinearMove Linear { get; private set; }

        /// <summary>
        /// Linear Movement to be performed
        /// </summary>
        public int LinearMovement
        {
            get
            {
                if (Linear == eLinearMove.NA)
                    return 0;
                else return _movement;
            }
            set
            {
                _movement = value;
            }
        }

        /// <summary>
        /// Constructor for rod action to be performed
        /// </summary>
        /// <param name="type">Current Rod Type</param>
        /// <param name="rotation">Rotational move to be performed (default is undefined)</param>
        /// <param name="linear">Linear move to be performed (default is undefined)</param>
        public RodAction(eRod type, eRotationalMove rotation = eRotationalMove.NA, eLinearMove linear = eLinearMove.NA)
        {
            _movement = 0;
            Type = type;
            Rotation = rotation;
            Linear = linear;
        }
    }
}
