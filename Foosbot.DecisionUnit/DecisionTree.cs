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
        /// Current Y coordinate of current rod (stopper) per each rod
        /// </summary>
        private Dictionary<eRod, int> _currentRodYCoordinate;

        /// <summary>
        /// Current rod rotation position
        /// </summary>
        private Dictionary<eRod, eRotationalMove> _currentRodRotationPosition;

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
            foreach(eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                int xCoord = Configuration.Attributes.GetRodXCoordinate(rodType);
                _rodXCoordinate.Add(rodType, xCoord);
                _currentRodYCoordinate.Add(rodType, 0);
                _currentRodRotationPosition.Add(rodType, eRotationalMove.DEFENCE);
            }

            Y_AXE_SIZE = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            ROD_START_Y = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_START_Y);
            ROD_END_Y = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_END_Y);
            PLAYER_WIDTH = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_PLAYER_WIDTH);
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
                    action = SubTreeBallInSector(rod, bfc);
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
        private RodAction SubTreeBallInSector(Rod rod, BallCoordinates bfc)
        {
            //Stage 4 - get current ball relative position to rod
            eXPositionRodRelative xRelative = XPositionToRodXPostion(bfc.X, rod.RodType);

            //stage 9, 12, 5 - define player to move AND get current ball relative position to player
            int responsingPlayer;
            eYPositionPlayerRelative yRelative = YPositionToPlayerYPosition(bfc.Y, rod, out responsingPlayer);

            //stage 10, 11, 13, 14, 6, 7 - current player rotational position
            eRotationalMove rotPos = _currentRodRotationPosition[rod.RodType];

            //stage 8 - choose if enough space to move by ball diameter
            //ToDo:...

            //ToDo: Create swich and define complete rod action and return it
            return null;
        }

        /// <summary>
        /// Define Ball Y position relative to responding player of current rod and re define index of responding rod (as out)
        /// </summary>
        /// <param name="yBallCoordinate">Current ball Y coordinate</param>
        /// <param name="currentRod">Current rod</param>
        /// <param name="playerToResponse">Responding player[out]</param>
        /// <returns>Ball Y position relative to responding player</returns>
        private eYPositionPlayerRelative YPositionToPlayerYPosition(int yBallCoordinate, Rod currentRod, out int playerToResponse)
        {
            //get array of players and their Y coordinates (player i stored in array index i - 1)
            int[] currentPlayerYs = CurrentPlayerYCoordinate(currentRod, _currentRodYCoordinate[currentRod.RodType]);

            //calculate movements for each player to reach the ball (Y Axe only)
            int[] movements = new int[currentPlayerYs.Length];
            for(int i = 0; i<currentPlayerYs.Length;i++)
            {
                movements[i] = yBallCoordinate - currentPlayerYs[i];
            }

            //convert movement to distance on Y Axe (Absolute value)
            int[] absoluteDistance = movements.Select(x => Math.Abs(x)).ToArray();

            //get index of fist minimal distance
            int minIndexFirst = Array.IndexOf(absoluteDistance, absoluteDistance.Min());
            //get value (movement) for first minimal movement
            int minFirst = movements[minIndexFirst];

            //eliminate the first minimal distance
            absoluteDistance[minIndexFirst] = Y_AXE_SIZE * 100;

            //get index of second minimal distance
            int minIndexSecond = Array.IndexOf(absoluteDistance, absoluteDistance.Min()); 
            //get value (movement) for second minimal movement
            int minSecond = movements[minIndexSecond];

            //chosen movement to perform
            int movement = 0;

            //Define actual player to response 
            if (IsEnoughSpaceToMove(currentRod, _currentRodYCoordinate[currentRod.RodType], minFirst))
            {
                //as index starts from 0 => first one is 1
                playerToResponse = minIndexFirst + 1;
                movement = minFirst;
            }
            else
            {
                //as index starts from 0 => first one is 1
                playerToResponse = minIndexSecond + 1;
                movement = minSecond;
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
        private int[] CurrentPlayerYCoordinate(Rod rod, int rodCoordinate)
        {
            int[] players = new int[rod.PlayersCount];
            for (int i = 0; i <= rod.PlayersCount; i++)
            {
                int playerY = rodCoordinate + rod.OffsetY + i * rod.PlayerDistance;
                players[i] = playerY;
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
            return (vector.X < 0);
        }

    }
}
