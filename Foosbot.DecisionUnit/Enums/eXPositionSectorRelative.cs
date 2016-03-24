using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Ball Relative Position to Current Rod's sector
    /// </summary>
    public enum eXPositionSectorRelative
    {
        /// <summary>
        /// Ball is Behind Current Sector
        /// </summary>
        BEHIND_SECTOR,

        /// <summary>
        /// Ball is In Current Sector
        /// </summary>
        IN_SECTOR,

        /// <summary>
        /// Ball is Ahead of Current Sector
        /// </summary>
        AHEAD_SECTOR
    }
}
