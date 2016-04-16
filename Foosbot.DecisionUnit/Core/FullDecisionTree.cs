// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

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

        

        

        ///// <summary>
        ///// Prepares and calls Sub Tree used to decide on action in case ball is in sector
        ///// Stage 4 and further in SDD document
        ///// </summary>
        ///// <param name="rod">Current rod</param>
        ///// <param name="bfc">Future ball coordinates</param>
        ///// <param name="responsingPlayer">Responding player on current rod (as out parameter)</param>
        ///// <returns>Rod Action to be performed</returns>
        //private RodAction EnterDecisionTreeBallInSector(IRod rod, BallCoordinates bfc, out int responsingPlayer)
        //{
        //    //Stage 4 - get current ball relative position to rod
        //    eXPositionRodRelative xRelative = XPositionToRodXPosition(bfc.X, rod.RodType);

        //    //stage 9, 12, 5 - define player to move AND get current ball relative position to player
        //    eYPositionPlayerRelative yRelative = BallYPositionToPlayerYCoordinate(bfc.Y, rod, out responsingPlayer);

        //    //stage 10, 11, 13, 14, 6, 7 - current player rotational position
        //    eRotationalMove rotPos = _currentRodRotationPosition[rod.RodType];

        //    //stage 8 - decide on UTurn direction if needed
        //    eLinearMove UTurn = eLinearMove.NA;
        //    if (yRelative == eYPositionPlayerRelative.CENTER)
        //        UTurn = (_helper.IsEnoughSpaceToMove(rod, _currentRodYCoordinate[rod.RodType], BALL_RADIUS * 2)) ?
        //            eLinearMove.RIGHT_BALL_DIAMETER : eLinearMove.LEFT_BALL_DIAMETER;

        //    return DecisionTreeBallInSector(rod.RodType, xRelative, yRelative, rotPos, UTurn);
        //}

        ///// <summary>
        ///// Sub Decision Tree starting from Stage 4 in diagram
        ///// </summary>
        ///// <param name="rodType">Current rod type</param>
        ///// <param name="xRelative">Current ball position relative to current rod (X coordinates)</param>
        ///// <param name="yRelative">Current ball position relative to chosen player (Y coordinates)</param>
        ///// <param name="rotPos">Current rod rotational state</param>
        ///// <param name="UTurn">In case of UTurn needed - UTurn left/right</param>
        ///// <returns>Rod Action to be performed for current rod</returns>
        //private RodAction DecisionTreeBallInSector(eRod rodType, eXPositionRodRelative xRelative, eYPositionPlayerRelative yRelative, eRotationalMove rotPos, eLinearMove UTurn)
        //{
        //    RodAction desiredAction = null;

        //    //Starting from Stage 4 in Decision Tree
        //    switch (xRelative)
        //    {
        //        case eXPositionRodRelative.FRONT:
        //            #region Stage 9
        //            switch (yRelative)
        //            {
        //                case eYPositionPlayerRelative.LEFT:
        //                case eYPositionPlayerRelative.RIGHT:
        //                    #region Stage 10
        //                    switch (rotPos)
        //                    {
        //                        case eRotationalMove.RISE:
        //                        case eRotationalMove.DEFENCE:
        //                            //Leafs 1, 2, 7, 8
        //                            desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
        //                            break;
        //                        case eRotationalMove.KICK:
        //                            //Leafs 3, 9
        //                            desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.NA);
        //                            break;
        //                    }
        //                    break;
        //                    #endregion Stage 10
        //                case eYPositionPlayerRelative.CENTER:
        //                    #region Stage 11
        //                    switch (rotPos)
        //                    {
        //                        case eRotationalMove.RISE:
        //                        case eRotationalMove.DEFENCE:
        //                            //Leaf 4, 5
        //                            desiredAction = new RodAction(rodType, eRotationalMove.KICK, eLinearMove.NA);
        //                            break;
        //                        case eRotationalMove.KICK:
        //                            //Stage 8 - (leaf 6)
        //                            desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
        //                            break;
        //                    }
        //                    break;
        //                    #endregion Stage 11
        //            }
        //            break;
        //            #endregion Stage 9
        //        case eXPositionRodRelative.CENTER:
        //            #region Stage 12
        //            switch (yRelative)
        //            {
        //                case eYPositionPlayerRelative.LEFT:
        //                case eYPositionPlayerRelative.RIGHT:
        //                    #region Stage 13
        //                    switch (rotPos)
        //                    {
        //                        case eRotationalMove.RISE:
        //                            //Leaf 10, 16
        //                            desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
        //                            break;
        //                        case eRotationalMove.DEFENCE:
        //                            //Leaf 11, 17
        //                            desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
        //                            break;
        //                        case eRotationalMove.KICK:
        //                            //Leaf 12, 18
        //                            desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.NA);
        //                            break;
        //                    }
        //                    break;
        //                    #endregion Stage 13
        //                case eYPositionPlayerRelative.CENTER:
        //                    #region Stage 14
        //                    switch (rotPos)
        //                    {
        //                        case eRotationalMove.RISE:
        //                            //Leaf 13
        //                            desiredAction = new RodAction(rodType, eRotationalMove.KICK, eLinearMove.NA);
        //                            break;
        //                        case eRotationalMove.KICK:
        //                            //Stage 8 - (Leaf 15)
        //                            desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
        //                            break;
        //                    }
        //                    break;
        //                    #endregion Stage 14
        //            }
        //            break;
        //            #endregion Stage 12
        //        case eXPositionRodRelative.BACK:
        //            #region Stage 5
        //            switch (yRelative)
        //            {
        //                case eYPositionPlayerRelative.LEFT:
        //                case eYPositionPlayerRelative.RIGHT:
        //                    #region Stage 6
        //                    switch (rotPos)
        //                    {
        //                        case eRotationalMove.RISE:
        //                            //Leaf 19, 25
        //                            desiredAction = new RodAction(rodType, eRotationalMove.NA, eLinearMove.BALL_Y);
        //                            break;
        //                        case eRotationalMove.DEFENCE:
        //                            //Leaf 20, 26
        //                            desiredAction = new RodAction(rodType, eRotationalMove.RISE, eLinearMove.NA);
        //                            break;
        //                        case eRotationalMove.KICK:
        //                            //Leaf 21, 27
        //                            desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.NA);
        //                            break;
        //                    }
        //                    break;
        //                    #endregion Stage 6
        //                case eYPositionPlayerRelative.CENTER:
        //                    #region Stage 7
        //                    switch (rotPos)
        //                    {
        //                        case eRotationalMove.RISE:
        //                            //Leaf 22
        //                            desiredAction = new RodAction(rodType, eRotationalMove.KICK, eLinearMove.NA);
        //                            break;
        //                        case eRotationalMove.DEFENCE:
        //                            //Stage 8 - (Leaf 23)
        //                            desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
        //                            break;
        //                        case eRotationalMove.KICK:
        //                            //Stage 8 - (Leaf 24)
        //                            desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
        //                            break;
        //                    }
        //                    break;
        //                    #endregion Stage 7
        //            }
        //            break;
        //            #endregion Stage 5
        //    }

        //    return desiredAction;
        //}

        

        ///// <summary>
        ///// Get Current Ball Position relative to current rod in Axe X
        ///// </summary>
        ///// <param name="xBallPosition">X ball coordinate</param>
        ///// <param name="currentRod">Current rod</param>
        ///// <returns>X position relative to current rod</returns>
        //private eXPositionRodRelative XPositionToRodXPosition(int xBallPosition, eRod currentRod)
        //{
        //    int xRodPosition = _rodXCoordinate[currentRod];
        //    if (xBallPosition + BALL_RADIUS > xRodPosition)
        //        return eXPositionRodRelative.FRONT;
        //    if (xBallPosition - BALL_RADIUS < xRodPosition)
        //        return eXPositionRodRelative.BACK;
        //    return eXPositionRodRelative.CENTER;
        //}
    }
}
