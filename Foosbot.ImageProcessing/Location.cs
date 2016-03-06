using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    public class Location : DefinableCartesianCoordinate<int>
    {
        public Location(DateTime timestamp)
            : base()
        {
            Timestamp = timestamp;
        }

        public Location(int x, int y, DateTime timestamp)
            :base(x, y)
        {
            Timestamp = timestamp;
        }

        public Location(double x, double y, DateTime timestamp)
            : base(Convert.ToInt32(x), Convert.ToInt32(y))
        {
            Timestamp = timestamp;
        }
        public DateTime Timestamp { get; private set; }
    }
}
