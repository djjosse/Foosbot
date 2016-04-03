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

        /// <summary>
        /// Ricochet callculations
        /// </summary>
        private VectorUtils _vectorUtils;

        #endregion private members

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
            _vectorUtils.Initialize();

            //Set constants from configuration file
            DELAYS = TimeSpan.FromMilliseconds(Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_DELAY));
            XMAX = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            YMAX = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
        }

        public override void Job()
        {
            _publisher.Dettach(this);

            BallCoordinates ballCoordinates = _publisher.Data;
            DecisionFlow(ballCoordinates);

            _publisher.Attach(this);
        }

        /// <summary>
        /// Main Decision Flow
        /// </summary>
        /// <param name="currentCoordinates"></param>
        public void DecisionFlow(BallCoordinates currentCoordinates)
        {
            if (currentCoordinates != null)
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
                {
                    RodAction action = _decisionTree.Decide(rod, _bfc);
                    if (rod.RodType == eRod.GoalKeeper)
                    {
                        Log.Common.Info(String.Format("Decision {0} {1}", action.Linear.ToString(), action.Rotation.ToString()));
                        Marks.DrawRodPlayers(eMarks.GoalKeeper, action.LinearMovement, action.Rotation);
                    }
                    if (rod.RodType == eRod.Defence)
                    {
                        Marks.DrawRodPlayers(eMarks.Defence, action.LinearMovement, action.Rotation);
                    }
                    if (rod.RodType == eRod.Midfield)
                    {
                        Marks.DrawRodPlayers(eMarks.Midfield, action.LinearMovement, action.Rotation);
                    }
                    if (rod.RodType == eRod.Attack)
                    {
                        Marks.DrawRodPlayers(eMarks.Attack, action.LinearMovement, action.Rotation);
                    }
                }
            }
            else
            {
                Log.Common.Error(String.Format("Coordinates received from vector calculation unit are null"));
            }
        }

        #region Private Member Functions


        /// <summary>
        /// Calculate Ball Future Coordinates in actual time system can responce
        /// </summary>
        /// <param name="currentCoordinates"><Current ball coordinates/param>
        /// <param name="actionTime">Actual system responce time</param>
        private void FindBallFutureCoordinates(BallCoordinates currentCoordinates, DateTime actionTime)
        {
            try
            {
                TimeSpan deltaT = actionTime - currentCoordinates.Timestamp;

                if (currentCoordinates.IsDefined && currentCoordinates.Vector.IsDefined)
                {
                    int xfc = Convert.ToInt32(currentCoordinates.Vector.X * deltaT.TotalSeconds + currentCoordinates.X);
                    int yfc = Convert.ToInt32(currentCoordinates.Vector.Y * deltaT.TotalSeconds + currentCoordinates.Y);

                    if (IsCoordinatesInRange(xfc, yfc))
                    {
                        _bfc = new BallCoordinates(xfc, yfc, actionTime);
                    }
                    else
                    {
                        //BallCoordinates ricoshetCoordiantes = _vectorUtils.Ricochet(currentCoordinates);
                        //FindBallFutureCoordinates(ricoshetCoordiantes, actionTime);
                    }
                }
                else
                {
                    throw new NotSupportedException("Currently not defined coordinates are not supported by Pre-Decision Flow");
                }
            }
            catch (Exception e)
            {
                Log.Common.Error(String.Format("[{0}] Error: {1}", MethodBase.GetCurrentMethod().Name, e.Message));
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
                    //BallCoordinates ricoshetCoordiantes = _vectorUtils.Ricochet(currentCoordinates);
                    //CalculateSectorIntersection(rod, ricoshetCoordiantes);
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
