using Foosbot.Common.Protocols;
using System;
namespace Foosbot.ImageProcessing
{
    public interface ILastBallCoordinatesUpdater
    {
        BallCoordinates LastBallCoordinates { get; }
    }
}
