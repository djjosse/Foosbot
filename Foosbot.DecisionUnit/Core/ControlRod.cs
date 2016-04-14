//// **************************************************************************************
//// **																				   **
//// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
//// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
//// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
//// **		The information and source code here belongs to Foosbot project			   **
//// **		and may not be reproduced or used without authors explicit permission.	   **
//// **																				   **
//// **************************************************************************************

using Foosbot.Common.Exceptions;
using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using Foosbot.VectorCalculation;
using System;
using System.Reflection;
namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Rod represents rod in foosbot
    /// </summary>
    public class ControlRod : IInitializableRod
    {
        private readonly int TABLE_HEIGHT;
        private readonly int ROD_STOPPER_MIN;

        /// <summary>
        /// Used in for range check vector intersection calculation
        /// </summary>
        private ISurveyor _surveyor;

        /// <summary>
        /// Used for ricochet in vector intersection calculation
        /// </summary>
        private RicochetCalc _vectorUtils;

        #region IRod private members

        /// <summary>
        /// Rod type private readonly member
        /// </summary>
        private eRod _rodType;

        /// <summary>
        /// Distance between each 2 player on current rod
        /// </summary>
        private int _playerDistance;

        /// <summary>
        /// Number of players in current rod
        /// </summary>
        private int _playerCount;

        /// <summary>
        /// Distance from table border (Y min) to head of first player
        /// </summary>
        private int _offsetY;

        /// <summary>
        /// Distance between stoppers of current rod
        /// </summary>
        private int _stopperDistance;

        /// <summary>
        /// Rod X coordinate in Foosbot world private readonly member
        /// </summary>
        private int _rodXCoordinate;

        /// <summary>
        /// Minimal Sector Width in Foosbot world private readonly member
        /// </summary>
        private int _minSectorWidth;

        /// <summary>
        /// Sector Factor used to calculate dynamic sector private readonly member
        /// </summary>
        private double _sectorFactor;

        /// <summary>
        /// Best Effort First Player Y Coordinate 
        /// </summary>
        private int _bestEffort;

        #endregion IRod private members

        #region IInitializableRod properties

        /// <summary>
        /// Rod type private readonly member
        /// </summary>
        public eRod RodType
        {
            get
            {
                if (IsInitialized)
                    return _rodType;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Distance between each 2 player on current rod
        /// </summary>
        public int PlayerDistance
        {
            get
            {
                if (IsInitialized)
                    return _playerDistance;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Number of players in current rod
        /// </summary>
        public int PlayerCount
        {
            get
            {
                if (IsInitialized)
                    return _playerCount;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Distance from table border (Y min) to head of first player
        /// </summary>
        public int OffsetY
        {
            get
            {
                if (IsInitialized)
                    return _offsetY;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Distance between stoppers of current rod
        /// </summary>
        public int StopperDistance
        {
            get
            {
                if (IsInitialized)
                    return _stopperDistance;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Rod X coordinate in Foosbot world private readonly member
        /// </summary>
        public int RodXCoordinate
        {
            get
            {
                if (IsInitialized)
                    return _rodXCoordinate;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Minimal Sector Width in Foosbot world private readonly member
        /// </summary>
        public int MinSectorWidth
        {
            get
            {
                if (IsInitialized)
                    return _minSectorWidth;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Sector Factor used to calculate dynamic sector private readonly member
        /// </summary>
        public double SectorFactor
        {
            get
            {
                if (IsInitialized)
                    return _sectorFactor;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Best Effort First Player Y Coordinate 
        /// </summary>
        public int BestEffort 
        {
            get
            {
                if (IsInitialized)
                    return _bestEffort;
                else
                    throw new InitializationException(String.Format(
                        "Class [{0}] must be initialized before it can be used.", GetType().Name));
            }
        }

        /// <summary>
        /// Minimum possible start stopper Y coordinate of rod
        /// </summary>
        public int MinimumPossibleStartStopperY { get; private set; }

        /// <summary>
        /// Maximum possible start stopper Y coordinate of rod
        /// </summary>
        public int MaximumPossibleStartStopperY { get; private set; }

        /// <summary>
        /// Class initialization property
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Intersection point and time with latest ball vector
        /// </summary>
        public TimedPoint Intersection { get; private set; }

        /// <summary>
        /// Dynamic Sector Get Property
        /// </summary>
        public int DynamicSector { get; private set; }

        /// <summary>
        /// Last Known State of Rod
        /// </summary>
        public IRodState State { get; private set; }

        #endregion IInitializableRod properties

        /// <summary>
        /// Control Rod Constructor
        /// </summary>
        /// <param name="rodType">Rod Type to construct</param>
        /// <param name="surveyor">Used in for range check vector intersection calculation</param>
        /// <param name="vectorUtils">Used for ricochet in vector intersection calculation</param>
        /// <param name="tableHeight">Foosbot table height - rod length (Y) in mm</param>
        /// <param name="tableHeight">Minimal point can be reached by stopper (Y) in mm</param>
        /// <param name="rodState">Rod state will be created in initialization method if passed null</param>
        public ControlRod(eRod rodType, ISurveyor surveyor, RicochetCalc vectorUtils, 
            int tableHeight = -1, int rodStopperMin = -1, IRodState rodState = null)
        {
            _rodType = rodType;
            _surveyor = surveyor;
            _vectorUtils = vectorUtils;
            State = rodState;

            TABLE_HEIGHT = (tableHeight > 0) ? tableHeight
                : Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);
            ROD_STOPPER_MIN = (rodStopperMin > 0) ? rodStopperMin
                : Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_START_Y);
        }

        /// <summary>
        /// Initialization with given parameters
        /// </summary>
        /// <param name="rodXCoordinate">X Coordinate of rod (in MM)</param>
        /// <param name="minSectorWidth">Minimal sector width (in MM)</param>
        /// <param name="sectorFactor">Sector factor to change width accoroding to speed</param>
        /// <param name="playerDistance">Distance beetween 2 player on rod (in MM)</param>
        /// <param name="playerCount">Player count on current rod (in MM)</param>
        /// <param name="offsetY">Distance between stopper and first player on rod (in MM)</param>
        /// <param name="stopperDistance">Distance between start and end stoppers of current rod (in MM)</param>
        /// <param name="bestEffort">Coordinate (in MM) for first player to be on in BEST_EFFORT state</param>
        public void Initialize(int rodXCoordinate, int minSectorWidth, double sectorFactor,
            int playerDistance, int playerCount, int offsetY, int stopperDistance, int bestEffort)
        {
            if (!IsInitialized)
            {
                _rodXCoordinate = rodXCoordinate;
                _minSectorWidth = minSectorWidth;
                _sectorFactor = sectorFactor;
                _playerDistance = playerDistance;
                _playerCount = playerCount;
                _offsetY = offsetY;
                _stopperDistance = stopperDistance;
                _bestEffort = bestEffort;

                DynamicSector = _minSectorWidth;
                MinimumPossibleStartStopperY = ROD_STOPPER_MIN;
                MaximumPossibleStartStopperY = TABLE_HEIGHT - ROD_STOPPER_MIN - _stopperDistance;

                if (State == null)
                {
                    State = new RodState(MinimumPossibleStartStopperY, MaximumPossibleStartStopperY);
                }

                IsInitialized = true;
            }
        }

        /// <summary>
        /// Initialization Using Configuration file
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                _rodXCoordinate = Configuration.Attributes.GetValue<int>(_rodType.ToString());
                _minSectorWidth = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_RODS_DIST);
                _sectorFactor = Configuration.Attributes.GetValue<double>(Configuration.Names.SECTOR_FACTOR);
                _playerDistance = Configuration.Attributes.GetPlayersDistancePerRod(_rodType);
                _playerCount = Configuration.Attributes.GetPlayersCountPerRod(_rodType);
                _offsetY = Configuration.Attributes.GetPlayersOffsetYPerRod(_rodType);
                _stopperDistance = Configuration.Attributes.GetRodDistanceBetweenStoppers(_rodType);
                _bestEffort = Configuration.Attributes.GetFirstPlayerBestEffort(_rodType);

                MinimumPossibleStartStopperY = ROD_STOPPER_MIN;
                MaximumPossibleStartStopperY = TABLE_HEIGHT - ROD_STOPPER_MIN - _stopperDistance;

                if (State == null)
                {
                    State = new RodState(MinimumPossibleStartStopperY, MaximumPossibleStartStopperY);
                }

                DynamicSector = _minSectorWidth;
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Dynamic sector calculation method - sets DynamicSector property value.
        /// * if coordinates and vector are defined AND vector is to the rod THEN sets dynamic sector due to ball velocity
        /// * else sets minimal sector width
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates and vector</param>
        /// <returns>Dynamic Sector Width</returns>
        public int CalculateDynamicSector(BallCoordinates currentCoordinates)
        {
            if (currentCoordinates != null && currentCoordinates.Vector != null 
                    && currentCoordinates.IsDefined && currentCoordinates.Vector.IsDefined
                        && currentCoordinates.X > _rodXCoordinate)
                DynamicSector = Convert.ToInt32(_minSectorWidth + Math.Abs(currentCoordinates.Vector.X) * _sectorFactor);
            else
                DynamicSector = _minSectorWidth;
            return DynamicSector;
        }

        /// <summary>
        /// Calculate Rod Intersection with current rod
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates to calculate intersection</param>
        public void CalculateSectorIntersection(BallCoordinates currentCoordinates)
        {
            try
            {
                //if unable to calculate or no intersection
                if (currentCoordinates == null || !currentCoordinates.IsDefined
                    || currentCoordinates.Vector == null || !currentCoordinates.Vector.IsDefined
                        || currentCoordinates.Vector.X == 0)
                {
                    Intersection = new TimedPoint();
                    return;
                }

                int xintersection = RodXCoordinate;
                /* //Define if we want to use dynamic sectors
                   (RodXCoordinate > currentCoordinates.X) ?
                     Convert.ToInt32(RodXCoordinate - DynamicSector / 2.0) :
                     Convert.ToInt32(RodXCoordinate + DynamicSector / 2.0);
                */

                double inSec = (xintersection - currentCoordinates.X) / currentCoordinates.Vector.X;
                //if timespan is bigger than 0 && Vector X is also bigger than 0
                if (inSec >= 0 && Math.Abs(currentCoordinates.Vector.X) > 0)
                {
                    TimeSpan intersectionTime = TimeSpan.FromSeconds(inSec);

                    int yintersection = Convert.ToInt32(currentCoordinates.Vector.Y * intersectionTime.TotalSeconds + currentCoordinates.Y);
                    if (_surveyor.IsCoordinatesYInRange(yintersection))
                    {
                        Intersection = new TimedPoint(xintersection, yintersection, currentCoordinates.Timestamp + intersectionTime);
                        Log.Common.Debug(String.Format("[{0}] Found intersection point with rod [{1}]: [{2}x{3}]",
                            MethodBase.GetCurrentMethod().Name, _rodType, xintersection, yintersection));
                    }
                    else
                    {
                        BallCoordinates ricoshetCoordiantes = _vectorUtils.Ricochet(currentCoordinates);
                        CalculateSectorIntersection(ricoshetCoordiantes);
                    }
                }
                else
                {
                    Intersection = new TimedPoint();
                }
            }
            catch(Exception e)
            {
                Intersection = new TimedPoint();
                Log.Common.Error(String.Format("[{0}] Unable to calculate rod intersection. Reason: {1}",
                    MethodBase.GetCurrentMethod().Name, e.Message));
            }
        }

        /// <summary>
        /// Get nearest possible DC position in case desired position is out of range
        /// </summary>
        /// <param name="desiredPosition">Originlly desired position</param>
        /// <returns>Nearest posible position</returns>
        public int NearestPossibleDcPosition(int desiredPosition)
        {
            if (desiredPosition < MinimumPossibleStartStopperY)
                return MinimumPossibleStartStopperY;
            if (desiredPosition > MaximumPossibleStartStopperY)
                return MaximumPossibleStartStopperY;
            return desiredPosition;
        }
    }
}
