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
             * Beta Version of inner DECISION TREE
             */
            switch(relativeX)
            {
                case eXPositionRodRelative.FRONT:
                    switch(relativeY)
                    {
                        case eYPositionPlayerRelative.RIGHT:
                        case eYPositionPlayerRelative.LEFT:
                            switch(rod.State.ServoPosition)
                            {
                                case eRotationalMove.RISE:
                                case eRotationalMove.DEFENCE:
                                    action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
                                    break;
                                case eRotationalMove.KICK:
                                    action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.NA);
                                    break;
                            }
                            break;
                        case eYPositionPlayerRelative.CENTER:
                            switch (rod.State.ServoPosition)
                            {
                                case eRotationalMove.RISE:
                                case eRotationalMove.DEFENCE:
                                    action = new RodAction(rod.RodType, eRotationalMove.KICK, eLinearMove.NA);
                                    break;
                                case eRotationalMove.KICK:
                                    if (_helper.IsEnoughSpaceToMove(rod, rod.State.DcPosition, BALL_RADIUS))
                                    {
                                        action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.RIGHT_BALL_DIAMETER);
                                    }
                                    else
                                    {
                                        action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.LEFT_BALL_DIAMETER);
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case eXPositionRodRelative.CENTER:
                case eXPositionRodRelative.BACK:
                    switch (relativeY)
                    {
                        case eYPositionPlayerRelative.RIGHT:
                        case eYPositionPlayerRelative.LEFT:
                            switch (rod.State.ServoPosition)
                            {
                                case eRotationalMove.RISE:
                                    action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.BALL_Y);
                                    break;
                                case eRotationalMove.DEFENCE:
                                    action = new RodAction(rod.RodType, eRotationalMove.RISE, eLinearMove.NA);
                                    break;
                                case eRotationalMove.KICK:
                                    action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.NA);
                                    break;
                            }
                            break;
                        case eYPositionPlayerRelative.CENTER:
                            switch (rod.State.ServoPosition)
                            {
                                case eRotationalMove.RISE:
                                    action = new RodAction(rod.RodType, eRotationalMove.KICK, eLinearMove.NA);
                                    break;
                                case eRotationalMove.DEFENCE:
                                case eRotationalMove.KICK:
                                    if (_helper.IsEnoughSpaceToMove(rod, rod.State.DcPosition, BALL_RADIUS))
                                    {
                                        action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.RIGHT_BALL_DIAMETER);
                                    }
                                    else
                                    {
                                        action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.LEFT_BALL_DIAMETER);
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }

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
