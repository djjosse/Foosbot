using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    public enum eStatisticsKey
    {
        /// <summary>
        /// Current FPS, Width and Heigth
        /// </summary>
        FrameInfo = 1,

        /// <summary>
        /// Foosbot Memory and CPU Info
        /// </summary>
        ProccessInfo = 2,

        /// <summary>
        /// Percentage of successful ball detection and average detection time
        /// </summary>
        BasicImageProcessingInfo = 3,

        /// <summary>
        /// Ball Coordinates
        /// </summary>
        BallCoordinates = 4
    }
}
