using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit
{
    public interface IDecisionHelper
    {

        /// <summary>
        /// Get all players Y coordinates per current rod
        /// </summary>
        /// <param name="rodType">Current rod</param>
        /// <param name="rodCoordinate">Y Coordinate of current rod (stopper coordinate)</param>
        /// <returns>Array contains player Y in index of player number + 1
        /// <example>Player 1 Y coordinate is stored in array[0]</example>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown in case rod is null</exception>
        int[] AllCurrentPlayersYCoordinates(Rod rod, int rodCoordinate);

        /// <summary>
        /// Verify if there is enough space to move the rod from current rod Y coordinate to new Y coordinate
        /// New Y coordinate is rod Y coordinate with provided movement (negative or positive)
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="currentRodYCoordinate">Current rod Y coordinate to move from, including ROD_START_Y </param>
        /// <param name="movement">Y delta to move from current rod Y coordinate (could be negative)</param>
        /// <returns>[True] in case there is enough space to move, [False] otherwise</returns>
        bool IsEnoughSpaceToMove(Rod rod, int currentRodYCoordinate, int movement);

        /// <summary>
        /// Calculate movements for each player to reach the ball (Y Axe only) in current rod
        /// </summary>
        /// <param name="currentPlayersYsCoordinates">Current players Y coordinates in current rod</param>
        /// <param name="yBallCoordinate">Current ball Y coordinate</param>
        /// <returns>Array of movements to be performed per each player to reach the ball</returns>
        int[] CalculateYMovementForAllPlayers(int[] currentPlayersYsCoordinates, int yBallCoordinate);

        /// <summary>
        /// Define ball vector angle is to rod or from it
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>[True] In case vector is to rod, [False] otherwise</returns>
        bool IsBallVectorToRod(Vector2D vector);

        /// <summary>
        /// Define if ball is in sector
        /// </summary>
        /// <param name="ballXcoordinate">Current ball coordinate</param>
        /// <param name="sectorStart">Sector start X</param>
        /// <param name="sectorEnd">Sector end X</param>
        /// <returns>Ball position relative to sector of current rod</returns>
        eXPositionSectorRelative IsBallInSector(int ballXcoordinate, int sectorStart, int sectorEnd);

        /// <summary>
        /// Calculate current Player Y coordinate
        /// </summary>
        /// <param name="rod">Current rod</param>
        /// <param name="currentRodYCoordinate">Current Rod Y coordinates (stopper)</param>
        /// <param name="playerIndex">Chosen player index to perform action (index 1 based)</param>
        /// <returns>Chosen player Y coordinate</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown in case player index is out of range</exception>
        int CalculateCurrentPlayerYCoordinate(Rod rod, int currentRodYCoordinate, int playerIndex);

        /// <summary>
        /// Verify Y Rod coordinate is in range
        /// </summary>
        /// <param name="rodStopperDistance">Distance beetween two stoppers of rod</param>
        /// <param name="coordinateY">Current Y coordinate</param>
        /// <exception cref="ArgumentOutOfRangeException">In case current Y coordinate is not in range</exception>
        void VerifyYRodCoordinate(int rodStopperDistance, int coordinateY);
    }
}
