using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Rod enumeration
    /// </summary>
    public enum eRod : int
    {
        /// <summary>
        /// Goal Keeper - First Rod
        /// </summary>
        GoalKeeper = 1,

        /// <summary>
        /// Defence - First Rod
        /// </summary>
        Defence = 2,

        /// <summary>
        /// Midfield - First Rod
        /// </summary>
        Midfield = 3,

        /// <summary>
        /// Attack - First Rod
        /// </summary>
        Attack = 4
    }
}
