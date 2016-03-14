using Foosbot.Common.Protocols;
using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevDemos
{
    public class DemoLastBallCoordinatesUpdater : ILastBallCoordinatesUpdater
    {
        public BallCoordinates LastBallCoordinates { get; set; }
    }
}
