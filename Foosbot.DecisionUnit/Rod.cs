using Foosbot;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Rod represents rod in foosbot
    /// Derived class from DefinableCartesianCoordinate 
    /// which in this case represent an intersection of ball vector with current rod.
    /// </summary>
    public class Rod : DefinableCartesianCoordinate<int>
    {
        #region Private Members

        /// <summary>
        /// Rod type private readonly member
        /// </summary>
        private readonly eRod _rodType;

        /// <summary>
        /// Distance between each 2 player on current rod
        /// </summary>
        private readonly int _playerDistance;

        /// <summary>
        /// Number of players in current rod
        /// </summary>
        private readonly int _playersCount;

        /// <summary>
        /// Distance from table border (Y min) to head of first player
        /// </summary>
        private readonly int _offsetY;

        /// <summary>
        /// Distance between stoppers of current rod
        /// </summary>
        private readonly int _stopperDistance;

        /// <summary>
        /// Rod X coordinate in Foosbot world private readonly member
        /// </summary>
        private readonly int _rodXCoordinate;

        /// <summary>
        /// Minimal Sector Width in Foosbot world private readonly member
        /// </summary>
        private readonly int _minSectorWidth;

        /// <summary>
        /// Sector Factor used to calculate dynamic sector private readonly member
        /// </summary>
        private readonly double _sectorFactor;

        /// <summary>
        /// Sector Intersection Time private member
        /// </summary>
        private DateTime _sectorIntersectionTime;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Rod Type get property
        /// </summary>
        public eRod RodType
        {   
            get
            {
                return _rodType;
            }
        }

        /// <summary>
        /// Rod X coordinate in Foosbot world get property
        /// </summary>
        public int RodXCoordinate
        {
            get
            {
                return _rodXCoordinate;
            }
        }

        /// <summary>
        /// Distance between each 2 player on current rod
        /// </summary>
        public int PlayerDistance
        {
            get
            {
                return _playerDistance;
            }
        }

        /// <summary>
        /// Number of players in current rod
        /// </summary>
        public int PlayersCount
        {
            get
            {
                return _playersCount;
            }
        }

        /// <summary>
        /// Distance from table border (Y min) to head of first player
        /// </summary>
        public int OffsetY
        {
            get
            {
                return _offsetY;
            }
        }

        /// <summary>
        /// Distance between stoppers of current rod
        /// </summary>
        public int StopperDistance
        {
            get
            {
                return _stopperDistance;
            }
        }

        /// <summary>
        /// Sector Intersection Time get property
        /// </summary>
        public DateTime SectorIntersectionTime
        {
            get
            {
                if (IsDefined)
                    return _sectorIntersectionTime;
                else
                    throw new Exception("Intersection time is undefined, no value stored in RodIntersectionTime");
            }
        }

        /// <summary>
        /// Dynamic Sector Get Property
        /// </summary>
        public int DynamicSector { get; private set; }

        #endregion Properties

        /// <summary>
        /// Constructor for Rod
        /// </summary>
        /// <param name="type">Rod type - based on this type rod location will be calculated</param>
        public Rod(eRod type)
        {
            //Set Rod Type
            _rodType = type;

            //Calculate Rod Location
            int actualRodXCoordinate = Configuration.Attributes.GetValue<int>(_rodType.ToString());
            int tableWidth = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            int foosbotWidth = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            _rodXCoordinate = actualRodXCoordinate * foosbotWidth / tableWidth;

            //Calculate Rod Minimal Sector
            int actualRodsDist = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_RODS_DIST);
            _minSectorWidth = actualRodsDist * foosbotWidth / tableWidth;

            //Set sector factor to calculate dynamic sector based ball velocity
            _sectorFactor = Configuration.Attributes.GetValue<double>(Configuration.Names.SECTOR_FACTOR);


            _playerDistance = Configuration.Attributes.GetPlayersDistancePerRod(_rodType);
            _playersCount = Configuration.Attributes.GetPlayersCountPerRod(_rodType);
            _offsetY = Configuration.Attributes.GetPlayersOffsetYPerRod(_rodType);
            _stopperDistance = Configuration.Attributes.GetRodDistanceBetweenStoppers(_rodType);
        }

        /// <summary>
        /// Dynamic sector callculation method
        /// </summary>
        /// <param name="ballXcoordinate">Ball X Coordinate</param>
        /// <param name="ballXvector">Ball X Vector Coordinate</param>
        /// <returns>Dynamic Sector Width</returns>
        public void CalculateDynamicSector(int ballXcoordinate, double ballXvector)
        {
            if (ballXcoordinate > _rodXCoordinate)
                DynamicSector = Convert.ToInt32(_minSectorWidth + Math.Abs(ballXvector) * _sectorFactor);
            else
                DynamicSector = _minSectorWidth;
        }

        /// <summary>
        /// Used to set Ball Intersection with current rod
        /// </summary>
        /// <param name="x">Intersection X (Must be calculated due to dynamic sector width)</param>
        /// <param name="y">Intersection Y</param>
        /// <param name="timestamp">Intersection Timestamp</param>
        public void SetBallIntersection(int x, int y, DateTime timestamp)
        {
            _x = x;
            _y = y;
            _sectorIntersectionTime = timestamp;
            IsDefined = true;
        }

        /// <summary>
        /// Set Ball Intersection to Undefined state
        /// </summary>
        public void SetBallIntersection()
        {
            IsDefined = false;
        }

    }
}
