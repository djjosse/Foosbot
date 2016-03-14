using Foosbot.Common.Protocols;
using System;
namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Last Ball Coordinates Interface for implementing interface
    /// </summary>
    public interface ILastBallCoordinatesUpdater
    {
        /// <summary>
        /// Last stored ball coordinates
        /// </summary>
        BallCoordinates LastBallCoordinates { get; }
    }
}
