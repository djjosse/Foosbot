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
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Decision Tree abstract base class 
    /// </summary>
    public abstract class DecisionTree : IDecisionTree
    {
        #region Constants

        protected readonly int BALL_RADIUS;
        protected readonly int TABLE_WIDTH;
        protected readonly int PLAYER_WIDTH;

        #endregion Constants

        /// <summary>
        /// Decision helper instance
        /// </summary>
        protected IDecisionHelper _helper;

        /// <summary>
        /// Decision Tree Constructor
        /// </summary>
        /// <param name="decisionHelper">Decision Helper Instance [default is null then will be constructed using Configuration File]</param>
        /// <param name="ballRadius">Ball Radius in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="tableWidth">Table Width (Y Axe) in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="playerWidth">Player Width in mm [default is -1 will be taken from Configuration File]</param>
        public DecisionTree(IDecisionHelper helper = null, int ballRadius = -1, int tableWidth = -1, int playerWidth = -1)
        {
            BALL_RADIUS = (ballRadius > 0) ? ballRadius
                : Configuration.Attributes.GetValue<int>(Configuration.Names.BALL_DIAMETR) / 2;
            TABLE_WIDTH = (tableWidth > 0) ? tableWidth
                : Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            PLAYER_WIDTH = (playerWidth > 0) ? playerWidth
                : Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_PLAYER_WIDTH);

            if (helper != null)
            {
                _helper = helper;
            }
            else
            {
                int start = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_START_Y);
                int end = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_END_Y);
                _helper = new DecisionHelper(start, end);
            }
        }

        /// <summary>
        /// Main Decision Flow Method
        /// </summary>
        /// <param name="rod">Rod to use for decision</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <returns>Rod Action to perform</returns>
        public abstract RodAction Decide(IRod rod, BallCoordinates bfc);

        /// <summary>
        /// Define Ball Y position relative to responding player of current rod and re define index of responding rod (as out)
        /// </summary>
        /// <param name="yBallCoordinate">Current ball Y coordinate</param>
        /// <param name="currentRod">Current rod</param>
        /// <param name="playerToResponse">Responding player[out] (index base is 0)</param>
        /// <returns>Ball Y position relative to responding player</returns>
        protected eYPositionPlayerRelative BallYPositionToPlayerYCoordinate(int yBallCoordinate, IRod currentRod, out int playerToResponse)
        {
            //get array of players and their Y coordinates (player i stored in array index i - 1)
            int[] currentPlayerYs = _helper.AllCurrentPlayersYCoordinates(currentRod, currentRod.State.DcPosition);

            //calculate movements for each player to reach the ball (Y Axe only)
            int[] movements = _helper.CalculateYMovementForAllPlayers(currentPlayerYs, yBallCoordinate);

            //convert movement to distance on Y Axe (Absolute value)
            int[] absoluteDistance = movements.Select(x => Math.Abs(x)).ToArray();

            //get index of fist minimal distance (movement is here: movements[minIndexFirst])
            int minIndexFirst = Array.IndexOf(absoluteDistance, absoluteDistance.Min());

            //eliminate the first minimal distance
            absoluteDistance[minIndexFirst] = TABLE_WIDTH * 100;

            //get index of second minimal distance (movement is here: movements[minIndexSecond])
            int minIndexSecond = Array.IndexOf(absoluteDistance, absoluteDistance.Min());

            //chosen movement to perform
            int movement = 0;

            //Define actual player to response 
            if (_helper.IsEnoughSpaceToMove(currentRod, currentRod.State.DcPosition, movements[minIndexFirst]))
            {
                //as index starts from 0 => first one is 1
                playerToResponse = minIndexFirst + 1;
                movement = movements[minIndexFirst];
            }
            else
            {
                //as index starts from 0 => first one is 1
                playerToResponse = minIndexSecond + 1;
                movement = movements[minIndexSecond];
            }

            //In case we reach the ball - no move needed
            if (Math.Abs(movement) < PLAYER_WIDTH)
                return eYPositionPlayerRelative.CENTER;
            else if (movement > 0)
                return eYPositionPlayerRelative.RIGHT;
            else // (movement < 0)
                return eYPositionPlayerRelative.LEFT;
        }

        /// <summary>
        /// Calculate actual linear movement (Y Axe) for current rod to perform
        /// Is based on desired linear move type
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="respondingPlayer">Responding player in current rod (1 based index)</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <param name="desiredLinearMove">Desired Linear Move Type</param>
        /// <returns>New rod coordinate to move to (Axe Y)</returns>
        protected int CalculateNewRodCoordinate(IRod rod, int respondingPlayer, BallCoordinates bfc, eLinearMove desiredLinearMove)
        {
            //Define actual desired rod coordinate to move to
            //NOTE: responding player might be undefined will be -1
            switch (desiredLinearMove)
            {
                case eLinearMove.BALL_Y:
                    return _helper.LocateRespondingPlayer(rod, bfc.Y, respondingPlayer);
                //    return bfc.Y - _helper.CalculateCurrentPlayerYCoordinate(rod, _currentRodYCoordinate[rod.RodType], respondingPlayer);

                //case eLinearMove.LEFT_BALL_DIAMETER:
                //    return (-1) * 2 * BALL_RADIUS;
                //case eLinearMove.RIGHT_BALL_DIAMETER:
                //    return 2 * BALL_RADIUS;
                case eLinearMove.VECTOR_BASED:
                    if (rod.Intersection.IsDefined)
                    {
                        return _helper.LocateRespondingPlayer(rod, rod.Intersection.Y, respondingPlayer);
                    }
                    //return rod.IntersectionY - _helper.CalculateCurrentPlayerYCoordinate(rod, _currentRodYCoordinate[rod.RodType], respondingPlayer);
                    return rod.State.DcPosition;
                //case eLinearMove.BEST_EFFORT:
                //    return rod.BestEffort;
                default:
                    return rod.State.DcPosition;
            }
        }
    }
}
