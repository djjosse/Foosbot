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
        /// Sector Start Coordinate X
        /// </summary>
        private int sectorStart;

        /// <summary>
        /// Sector End Coordinate X
        /// </summary>
        private int sectorEnd;


        public RodAction Decide(Rod rod, BallCoordinates bfc)
        {
            RodAction action = null;

            //calculate sector start and end coordinates
            sectorStart = rod.RodXCoordinate - rod.DynamicSector / 2;
            sectorEnd = rod.RodXCoordinate + rod.DynamicSector / 2;

            switch (IsBallInSector(bfc.X))
            {
                
                //Ball is in Current Rod Sector
                case eBallRelativePos.IN_SECTOR:
                    //The Big Sub Tree
                    action = SubTreeBallInSector(rod.RodType, bfc);
                    break;
                //Ball is ahead of Current Rod Sector
                case eBallRelativePos.AHEAD_SECTOR:
                    if (IsBallVectorToRod(bfc.Vector))
                        //Ball Vector Direction is TO Current Rod
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.VECTOR_BASED);
                    else
                        //Ball Vector Direction is FROM Current Rod
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.BEST_EFFORT);
                    break;
                //Ball is behind Current Rod Sector
                case eBallRelativePos.BEHIND_SECTOR:
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
        /// Stage 4 in SDD document
        /// </summary>
        /// <returns>Rod Action to be performed</returns>
        private RodAction SubTreeBallInSector(eRod rodType, BallCoordinates bfc)
        {
            return null;
        }

        private eBallRelativePos IsBallInSector(int ballXcoordinate)
        {
            if (ballXcoordinate < sectorStart)
                return eBallRelativePos.BEHIND_SECTOR;
            else if (ballXcoordinate > sectorEnd)
                return eBallRelativePos.AHEAD_SECTOR;
            else
                return eBallRelativePos.IN_SECTOR;
        }

        private bool IsBallVectorToRod(Vector2D vector)
        {
            return (vector.X < 0);
        }

    }
}
