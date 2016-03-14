using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Callibration State Enum
    /// </summary>
    public enum eCallibrationState : int
    {
        /// <summary>
        /// Callibration current state - callibration not started
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Callibration current state - callibration phase I finished
        /// </summary>
        FinishedPhaseI = 1,

        /// <summary>
        /// Callibration current state - callibration phase II finished
        /// </summary>
        Finished = 2
    }
}
