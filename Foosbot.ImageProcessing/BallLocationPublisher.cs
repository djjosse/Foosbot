using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Pubishes Latest Ball Coordinates and notifies observers on demand
    /// </summary>
    public class BallLocationPublisher: Publisher<BallCoordinates>
    {
        /// <summary>
        /// To get latest
        /// </summary>
        private ILastBallCoordinatesUpdater _coordinatesUpdater;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinatesUpdater">Updater to get coordinates from</param>
        public BallLocationPublisher(ILastBallCoordinatesUpdater coordinatesUpdater)
        {
            _coordinatesUpdater = coordinatesUpdater;
        }

        /// <summary>
        /// Gets the latest coordinates from coordinates updater passed in constructor
        /// and notifies all attched observers
        /// </summary>
        public void UpdateAndNotify()
        {
            Data = _coordinatesUpdater.LastBallCoordinates;
            NotifyAll();
        }
    }
}
