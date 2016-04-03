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
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Current Y coordinate of current rod (stopper) per each rod
        /// </summary>
        private Dictionary<eRod, int> _currentRodYCoordinate;

        /// <summary>
        /// Current rod rotation position
        /// </summary>
        private Dictionary<eRod, eRotationalMove> _currentRodRotationPosition;

        /// <summary>
        /// Last decided rod movement (on Axe Y)
        /// </summary>
        private Dictionary<eRod, int> _lastDecidedRodLinearMovement;

        /// <summary>
        /// Last decided rod rotation position
        /// </summary>
        private Dictionary<eRod, eRotationalMove> _lastDecidedRodRotationPosition;

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
        /// Foosbot World Axe Y Length
        /// </summary>
        public readonly int Y_AXE_SIZE;

        /// <summary>
        /// Rod Start Y coordinate (Stopper Coordinate)
        /// </summary>
        public readonly int ROD_START_Y;

        /// <summary>
        /// Rod End Y coordinate (Stopper Coordinate)
        /// </summary>
        public readonly int ROD_END_Y;

        /// <summary>
        /// Player width - legs square width
        /// </summary>
        public readonly int PLAYER_WIDTH;

        /// <summary>
        /// Decision Tree Constructor
        /// </summary>
        public DecisionTree()
        {
            //get all rod X coordinates from configuration and store
            _rodXCoordinate = new Dictionary<eRod, int>();
            //set initial (stopper) coordinate per each rod
            _currentRodYCoordinate = new Dictionary<eRod, int>();
            //set initial rotational position per each rod
            _currentRodRotationPosition = new Dictionary<eRod, eRotationalMove>();
            //set initial decided movement for each rod
            _lastDecidedRodLinearMovement = new Dictionary<eRod, int>();
            //set initial decided rotational position per each rod
            _lastDecidedRodRotationPosition = new Dictionary<eRod, eRotationalMove>();
            foreach(eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                int xCoord = Configuration.Attributes.GetRodXCoordinate(rodType);
                _rodXCoordinate.Add(rodType, xCoord);
                _currentRodYCoordinate.Add(rodType, 0);
                _currentRodRotationPosition.Add(rodType, eRotationalMove.DEFENCE);
                _lastDecidedRodLinearMovement.Add(rodType, 0);
                _lastDecidedRodRotationPosition.Add(rodType, eRotationalMove.DEFENCE);
            }

            Y_AXE_SIZE = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            ROD_START_Y = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_START_Y);
            ROD_END_Y = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_END_Y);
            PLAYER_WIDTH = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_PLAYER_WIDTH);
        }

        public RodAction Decide(Rod rod, BallCoordinates bfc)
        {
            //set current coordinates and current rotational position of rod
            //ToDo: Use timer
            _currentRodYCoordinate[rod.RodType] += _lastDecidedRodLinearMovement[rod.RodType];
            _currentRodRotationPosition[rod.RodType] = _lastDecidedRodRotationPosition[rod.RodType];

            //calculate sector start and end coordinates
            sectorStart = rod.RodXCoordinate - rod.DynamicSector / 2;
            sectorEnd = rod.RodXCoordinate + rod.DynamicSector / 2;

            //Player to respond  (index base is 0)
            int respondingPlayer = -1;

            //Chose responding player on rod and define action to perform
            RodAction action = DefineActionAndRespondingPlayer(rod, bfc, out respondingPlayer);

            //Define actual desired rod coordinate to move to
            action.LinearMovement = CalculateActualLinearMovementToPerform(rod, respondingPlayer, bfc, action.Linear);

            //ToDo: Add Start Movement Timer Here

            //Set last decided rod and player coordinates 
            _lastDecidedRodLinearMovement[rod.RodType] = action.LinearMovement;
            _lastDecidedRodRotationPosition[rod.RodType] = (action.Rotation != eRotationalMove.NA) ? action.Rotation : _lastDecidedRodRotationPosition[rod.RodType];
            return action;
        }

        /// <summary>
        /// Calculate actual linear movement (Y Axe) for current rod to perform
        /// Is based on desired linear move type
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="respondingPlayer">Responding player in current rod (1 based index)</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <param name="desiredLinearMove">Desired Linear Move Type</param>
        /// <returns>Movement to be performed (Axe Y)</returns>
        private int CalculateActualLinearMovementToPerform(Rod rod, int respondingPlayer, BallCoordinates bfc, eLinearMove desiredLinearMove)
        {
            //Define actual desired rod coordinate to move to
            switch (desiredLinearMove)
            {
                case eLinearMove.BALL_Y:
                    return bfc.Y - CalculateCurrentPlayerYCoordinate(rod, _currentRodYCoordinate[rod.RodType], respondingPlayer);
                case eLinearMove.BEST_EFFORT:
                    return rod.BestEffort - CalculateCurrentPlayerYCoordinate(rod, _currentRodYCoordinate[rod.RodType], 1);
                case eLinearMove.LEFT_BALL_DIAMETER:
                    return (-1) * 2 * BALL_RADIUS;
                case eLinearMove.RIGHT_BALL_DIAMETER:
                    return 2 * BALL_RADIUS;
                case eLinearMove.VECTOR_BASED:
                    return rod.IntersectionY - CalculateCurrentPlayerYCoordinate(rod, _currentRodYCoordinate[rod.RodType], respondingPlayer);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Choose player to respond on current rod and action to perform
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="bfc">Ball Future Coordinates</param>
        /// <param name="respondingPlayer">Responding Player index (1 based) on current rod [out]</param>
        /// <returns>Rod Action to be performed</returns>
        private RodAction DefineActionAndRespondingPlayer(Rod rod, BallCoordinates bfc, out int respondingPlayer)
        {
            RodAction action = null;
            respondingPlayer = -1;
            switch (IsBallInSector(bfc.X))
            {
                //Ball is in Current Rod Sector
                case eXPositionSectorRelative.IN_SECTOR:
                    //The Big Sub Tree
                    action = EnterDecisionTreeBallInSector(rod, bfc, out respondingPlayer);
                    break;
                //Ball is ahead of Current Rod Sector
                case eXPositionSectorRelative.AHEAD_SECTOR:
                    if (IsBallVectorToRod(bfc.Vector))
                    {
                        //Ball Vector Direction is TO Current Rod
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.VECTOR_BASED);
                        //Define responding player index
                        BallYPositionToPlayerYCoordinate(rod.IntersectionY, rod, out respondingPlayer);
                    }
                    else
                        //Ball Vector Direction is FROM Current Rod
                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.BEST_EFFORT);
                        //not really relevant
                        respondingPlayer = 1;
                    break;
                //Ball is behind Current Rod Sector
                case eXPositionSectorRelative.BEHIND_SECTOR:
                    action = new RodAction(rod.RodType, eRotationalMove.RISE, eLinearMove.BEST_EFFORT);
                    break;
            }
            return action;
        }

        /// <summary>
        /// Prepares and calls Sub Tree used to decide on action in case ball is in sector
        /// Stage 4 and further in SDD document
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="bfc">Future ball coordinates</param>
        /// <param name="responsingPlayer">Responding player on current rod (as out parameter)</param>
        /// <returns>Rod Action to be performed</returns>
        private RodAction EnterDecisionTreeBallInSector(Rod rod, BallCoordinates bfc, out int responsingPlayer)
        {
            //Stage 4 - get current ball relative position to rod
            eXPositionRodRelative xRelative = XPositionToRodXPostion(bfc.X, rod.RodType);

            //stage 9, 12, 5 - define player to move AND get current ball relative position to player
            eYPositionPlayerRelative yRelative = BallYPositionToPlayerYCoordinate(bfc.Y, rod, out responsingPlayer);

            //stage 10, 11, 13, 14, 6, 7 - current player rotational position
            eRotationalMove rotPos = _currentRodRotationPosition[rod.RodType];

            //stage 8 - decide on UTurn direction if needed
            eLinearMove UTurn = eLinearMove.NA;
            if (yRelative == eYPositionPlayerRelative.CENTER)
                UTurn = (IsEnoughSpaceToMove(rod, _currentRodYCoordinate[rod.RodType], BALL_RADIUS * 2)) ?
                    eLinearMove.RIGHT_BALL_DIAMETER : eLinearMove.LEFT_BALL_DIAMETER;

            return DecisionTreeBallInSector(rod.RodType, xRelative, yRelative, rotPos, UTurn);
        }

        /// <summary>
        /// Sub Decision Tree starting from Stage 4 in diagram
        /// </summary>
        /// <param name="rodType">Current rod type</param>
        /// <param name="xRelative">Current ball position relative to current rod (X coordinates)</param>
        /// <param name="yRelative">Current ball position relative to chosen player (Y coordinates)</param>
        /// <param name="rotPos">Current rod rotational state</param>
        /// <param name="UTurn">In case of UTurn needed - UTurn left/right</param>
        /// <returns>Rod Action to be performed for current rod</returns>
        private RodAction DecisionTreeBallInSector(eRod rodType, eXPositionRodRelative xRelative, eYPositionPlayerRelative yRelative, eRotationalMove rotPos, eLinearMove UTurn)
        {
            RodAction desiredAction = null;

            //Starting from Stage 4 in Decision Tree
            switch (xRelative)
            {
                case eXPositionRodRelative.FRONT:
                    #region Stage 9
                    switch (yRelative)
                    {
                        case eYPositionPlayerRelative.LEFT:
                        case eYPositionPlayerRelative.RIGHT:
                            #region Stage 10
                            switch (rotPos)
                            {
                                case eRotationalMove.RISE:
                                case eRotationalMove.DEFENCE:
                                    //Leafs 1, 2, 7, 8
                                    desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
                                    break;
                                case eRotationalMove.KICK:
                                    //Leafs 3, 9
                                    desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.NA);
                                    break;
                            }
                            break;
                            #endregion Stage 10
                        case eYPositionPlayerRelative.CENTER:
                            #region Stage 11
                            switch (rotPos)
                            {
                                case eRotationalMove.RISE:
                                case eRotationalMove.DEFENCE:
                                    //Leaf 4, 5
                                    desiredAction = new RodAction(rodType, eRotationalMove.KICK, eLinearMove.NA);
                                    break;
                                case eRotationalMove.KICK:
                                    //Stage 8 - (leaf 6)
                                    desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
                                    break;
                            }
                            break;
                            #endregion Stage 11
                    }
                    break;
                    #endregion Stage 9
                case eXPositionRodRelative.CENTER:
                    #region Stage 12
                    switch (yRelative)
                    {
                        case eYPositionPlayerRelative.LEFT:
                        case eYPositionPlayerRelative.RIGHT:
                            #region Stage 13
                            switch (rotPos)
                            {
                                case eRotationalMove.RISE:
                                    //Leaf 10, 16
                                    desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
                                    break;
                                case eRotationalMove.DEFENCE:
                                    //Leaf 11, 17
                                    desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
                                    break;
                                case eRotationalMove.KICK:
                                    //Leaf 12, 18
                                    desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.NA);
                                    break;
                            }
                            break;
                            #endregion Stage 13
                        case eYPositionPlayerRelative.CENTER:
                            #region Stage 14
                            switch (rotPos)
                            {
                                case eRotationalMove.RISE:
                                    //Leaf 13
                                    desiredAction = new RodAction(rodType, eRotationalMove.KICK, eLinearMove.NA);
                                    break;
                                case eRotationalMove.KICK:
                                    //Stage 8 - (Leaf 15)
                                    desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
                                    break;
                            }
                            break;
                            #endregion Stage 14
                    }
                    break;
                    #endregion Stage 12
                case eXPositionRodRelative.BACK:
                    #region Stage 5
                    switch (yRelative)
                    {
                        case eYPositionPlayerRelative.LEFT:
                        case eYPositionPlayerRelative.RIGHT:
                            #region Stage 6
                            switch (rotPos)
                            {
                                case eRotationalMove.RISE:
                                    //Leaf 19, 25
                                    desiredAction = new RodAction(rodType, eRotationalMove.NA, eLinearMove.BALL_Y);
                                    break;
                                case eRotationalMove.DEFENCE:
                                    //Leaf 20, 26
                                    desiredAction = new RodAction(rodType, eRotationalMove.RISE, eLinearMove.NA);
                                    break;
                                case eRotationalMove.KICK:
                                    //Leaf 21, 27
                                    desiredAction = new RodAction(rodType, eRotationalMove.DEFENCE, eLinearMove.NA);
                                    break;
                            }
                            break;
                            #endregion Stage 6
                        case eYPositionPlayerRelative.CENTER:
                            #region Stage 7
                            switch (rotPos)
                            {
                                case eRotationalMove.RISE:
                                    //Leaf 22
                                    desiredAction = new RodAction(rodType, eRotationalMove.KICK, eLinearMove.NA);
                                    break;
                                case eRotationalMove.DEFENCE:
                                    //Stage 8 - (Leaf 23)
                                    desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
                                    break;
                                case eRotationalMove.KICK:
                                    //Stage 8 - (Leaf 24)
                                    desiredAction = new RodAction(rodType, eRotationalMove.NA, UTurn);
                                    break;
                            }
                            break;
                            #endregion Stage 7
                    }
                    break;
                    #endregion Stage 5
            }

            return desiredAction;
        }

        /// <summary>
        /// Define Ball Y position relative to responding player of current rod and re define index of responding rod (as out)
        /// </summary>
        /// <param name="yBallCoordinate">Current ball Y coordinate</param>
        /// <param name="currentRod">Current rod</param>
        /// <param name="playerToResponse">Responding player[out] (index base is 0)</param>
        /// <returns>Ball Y position relative to responding player</returns>
        private eYPositionPlayerRelative BallYPositionToPlayerYCoordinate(int yBallCoordinate, Rod currentRod, out int playerToResponse)
        {
            //get array of players and their Y coordinates (player i stored in array index i - 1)
            int[] currentPlayerYs = AllCurrentPlayersYCoordinates(currentRod, _currentRodYCoordinate[currentRod.RodType]);

            //calculate movements for each player to reach the ball (Y Axe only)
            int[] movements = CallculatedYMovementForAllPlayers(currentPlayerYs, yBallCoordinate);

            //convert movement to distance on Y Axe (Absolute value)
            int[] absoluteDistance = movements.Select(x => Math.Abs(x)).ToArray();

            //get index of fist minimal distance (movement is here: movements[minIndexFirst])
            int minIndexFirst = Array.IndexOf(absoluteDistance, absoluteDistance.Min());

            //eliminate the first minimal distance
            absoluteDistance[minIndexFirst] = Y_AXE_SIZE * 100;

            //get index of second minimal distance (movement is here: movements[minIndexSecond])
            int minIndexSecond = Array.IndexOf(absoluteDistance, absoluteDistance.Min());

            //chosen movement to perform
            int movement = 0;

            //Define actual player to response 
            if (IsEnoughSpaceToMove(currentRod, _currentRodYCoordinate[currentRod.RodType], movements[minIndexFirst]))
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
        /// Get all players Y coordinates per current rod
        /// </summary>
        /// <param name="rodType">Current rod</param>
        /// <param name="rodCoordinate">Y Coordinate of current rod (stopper coordinate)</param>
        /// <returns>Array contains player Y in index of player number + 1
        /// <example>Player 1 Y coordinate is stored in array[0]</example>
        /// </returns>
        private int[] AllCurrentPlayersYCoordinates(Rod rod, int rodCoordinate)
        {
            int[] players = new int[rod.PlayersCount];
            for (int i = 0; i < rod.PlayersCount; i++)
            {
                players[i] = rodCoordinate + rod.OffsetY + i * rod.PlayerDistance;
            }
            return players;
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

        /// <summary>
        /// Verify if there is enough space to move the rod from current rod Y coordinate to new Y coordinate
        /// New Y coordinate is rod Y coordinate with provided movement (negative or positive)
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="currentRodYCoordinate">Current rod Y coordinate to move from</param>
        /// <param name="movement">Y delta to move from current rod Y coordinate (could be negative)</param>
        /// <returns>[True] in case there is enough space to move, [False] otherwise</returns>
        private bool IsEnoughSpaceToMove(Rod rod, int currentRodYCoordinate, int movement)
        {
            //Check if potential start of rod stopper is in range
            int potentialStartY = currentRodYCoordinate + movement;
            if (potentialStartY < ROD_START_Y)
                return false;

            //Check if potential end of rod stopper is in range
            int potentialEndY = potentialStartY + rod.StopperDistance;
            if (potentialEndY > ROD_END_Y)
                return false;

            //We are good, we have space to move!
            return true;
        }

        /// <summary>
        /// Define if ball is in sector
        /// </summary>
        /// <param name="ballXcoordinate">Current ball coordinate</param>
        /// <returns>Ball position relative to sector of current rod</returns>
        private eXPositionSectorRelative IsBallInSector(int ballXcoordinate)
        {
            if (ballXcoordinate < sectorStart)
                return eXPositionSectorRelative.BEHIND_SECTOR;
            else if (ballXcoordinate > sectorEnd)
                return eXPositionSectorRelative.AHEAD_SECTOR;
            else
                return eXPositionSectorRelative.IN_SECTOR;
        }

        /// <summary>
        /// Define ball vector angle is to rod or from it
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>[True] In case vector is to rod, [False] otherwise</returns>
        private bool IsBallVectorToRod(Vector2D vector)
        {
            return (vector != null && vector.IsDefined && vector.X < 0);
        }

        /// <summary>
        /// Calculate movements for each player to reach the ball (Y Axe only) in current rod
        /// </summary>
        /// <param name="currentPlayersYsCoordinates">Current players Y coordinates in current rod</param>
        /// <param name="yBallCoordinate">Current ball Y coordinate</param>
        /// <returns>Array of movements to be performed per each player to reach the ball</returns>
        private int [] CallculatedYMovementForAllPlayers(int [] currentPlayersYsCoordinates, int yBallCoordinate)
        {
            int[] movements = new int[currentPlayersYsCoordinates.Length];
            for (int i = 0; i < currentPlayersYsCoordinates.Length; i++)
            {
                movements[i] = yBallCoordinate - currentPlayersYsCoordinates[i];
            }
            return movements;
        }

        /// <summary>
        /// Calculate current Player Y coordinate
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="currentRodYCoordinate">Current Rod Y coordinates (stopper)</param>
        /// <param name="playerIndex">Chosen player index to perform action (index 1 based)</param>
        /// <returns>Chosen player Y coordinate</returns>
        private int CalculateCurrentPlayerYCoordinate(Rod rod, int currentRodYCoordinate, int playerIndex)
        {
            if (playerIndex > rod.PlayersCount || playerIndex < 1)
                throw new Exception(String.Format(
                    "Player index {0} for rod type {1} is wrong! Players count is {2}", 
                        playerIndex, rod.RodType, rod.PlayersCount));

            return rod.OffsetY + _currentRodYCoordinate[rod.RodType] + rod.PlayerDistance * (playerIndex - 1);
        }
    }
}
