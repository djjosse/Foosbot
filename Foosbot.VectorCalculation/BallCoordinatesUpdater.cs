using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation
{
    class BallCoordinatesUpdater : ILastBallCoordinatesUpdater
    {
        public BallCoordinates LastBallCoordinates { get; set; }
    }
}
