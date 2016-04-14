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
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Class contains common methods for decision tree
    /// </summary>
    public class DecisionHelper : IDecisionHelper
    {
        /// <summary>
        /// Constructor for DecisionHelper
        /// </summary>
        /// <param name="rodStartY">Rod start Y coordinate (Stopper)</param>
        /// <param name="rodEndY">Rod end Y coordinate (Stopper)</param>
        public DecisionHelper(int rodStartY, int rodEndY)
        {
            ROD_START_Y = rodStartY;
            ROD_END_Y = rodEndY;
        }

        /// <summary>
        /// Rod Start Y coordinate (Stopper Coordinate)
        /// </summary>
        public readonly int ROD_START_Y;

        /// <summary>
        /// Rod End Y coordinate (Stopper Coordinate)
        /// </summary>
        public readonly int ROD_END_Y;

        /// <summary>
        /// Get all players Y coordinates per current rod
        /// </summary>
        /// <param name="rodType">Current rod</param>
        /// <param name="rodCoordinate">Y Coordinate of current rod (stopper coordinate)</param>
        /// <returns>Array contains player Y in index of player number + 1
        /// <example>Player 1 Y coordinate is stored in array[0]</example>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown in case rod is null</exception>
        public int[] AllCurrentPlayersYCoordinates(IRod rod, int rodCoordinate)
        {
            if (rod == null)
                throw new ArgumentNullException(String.Format(
                    "[{0}] Unable to calculate all players Y coordinates on rod because rod is NULL!",
                        MethodBase.GetCurrentMethod().Name));

            VerifyPlayerCountOnRod(rod);
            VerifyYRodCoordinate(rod.StopperDistance, rodCoordinate);

            int[] players = new int[rod.PlayerCount];
            for (int i = 0; i < rod.PlayerCount; i++)
            {
                players[i] = rodCoordinate + rod.OffsetY + i * rod.PlayerDistance;
            }
            return players;
        }

        /// <summary>
        /// Verify if there is enough space to move the rod from current rod Y coordinate to new Y coordinate
        /// New Y coordinate is rod Y coordinate with provided movement (negative or positive)
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="currentRodYCoordinate">Current rod Y coordinate to move from, including ROD_START_Y </param>
        /// <param name="movement">Y delta to move from current rod Y coordinate (could be negative)</param>
        /// <returns>[True] in case there is enough space to move, [False] otherwise</returns>
        public bool IsEnoughSpaceToMove(IRod rod, int currentRodYCoordinate, int movement)
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
        /// Calculate movements for each player to reach the ball (Y Axe only) in current rod
        /// </summary>
        /// <param name="currentPlayersYsCoordinates">Current players Y coordinates in current rod</param>
        /// <param name="yBallCoordinate">Current ball Y coordinate</param>
        /// <returns>Array of movements to be performed per each player to reach the ball</returns>
        public int[] CalculateYMovementForAllPlayers(int[] currentPlayersYsCoordinates, int yBallCoordinate)
        {
            int[] movements = new int[currentPlayersYsCoordinates.Length];
            for (int i = 0; i < currentPlayersYsCoordinates.Length; i++)
            {
                movements[i] = yBallCoordinate - currentPlayersYsCoordinates[i];
            }
            return movements;
        }

        /// <summary>
        /// Define ball vector angle is to rod or from it
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>[True] In case vector is to rod, [False] otherwise</returns>
        public bool IsBallVectorToRod(Vector2D vector)
        {
            return (vector != null && vector.IsDefined && vector.X < 0);
        }


        /// <summary>
        /// Define if ball is in sector
        /// </summary>
        /// <param name="ballXcoordinate">Current ball coordinate</param>
        /// <param name="sectorStart">Rod X Coordinate</param>
        /// <param name="sectorEnd">Dynamic Sector Width</param>
        /// <returns>Ball position relative to sector of current rod</returns>
        public eXPositionSectorRelative IsBallInSector(int ballXcoordinate, int rodCoordinates, int dynamicSectorWidth)
        {
            int sectorStart = Convert.ToInt32(rodCoordinates - dynamicSectorWidth / 2.0);
            int sectorEnd = Convert.ToInt32(rodCoordinates + dynamicSectorWidth / 2.0);
            if (ballXcoordinate < sectorStart)
                return eXPositionSectorRelative.BEHIND_SECTOR;
            else if (ballXcoordinate > sectorEnd)
                return eXPositionSectorRelative.AHEAD_SECTOR;
            else
                return eXPositionSectorRelative.IN_SECTOR;
        }

        /// <summary>
        /// Calculate current Player Y coordinate
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="currentRodYCoordinate">Current Rod Y coordinates (stopper)</param>
        /// <param name="playerIndex">Chosen player index to perform action (index 1 based)</param>
        /// <returns>Chosen player Y coordinate</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown in case player index is out of range</exception>
        public int CalculateCurrentPlayerYCoordinate(IRod rod, int currentRodYCoordinate, int playerIndex)
        {
            if (playerIndex > rod.PlayerCount || playerIndex < 1)
                throw new ArgumentOutOfRangeException(String.Format(
                    "Player index {0} for rod type {1} is wrong! Players count is {2}",
                        playerIndex, rod.RodType, rod.PlayerCount));

            return rod.OffsetY + currentRodYCoordinate + rod.PlayerDistance * (playerIndex - 1);
        }

        /// <summary>
        /// Verify Y Rod coordinate is in range
        /// </summary>
        /// <param name="rodStopperDistance">Distance beetween two stoppers of rod</param>
        /// <param name="coordinateY">Current Y coordinate</param>
        /// <exception cref="ArgumentOutOfRangeException">In case current Y coordinate is not in range</exception>
        public void VerifyYRodCoordinate(int rodStopperDistance, int coordinateY)
        {
            int min = ROD_START_Y;
            int max = ROD_END_Y - rodStopperDistance;
            if (coordinateY < min || coordinateY > max)
                throw new ArgumentOutOfRangeException(String.Format("[{0}] Rod Y coordinate {1} is out of range {2} to {3}!",
                    MethodBase.GetCurrentMethod().Name, coordinateY, min, max));
        }

        /// <summary>
        /// Get Y coordinate of rod stopper to bring responding player to desired Y coordinate
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="desiredY">Desired Y coordinate to reach by responding player</param>
        /// <param name="respondingPlayer">One-Based Player Index in rod</param>
        /// <returns>Y coordinate of rod stopper to bring palyer to desired Y coordinate</returns>
        public int LocateRespondingPlayer(IRod rod, int desiredY, int respondingPlayer)
        {
            return desiredY - rod.OffsetY - rod.PlayerDistance * (respondingPlayer - 1);
        }

        #region private methods

        /// <summary>
        /// Verify Player Count on given rod is beetween 1 to 5.
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <exception cref="ArgumentException">Thrown in case player count on rod is not in range of 1 to 5</exception>
        private void VerifyPlayerCountOnRod(IRod rod)
        {
            if (rod.PlayerCount < 1 || rod.PlayerCount > 5)
                throw new ArgumentException(String.Format("[{0}] Number of players on rod {1} is incorrect {2}!",
                    MethodBase.GetCurrentMethod().Name, rod.RodType.ToString(), rod.PlayerCount));
        }


        #endregion private methods
    }
}
