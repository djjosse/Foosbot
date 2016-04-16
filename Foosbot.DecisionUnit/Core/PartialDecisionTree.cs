using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using Foosbot.DecisionUnit.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    public class PartialDecisionTree : DecisionTree
    {
        /// <summary>
        /// Decision Tree Constructor for tree without Subtree
        /// </summary>
        /// <param name="decisionHelper">Decision Helper Instance [default is null then will be constructed using Configuration File]</param>
        /// <param name="ballRadius">Ball Radius in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="tableWidth">Table Width (Y Axe) in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="playerWidth">Player Width in mm [default is -1 will be taken from Configuration File]</param>
        public PartialDecisionTree(IDecisionHelper helper = null, int ballRadius = -1, int tableWidth = -1, int playerWidth = -1)
            :base(helper, ballRadius, tableWidth, playerWidth)
        {

        }

        /// <summary>
        /// Main Decision Flow Method desides on action and sets property of responding player
        /// </summary>
        /// <param name="rod">Rod to use for decision</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <returns>Rod Action to perform</returns>
        public override RodAction Decide(IRod rod, BallCoordinates bfc)
        {
            //Get relative Y position and set Responding Player
            eYPositionPlayerRelative relativeY = BallYPositionToPlayerYCoordinate(bfc.Y, rod);

            //Get relative X position
            eXPositionRodRelative relativeX = BallXPositionToRodXPosition(bfc.X, rod);

            RodAction action = new RodAction(rod.RodType);

            /*
             * For Alpha this is good enough to make a kick.
             * For Beta need to define the actual sub tree.
             */
            if (relativeX.Equals(eXPositionRodRelative.FRONT))
                action = new RodAction(rod.RodType, eRotationalMove.KICK, eLinearMove.VECTOR_BASED);
            if (relativeX.Equals(eXPositionRodRelative.BACK))
                action = new RodAction(rod.RodType, eRotationalMove.RISE, eLinearMove.VECTOR_BASED);
            if (relativeX.Equals(eXPositionRodRelative.CENTER))
                action = new RodAction(rod.RodType, eRotationalMove.KICK, eLinearMove.BALL_Y);

            //Define actual desired rod coordinate to move to
            int startStopperDesiredY = CalculateNewRodCoordinate(rod, RespondingPlayer, bfc, action.Linear);
            action.DcCoordinate = rod.NearestPossibleDcPosition(startStopperDesiredY);

            //Set last decided rod and player coordinates 
            rod.State.DcPosition = action.DcCoordinate;
            rod.State.ServoPosition = action.Rotation;

            return action;
        }
    }
}
