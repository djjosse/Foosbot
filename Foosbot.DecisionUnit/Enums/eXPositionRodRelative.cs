using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Ball position relative to current rod
    /// </summary>
    public enum eXPositionRodRelative
    {
        /// <summary>
        /// Ball is in Front of Current Rod
        /// </summary>
        FRONT,

        /// <summary>
        /// Ball and Rod X are Identical
        /// </summary>
        CENTER,

        /// <summary>
        /// Ball is Behind Current Rod
        /// </summary>
        BACK
    }
}
