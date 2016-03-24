using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Ball Y Relative Position to Player in Current Rod
    /// </summary>
    public enum eYPositionPlayerRelative
    {
        /// <summary>
        /// Y ball coordinate less than Y Player coordinate
        /// (needs negative movement)
        /// </summary>
        LEFT,

        /// <summary>
        /// Y ball coordinate is equal to Y Player coordinate
        /// (no need to move)
        /// </summary>
        CENTER,

        /// <summary>
        /// Y ball coordinate greater than Y Player coordinate
        /// (needs positive movement)
        /// </summary>
        RIGHT
    }
}
