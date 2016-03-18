using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Decision Unit Main Class
    /// </summary>
    public class Decision
    {
        #region Constants

        /// <summary>
        /// Mechanical, Calculation and other delays
        /// </summary>
        private readonly TimeSpan DELAYS;

        /// <summary>
        /// Table X lenght (width) in Foosbot world
        /// </summary>
        private readonly int XMAX;

        /// <summary>
        /// Table Y lenght (height) in Foosbot world
        /// </summary>
        private readonly int YMAX;

        #endregion Constants

        #region private members

        /// <summary>
        /// Decision Tree instance
        /// </summary>
        private DecisionTree _decisionTree; 

        /// <summary>
        /// Ball Future Coordinates calculated
        /// Coordinates in time system can actually respond
        /// </summary>
        private BallCoordinates _bfc;

        /// <summary>
        /// All existing rods
        /// </summary>
        private Dictionary<eRod, Rod> _rods;

        #endregion private members

        /// <summary>
        /// Decision Unit Constructor
        /// </summary>
        public Decision()
        {
            //Create all rods
            _rods = new Dictionary<eRod, Rod>();
            foreach (eRod type in Enum.GetValues(typeof(eRod)))
                _rods.Add(type, new Rod(type));

            //Create decision tree instance
            _decisionTree = new DecisionTree();

            //Set constants from configuration file
            DELAYS = TimeSpan.FromMilliseconds(Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_DELAY));
            XMAX = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            YMAX = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
        }

        /// <summary>
        /// Main Decision Flow
        /// </summary>
        /// <param name="currentCoordinates"></param>
        public void Flow(BallCoordinates currentCoordinates)
        {
            //Calculate Actual Possible Action Time
            DateTime timeOfAction = DateTime.Now + DELAYS;

            //Calculate ball future coordinates
            FindBallFutureCoordinates(currentCoordinates, timeOfAction);

            //Calculate dynamic sectors
            if (currentCoordinates.IsDefined && currentCoordinates.Vector.IsDefined)
                CalculateDynamicSectors(currentCoordinates.X, currentCoordinates.Vector.X);
            else            
                throw new NotSupportedException("Currently not defined coordinates are not supported by Pre-Decision Flow");

            //Calculate Rod Intersection with a ball for all rods
            CalculateSectorIntersection(currentCoordinates);

            //Take decision for each rod
            foreach (Rod rod in _rods.Values)
                _decisionTree.Decide(rod, _bfc);
        }

        #region Private Member Functions

        /// <summary>
        /// Calculate Ball Future Coordinates in actual time system can responce
        /// </summary>
        /// <param name="currentCoordinates"><Current ball coordinates/param>
        /// <param name="actionTime">Actual system responce time</param>
        private void FindBallFutureCoordinates(BallCoordinates currentCoordinates, DateTime actionTime)
        {
            TimeSpan deltaT = actionTime - currentCoordinates.Timestamp;

            if (currentCoordinates.IsDefined && currentCoordinates.Vector.IsDefined)
            {
                int xfc = Convert.ToInt32(currentCoordinates.Vector.X * deltaT.TotalMilliseconds + currentCoordinates.X);
                int yfc = Convert.ToInt32(currentCoordinates.Vector.Y * deltaT.TotalMilliseconds + currentCoordinates.Y);
                
                if (IsCoordinatesInRange(xfc, yfc))
                {
                    _bfc = new BallCoordinates(xfc, yfc, actionTime);
                }
                else
                {
                    //ToDo: Call ricoshet algorithm with:
                    BallCoordinates ricoshetCoordiantes = null;
                    //ricoshetCoordiantes = RicoshetAlgorithm(currentCoordinates);
                    FindBallFutureCoordinates(ricoshetCoordiantes, actionTime);
                }
            }
            else
            {
                throw new NotSupportedException("Currently not defined coordinates are not supported by Pre-Decision Flow");
            }
        }

        /// <summary>
        /// Calcullates Dynamic Sectors per each rod
        /// </summary>
        /// <param name="ballXcoordinate">Current Ball X Coordinate</param>
        /// <param name="ballXvector">Current Ball Vector X Coordinate</param>
        private void CalculateDynamicSectors(int ballXcoordinate, double ballXvector)
        {
            foreach (Rod rod in _rods.Values)
                rod.CalculateDynamicSector(ballXcoordinate, ballXvector);
        }

        /// <summary>
        /// Callculate Rod Intersection for all rods
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates to calculate intersection</param>
        private void CalculateSectorIntersection(BallCoordinates currentCoordinates)
        {
            foreach (Rod rod in _rods.Values)
                CalculateSectorIntersection(rod, currentCoordinates);
        }

        /// <summary>
        /// Callculate Rod Intersection with current rod
        /// </summary>
        /// <param name="rod">Current rod to calculate intersection with</param>
        /// <param name="currentCoordinates">Current ball coordinates to calculate intersection</param>
        private void CalculateSectorIntersection(Rod rod, BallCoordinates currentCoordinates)
        {
            int xintersection = (rod.RodXCoordinate > currentCoordinates.X) ? 
                Convert.ToInt32(rod.RodXCoordinate - rod.DynamicSector / 2.0) : 
                Convert.ToInt32(rod.RodXCoordinate + rod.DynamicSector / 2.0);

            double inms = (xintersection - currentCoordinates.X) / currentCoordinates.Vector.X;
            if (inms >= 0)
            {
                TimeSpan intersectionTime = TimeSpan.FromMilliseconds(inms);

                int yintersection = Convert.ToInt32(currentCoordinates.Vector.Y * intersectionTime.TotalMilliseconds + currentCoordinates.Y);

                if (IsCoordinatesYInRange(yintersection))
                {
                    rod.SetBallIntersection(xintersection, yintersection, currentCoordinates.Timestamp + intersectionTime);
                }
                else
                {
                    //ToDo: Call ricoshet algorithm with:
                    BallCoordinates ricoshetCoordiantes = null;
                    //ricoshetCoordiantes = RicoshetAlgorithm(currentCoordinates);
                    CalculateSectorIntersection(rod, ricoshetCoordiantes);
                }
            }
            else
            {
                rod.SetBallIntersection();
            }
        }

        #endregion Private Member Functions

        #region is coordinates in range

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="xCoordinate">X cooridanate to compare</param>
        /// <param name="yCoordinate">Y cooridanate to compare</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        public bool IsCoordinatesInRange(int xCoordinate, int yCoordinate)
        {
            return (IsCoordinatesXInRange(xCoordinate) && IsCoordinatesYInRange(yCoordinate));
        }

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="xCoordinate">X cooridanate to compare</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        public bool IsCoordinatesXInRange(int xCoordinate)
        {
            return (xCoordinate >= 0 && xCoordinate <= XMAX);
        }

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="yCoordinate">Y cooridanate to compare</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        public bool IsCoordinatesYInRange(int yCoordinate)
        {
            return (yCoordinate >= 0 && yCoordinate <= YMAX);
        }

        #endregion is coordinates in range
    }
}
