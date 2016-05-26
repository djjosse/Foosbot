using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation
{
    /// <summary>
    /// Coordinates Stabilizer Class
    /// </summary>
    public class CoordinatesStabilizer
    {
        /// <summary>
        /// Number of frames to store last known coordinate if not found new one
        /// </summary>
        public const int MAX_UNDEFINED_THRESHOLD = 30;

        /// <summary>
        /// Ball Radius
        /// </summary>
        public int BallRadius { get; private set; }

        /// <summary>
        /// Current counter of not found coordinates
        /// </summary>
        private int _undefinedCoordinatesCounter;

        /// <summary>
        /// Last known coordinates
        /// </summary>
        private BallCoordinates _lastGoodCoordinates;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ballRadius">Ball Radius</param>
        public CoordinatesStabilizer(int ballRadius)
        {
            BallRadius = ballRadius;
            _undefinedCoordinatesCounter = 0;
            _lastGoodCoordinates = new BallCoordinates(DateTime.Now);
        }

        /// <summary>
        /// Stabilization method used to remove shaking
        /// </summary>
        /// <param name="newCoordinates">New coordinates</param>
        /// <param name="storedCoordinates">Last known stored coordinates</param>
        /// <returns>Approximate Coordinates without shaking</returns>
        public BallCoordinates Stabilize(BallCoordinates newCoordinates, BallCoordinates storedCoordinates)
        {
            if (newCoordinates.IsDefined)
            {
                _undefinedCoordinatesCounter = 0;
                _lastGoodCoordinates = newCoordinates;

                BallCoordinates coordinates = RemoveShaking(newCoordinates, storedCoordinates);
                return coordinates;
            }
            else //new coordinates are undefined
            {
                if (_undefinedCoordinatesCounter < MAX_UNDEFINED_THRESHOLD)
                {
                    _undefinedCoordinatesCounter++;
                    if (storedCoordinates.IsDefined)
                        _lastGoodCoordinates = storedCoordinates;
                }
                else
                {
                    _lastGoodCoordinates = newCoordinates;
                }
                return _lastGoodCoordinates;
            }
        }

        /// <summary>
        /// Remove Shaking method
        /// </summary>
        /// <param name="newCoordinates">New coordinates</param>
        /// <param name="lastKnownCoordinates">Last known coordinates</param>
        /// <returns>Approximate Coordinates without shaking</returns>
        private BallCoordinates RemoveShaking(BallCoordinates newCoordinates, BallCoordinates lastKnownCoordinates)
        {
            if (lastKnownCoordinates.IsDefined)
            {
                if (IsInRadiusRange(newCoordinates, lastKnownCoordinates))
                {
                    return new BallCoordinates(lastKnownCoordinates.X, lastKnownCoordinates.Y, newCoordinates.Timestamp);
                }
            }
            return newCoordinates;
        }

        /// <summary>
        /// Check if new coordinates are in ball radius with old coordinates
        /// </summary>
        /// <param name="coordNew">New coordinates</param>
        /// <param name="coordOld">Old coordinates</param>
        /// <returns>[True] if coordinates in ball radius, [False] otherwise</returns>
        private bool IsInRadiusRange(BallCoordinates coordNew, BallCoordinates coordOld)
        {
            return (coordNew.Distance(coordOld) < BallRadius);
        }
    }
}
