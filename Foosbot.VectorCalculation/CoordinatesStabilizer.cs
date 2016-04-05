using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation
{
    public class CoordinatesStabilizer
    {
        public const int MAX_UNDEFINED_THRESHOLD = 30;

        public int BallRadius { get; private set; }

        private int _undefinedCoordinatesCounter;
        private BallCoordinates _lastGoodCoordinates;

        public CoordinatesStabilizer(int ballRadius)
        {
            BallRadius = ballRadius;
            _undefinedCoordinatesCounter = 0;
            _lastGoodCoordinates = new BallCoordinates(DateTime.Now);
        }

        public BallCoordinates Stabilize(BallCoordinates newCoordinates, BallCoordinates storedCoordinates)
        {
            if (newCoordinates.IsDefined)
            {
                _undefinedCoordinatesCounter = 0;
                _lastGoodCoordinates = newCoordinates;

                BallCoordinates coordinates = RemoveShaking(newCoordinates, storedCoordinates);
               // coordinates = RemoveInfinitySpeed(coordinates);
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
        private BallCoordinates RemoveInfinitySpeed(BallCoordinates newCoordinates)
        {
            return newCoordinates;
        }

        private bool IsInRadiusRange(BallCoordinates coordNew, BallCoordinates coordOld)
        {
            return (coordNew.Distance(coordOld) < BallRadius);
        }
    }
}
