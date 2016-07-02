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
using System.Reflection;

namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Decision Tree Class
    /// </summary>
    public class FullDecisionTree : DecisionTree
    {
        #region Constructors

        /// <summary>
        /// Decision Tree Constructor
        /// </summary>
        /// <param name="subtree">Decision Sub Tree</param>
        /// <param name="decisionHelper">Decision Helper [default is null then will be constructed using Configuration File]</param>
        /// <param name="ballRadius">Ball Radius in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="tableWidth">Table Width (Y Axe) in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="playerWidth">Player Width in mm [default is -1 will be taken from Configuration File]</param>
        public FullDecisionTree(IDecisionTree subtree, IDecisionHelper decisionHelper = null, int ballRadius = -1, int tableWidth = -1, int playerWidth = -1)
            : base(subtree, decisionHelper, ballRadius, tableWidth, playerWidth)
        {
        }

        #endregion Constructors

        #region DecisionTree implementation

        /// <summary>
        /// Main Decision Flow Method
        /// </summary>
        /// <param name="rod">Rod to use for decision</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <returns>Rod Action to perform</returns>
        public override RodAction Decide(IRod rod, BallCoordinates bfc)
        {
            //Player to respond  (index base is 0)
            int respondingPlayer = -1;

            //Chose responding player on rod and define action to perform
            RodAction action = DefineActionAndRespondingPlayer(rod, bfc, out respondingPlayer);

            //Define actual desired rod coordinate to move to
            int startStopperDesiredY = CalculateNewRodCoordinate(rod, respondingPlayer, bfc, action.Linear);
            action.DcCoordinate = rod.NearestPossibleDcPosition(startStopperDesiredY);

            //Set last decided rod and player coordinates 
            rod.State.DcPosition = action.DcCoordinate;
            if (_helper.ShouldSetServoStateFromTree(rod.RodType))
                rod.State.ServoPosition = action.Rotation;
            return action;
        }

        #endregion DecisionTree implementation

        #region Protected methods

        /// <summary>
        /// Choose player to respond on current rod and action to perform
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="bfc">Ball Future Coordinates</param>
        /// <param name="respondingPlayer">Responding Player index (1 based) on current rod [out]</param>
        /// <returns>Rod Action to be performed</returns>
        protected RodAction DefineActionAndRespondingPlayer(IRod rod, BallCoordinates bfc, out int respondingPlayer)
        {
            if (rod == null)
                throw new ArgumentException(String.Format(
                     "[{0}] Unable to define action and responding player while rod argument is NULL!",
                        MethodBase.GetCurrentMethod().Name));

            if (bfc == null || !bfc.IsDefined)
                throw new ArgumentException(String.Format(
                    "[{0}] Unable to define action and responding player while ball coordinates are NULL or UNDEFINED!",
                        MethodBase.GetCurrentMethod().Name));

            RodAction action = null;
            respondingPlayer = -1;
            switch (_helper.IsBallInSector(bfc.X, rod.RodXCoordinate, rod.DynamicSector))
            {
                //Ball is in Current Rod Sector
                case eXPositionSectorRelative.IN_SECTOR:
                    action = SubTree.Decide(rod, bfc);
                    respondingPlayer = SubTree.RespondingPlayer;
                    break;

                    /* OLD :
                     *  //The Big Sub Tree
                     *  action = EnterDecisionTreeBallInSector(rod, bfc, out respondingPlayer);
                     */

                //Ball is ahead of Current Rod Sector
                case eXPositionSectorRelative.AHEAD_SECTOR:
                    //Ball Vector Direction is TO Current Rod and we have intersection point
                    if (_helper.IsBallVectorToRod(bfc.Vector) &&
                            rod.Intersection.IsDefined)
                    {
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.VECTOR_BASED);
                        
                        //Define responding player index
                        BallYPositionToPlayerYCoordinate(bfc.Y, rod);
                        respondingPlayer = this.RespondingPlayer;
                    }
                    else
                    {
                        //Ball Vector Direction is FROM Current Rod
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.BEST_EFFORT);
                    }
                    break;
                //Ball is behind Current Rod Sector
                case eXPositionSectorRelative.BEHIND_SECTOR:
                    action = new RodAction(rod.RodType, eRotationalMove.RISE, eLinearMove.BEST_EFFORT);
                    break;
            }
            return action;
        }

        
        
        #endregion Protected methods
    }
}
