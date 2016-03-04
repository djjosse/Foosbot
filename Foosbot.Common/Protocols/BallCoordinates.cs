using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Image Processing and Vector Calculation Units common output class
    /// </summary>
    public class BallCoordinates : DefinableCartesianCoordinate<int>
    {
        /// <summary>
        /// Constructor for defined coordinates
        /// </summary>
        /// <param name="x">X ball coordinate</param>
        /// <param name="y">Y ball coordinate</param>
        /// <param name="timestamp"></param>
        public BallCoordinates(int x, int y, DateTime timestamp) : base(x, y)
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Constructor for undefined coordinates
        /// </summary>
        /// <param name="timestamp">Timestamp of current coordinates</param>
        public BallCoordinates(DateTime timestamp):base()
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Timestamp of current coordinates
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Vector of current ball location - TBD in VectorCalculationUnit
        /// </summary>
        public Vector2D Vector { get; set; }
    }
}
