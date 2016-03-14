using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Ball Location in real world before transformation performed
    /// </summary>
    public class Location : DefinableCartesianCoordinate<int>
    {
        /// <summary>
        /// Location constructor for undefined location
        /// </summary>
        /// <param name="timestamp">Frame timestamp</param>
        public Location(DateTime timestamp)
            : base()
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Location constructor for defined location
        /// </summary>
        /// <param name="x">Ball X location in frame</param>
        /// <param name="y">Ball Y location in frame</param>
        /// <param name="timestamp">Frame timestamp</param>
        public Location(int x, int y, DateTime timestamp)
            :base(x, y)
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Location constructor for defined location
        /// </summary>
        /// <param name="x">Ball X location in frame (will be cut to integer)</param>
        /// <param name="y">Ball Y location in frame (will be cut to integer)</param>
        /// <param name="timestamp"></param>
        public Location(double x, double y, DateTime timestamp)
            : base(Convert.ToInt32(x), Convert.ToInt32(y))
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Ball Location in Frame Timestamp Get Property
        /// </summary>
        public DateTime Timestamp { get; private set; }
    }
}
