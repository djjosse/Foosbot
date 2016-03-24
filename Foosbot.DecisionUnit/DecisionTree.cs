using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    /// <summary>
    /// Decision Tree Class
    /// </summary>
    public class DecisionTree
    {
        /// <summary>
        /// Dictionary initialized from configuration file in constructor
        /// This dictionary contains X coordinates of each rod
        /// </summary>
        private Dictionary<eRod, int> _rodXCoordinate;



        /// <summary>
        /// Sector Start Coordinate X
        /// </summary>
        private int sectorStart;

        /// <summary>
        /// Sector End Coordinate X
        /// </summary>
        private int sectorEnd;

        /// <summary>
        /// Ball Radius
        /// ToDo: Calculate Dynamically
        /// </summary>
        private readonly int BALL_RADIUS = 15;

        /// <summary>
        /// Decision Tree Constructor
        /// </summary>
        public DecisionTree()
        {
            //get all rod X coordinates from configuration and store
            _rodXCoordinate = new Dictionary<eRod, int>();
            foreach(eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                int xCoord = Configuration.Attributes.GetValue<int>(rodType.ToString());
                _rodXCoordinate.Add(rodType, xCoord);
            }
        }

        public RodAction Decide(Rod rod, BallCoordinates bfc)
        {
            RodAction action = null;

            //calculate sector start and end coordinates
            sectorStart = rod.RodXCoordinate - rod.DynamicSector / 2;
            sectorEnd = rod.RodXCoordinate + rod.DynamicSector / 2;

            switch (IsBallInSector(bfc.X))
            {
                
                //Ball is in Current Rod Sector
                case eXPositionSectorRelative.IN_SECTOR:
                    //The Big Sub Tree
                    action = SubTreeBallInSector(rod.RodType, bfc);
                    break;
                //Ball is ahead of Current Rod Sector
                case eXPositionSectorRelative.AHEAD_SECTOR:
                    if (IsBallVectorToRod(bfc.Vector))
                        //Ball Vector Direction is TO Current Rod
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.VECTOR_BASED);
                    else
                        //Ball Vector Direction is FROM Current Rod
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.BEST_EFFORT);
                    break;
                //Ball is behind Current Rod Sector
                case eXPositionSectorRelative.BEHIND_SECTOR:
                    action = new RodAction(rod.RodType, eRotationalMove.RISE, eLinearMove.BEST_EFFORT);
                    break;
            }

            //Define actual desired rod location to move to
            switch (action.Linear)
            {
                case eLinearMove.BALL_Y:

                    break;
                case eLinearMove.BEST_EFFORT:

                    break;
                case eLinearMove.LEFT_BALL_DIAMETER:

                    break;
                case eLinearMove.RIGHT_BALL_DIAMETER:

                    break;
                case eLinearMove.VECTOR_BASED:

                    break;
                default:

                    break;
            }

            return action;
        }

        /// <summary>
        /// Sub Tree used to decide on action in case ball is in sector
        /// Stage 4 and further in SDD document
        /// </summary>
        /// <returns>Rod Action to be performed</returns>
        private RodAction SubTreeBallInSector(eRod rodType, BallCoordinates bfc)
        {
            //Stage 4 - get current ball relative position to rod
            eXPositionRodRelative xRelative = XPositionToRodXPostion(bfc.X, rodType);

            eYPositionPlayerRelative yRelative = YPositionToPlayerYPosition(bfc.Y, rodType);

            return null;
        }

        private eYPositionPlayerRelative YPositionToPlayerYPosition(int yBallPosition, eRod currentRod)
        {
            return eYPositionPlayerRelative.CENTER;
        }

        /// <summary>
        /// Get Current Ball Position relative to current rod in Axe X
        /// </summary>
        /// <param name="xBallPosition">X ball coordinate</param>
        /// <param name="currentRod">Current rod</param>
        /// <returns>X position relative to current rod</returns>
        private eXPositionRodRelative XPositionToRodXPostion(int xBallPosition, eRod currentRod)
        {
            int xRodPosition = _rodXCoordinate[currentRod];
            if (xBallPosition + BALL_RADIUS > xRodPosition)
                return eXPositionRodRelative.FRONT;
            if (xBallPosition - BALL_RADIUS < xRodPosition)
                return eXPositionRodRelative.BACK;
            return eXPositionRodRelative.CENTER;
        }

        private eXPositionSectorRelative IsBallInSector(int ballXcoordinate)
        {
            if (ballXcoordinate < sectorStart)
                return eXPositionSectorRelative.BEHIND_SECTOR;
            else if (ballXcoordinate > sectorEnd)
                return eXPositionSectorRelative.AHEAD_SECTOR;
            else
                return eXPositionSectorRelative.IN_SECTOR;
        }

        private bool IsBallVectorToRod(Vector2D vector)
        {
            return (vector.X < 0);
        }

    }
}
