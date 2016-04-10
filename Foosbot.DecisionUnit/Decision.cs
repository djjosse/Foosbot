// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.VectorCalculation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Decision Unit Main Class
    /// </summary>
    public class Decision : Observer<BallCoordinates>
    {
        #region Constants

        /// <summary>
        /// Mechanical, Calculation and other delays
        /// </summary>
        private readonly TimeSpan DELAYS;

        /// <summary>
        /// Table X lenght (width) in Foosbot world (POINTS)
        /// </summary>
        private readonly int XMAX_PTS;

        /// <summary>
        /// Table Y lenght (height) in Foosbot world (POINTS)
        /// </summary>
        private readonly int YMAX_PTS;

        /// <summary>
        /// Table X lenght (width) in Foosbot world (MM)
        /// </summary>
        protected int XMAX_MM;

        /// <summary>
        /// Table Y lenght (height) in Foosbot world (MM)
        /// </summary>
        protected int YMAX_MM;

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

        /// <summary>
        /// Ricochet callculations
        /// </summary>
        private VectorUtils _vectorUtils;

        #endregion private members

        public Dictionary<eRod, RodActionPublisher> RodActionPublishers;

        /// <summary>
        /// Decision Unit Constructor
        /// </summary>
        public Decision(Publisher<BallCoordinates> vectorPublisher)
            : base(vectorPublisher)
        {
            //Create all rods
            _rods = new Dictionary<eRod, Rod>();
            foreach (eRod type in Enum.GetValues(typeof(eRod)))
                _rods.Add(type, new Rod(type));

            //Create decision tree instance
            _decisionTree = new DecisionTree();

            //Used for Ricochet calculations
            _vectorUtils = new VectorUtils();
            //Initialize to work in MM
            _vectorUtils.Initialize(true);

            //Set constants from configuration file
            DELAYS = TimeSpan.FromMilliseconds(Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_DELAY));
            XMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            YMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);

            XMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            YMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);

            RodActionPublishers = new Dictionary<eRod, RodActionPublisher>();
            foreach(eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                RodActionPublishers.Add(rodType, new RodActionPublisher());
            }
        }

        public override void Job()
        {
            try
            {
                _publisher.Dettach(this);

                BallCoordinates ballCoordinates = _publisher.Data;
                DecisionFlow(ballCoordinates);
            }
            catch (ThreadInterruptedException)
            {
                /* Got new data while wait - it is good */
            }
            catch (Exception ex)
            {
                Log.Common.Error(String.Format("[{0}] [{1}]",
                    MethodBase.GetCurrentMethod().DeclaringType.Name, ex.Message));
            }
            finally
            {
                _publisher.Attach(this);
            }
        }

        /// <summary>
        /// Convert ball coordinates from point units to mm units
        /// </summary>
        /// <param name="pointsCoords">Coordinates in points</param>
        /// <returns>Coordinates in millimeters</returns>
        public BallCoordinates ConvertPointsToMillimeters(BallCoordinates pointsCoords)
        {
            BallCoordinates mmCoords = null;
            if (pointsCoords != null && pointsCoords.IsDefined)
            {
                int xMm = pointsCoords.X * XMAX_MM / XMAX_PTS;
                int yMm = pointsCoords.Y * YMAX_MM / YMAX_PTS;
                mmCoords = new BallCoordinates(xMm, yMm, pointsCoords.Timestamp);
            }
            else
            {
                return pointsCoords;
            }

            if (pointsCoords.Vector != null && pointsCoords.Vector.IsDefined)
            {
                double xMm = pointsCoords.Vector.X * (double)XMAX_MM / (double)XMAX_PTS;
                double yMm = pointsCoords.Vector.Y * (double)YMAX_MM / (double)YMAX_PTS;
                mmCoords.Vector = new Vector2D(xMm, yMm);
            }
            else
            {
                mmCoords.Vector = pointsCoords.Vector;
            }
            return mmCoords;
        }

        /// <summary>
        /// Main Decision Flow
        /// </summary>
        /// <param name="currentCoordinates">Ball current coordinates and vector to make decision on movement</param>
        /// <exception cref="ArgumentException">Thrown in case current ball coordinates are null</exception>
        public void DecisionFlow(BallCoordinates currentCoordinates)
        {
            if (currentCoordinates == null)
                throw new ArgumentException(String.Format("[{0}] Coordinates received from vector calculation unit are null",
                    MethodBase.GetCurrentMethod().Name));

            //Convert pts and pts/sec to mm and mm/sec
            currentCoordinates = ConvertPointsToMillimeters(currentCoordinates);

            //Calculate Actual Possible Action Time
            DateTime timeOfAction = FindActionTime(DELAYS);

            //Calculate ball future coordinates
            _bfc = FindBallFutureCoordinates(currentCoordinates, timeOfAction);

            //Calculate dynamic sectors
            CalculateDynamicSectors(currentCoordinates);

            //Calculate Rod Intersection with a ball for all rods
            CalculateSectorIntersection(currentCoordinates);

            //Take decision for each rod
            foreach (Rod rod in _rods.Values)
            {
                RodAction action = _decisionTree.Decide(rod, _bfc);
                if (rod.RodType == eRod.GoalKeeper)
                    Log.Common.Debug(String.Format(
                     "[{0}] New action for {1} DC: {2} pts, Servo: {3}",
                            MethodBase.GetCurrentMethod().Name, rod.RodType.ToString(), action.LinearMovement, action.Rotation.ToString()));
                eMarks rodMark = (eMarks)Enum.Parse(typeof(eMarks), rod.RodType.ToString(), true);
                Marks.DrawRodPlayers(rodMark, action.LinearMovement, action.Rotation);
                RodActionPublishers[rod.RodType].UpdateAndNotify(action);
            }
        }

        #region Private Member Functions

        /// <summary>
        /// Calculate Actual Possible Action Time
        /// </summary>
        /// <param name="delays">System delays</param>
        /// <returns>Possible action time stamp</returns>
        private DateTime FindActionTime(TimeSpan delays)
        {
            return DateTime.Now + delays;
        }

        /// <summary>
        /// Calculate Ball Future Coordinates in actual time system can responce
        /// </summary>
        /// <param name="currentCoordinates"><Current ball coordinates/param>
        /// <param name="actionTime">Actual system responce time</param>
        /// <returns>Ball Future coordinates</returns>
        public BallCoordinates FindBallFutureCoordinates(BallCoordinates currentCoordinates, DateTime actionTime)
        {
            if (currentCoordinates == null || !currentCoordinates.IsDefined)
                throw new ArgumentException(String.Format(
                    "[{0}] Unable to calculate ball future coordinates while current coordinates are null or undefined", 
                        MethodBase.GetCurrentMethod().Name));

            if (actionTime < currentCoordinates.Timestamp)
                throw new ArgumentException(String.Format(
                    "[{0}] Unable to calculate ball future coordinates while action time is earlier than time stamp",
                        MethodBase.GetCurrentMethod().Name));

            BallCoordinates bfc;

            if (currentCoordinates.Vector == null || !currentCoordinates.Vector.IsDefined)
            {
                bfc = new BallCoordinates(currentCoordinates.X, currentCoordinates.Y, actionTime);
                bfc.Vector = currentCoordinates.Vector;
                return bfc;
            }

            bfc = currentCoordinates;
            try
            {
                TimeSpan deltaT = actionTime - currentCoordinates.Timestamp;

                int xfc = Convert.ToInt32(currentCoordinates.Vector.X * deltaT.TotalSeconds + currentCoordinates.X);
                int yfc = Convert.ToInt32(currentCoordinates.Vector.Y * deltaT.TotalSeconds + currentCoordinates.Y);

                if (IsCoordinatesInRange(xfc, yfc))
                {
                    bfc = new BallCoordinates(xfc, yfc, actionTime);
                    bfc.Vector = currentCoordinates.Vector;
                }
                else
                {
                    BallCoordinates ricoshetCoordiantes = _vectorUtils.Ricochet(currentCoordinates);
                    return FindBallFutureCoordinates(ricoshetCoordiantes, actionTime);
                }
            }
            catch (Exception e)
            {
                Log.Common.Error(String.Format("[{0}] Error: {1}", MethodBase.GetCurrentMethod().Name, e.Message));
            }
            return bfc;
        }

        /// <summary>
        /// Calculates Dynamic Sectors per each rod
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates</param>
        private void CalculateDynamicSectors(BallCoordinates currentCoordinates)
        {
            foreach (Rod rod in _rods.Values)
                rod.CalculateDynamicSector(currentCoordinates);
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
            //if timespan is bigger than 0 && Vector X is also bigger than 0
            if (inms >= 0 && Math.Abs(currentCoordinates.Vector.X) > 0)
            {
                TimeSpan intersectionTime = TimeSpan.FromMilliseconds(inms);

                int yintersection = Convert.ToInt32(currentCoordinates.Vector.Y * intersectionTime.TotalMilliseconds + currentCoordinates.Y);

                if (IsCoordinatesYInRange(yintersection))
                {
                    rod.SetBallIntersection(xintersection, yintersection, currentCoordinates.Timestamp + intersectionTime);
                }
                else
                {
                    /*
                    BallCoordinates ricoshetCoordiantes = _vectorUtils.Ricochet(currentCoordinates);
                    CalculateSectorIntersection(rod, ricoshetCoordiantes);
                    */
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
            return (xCoordinate >= 0 && xCoordinate <= XMAX_MM);
        }

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="yCoordinate">Y cooridanate to compare</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        public bool IsCoordinatesYInRange(int yCoordinate)
        {
            return (yCoordinate >= 0 && yCoordinate <= YMAX_MM);
        }

        #endregion is coordinates in range
    }
}
