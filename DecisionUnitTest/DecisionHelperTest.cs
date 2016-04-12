using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionUnitTest
{
   

    [TestClass]
    public class DecisionHelperTest
    {
        int rodYstart = 30;
        int rodYend = 640;
        DecisionHelper _treeHelper;

        static RodWrapper _rodGoalKeaper;
        static RodWrapper _rodMidfield;

        [ClassInitialize]
        public static void DecisionHelperTestInit(TestContext context)
        {
            _rodGoalKeaper = new RodWrapper(eRod.GoalKeeper, 50, 145, 2, 0, 1, 230, 460, 355);
            _rodMidfield = new RodWrapper(eRod.Midfield, 480, 145, 2, 15, 5, 25, 505, 120);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _treeHelper = new DecisionHelper(rodYstart, rodYend);
        }

        #region AllCurrentPlayersYCoordinates

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllCurrentPlayersYCoordinates_RodIsNull()
        {
            _treeHelper.AllCurrentPlayersYCoordinates(null, 50);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AllCurrentPlayersYCoordinates_VerifyPlayerCountOnRod_Zero()
        {
            RodWrapper rod = new RodWrapper(eRod.GoalKeeper, 0, 0, 0, 0, 0, 0, 0, 0);
            _treeHelper.AllCurrentPlayersYCoordinates(rod, 50);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AllCurrentPlayersYCoordinates_VerifyPlayerCountOnRod_Six()
        {
            RodWrapper rod = new RodWrapper(eRod.GoalKeeper, 0, 0, 0, 0, 6, 0, 0, 0);
            _treeHelper.AllCurrentPlayersYCoordinates(rod, 50);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AllCurrentPlayersYCoordinates_CoordinateOutOfRangeToSmall()
        {
            RodWrapper rod = new RodWrapper(eRod.GoalKeeper, 0, 0, 0, 0, 5, 0, 300, 0);
            _treeHelper.AllCurrentPlayersYCoordinates(rod, rodYstart - 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AllCurrentPlayersYCoordinates_CoordinateOutOfRangeToBig()
        {
            RodWrapper rod = new RodWrapper(eRod.GoalKeeper, 0, 0, 0, 0, 5, 0, 300, 0);
            _treeHelper.AllCurrentPlayersYCoordinates(rod, rodYend);
        }

        [TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveOne()
        {
            int playerCount = 1;
            int playerDistance = 0;
            int rodOffset = 30;
            int currentCoord = 80;
            RodWrapper rod = new RodWrapper(eRod.GoalKeeper, 0, 0, 0, playerDistance, playerCount, rodOffset, 500, 0);
            int[] result = _treeHelper.AllCurrentPlayersYCoordinates(rod, currentCoord);
            Assert.IsTrue(result.Length == playerCount);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset, result[0]);
        }

        [TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveTwo()
        {
            int playerCount = 1;
            int playerDistance = 0;
            int rodOffset = 30;
            int currentCoord = 40;
            RodWrapper rod = new RodWrapper(eRod.GoalKeeper, 0, 0, 0, playerDistance, playerCount, rodOffset, 500, 0);
            int[] result = _treeHelper.AllCurrentPlayersYCoordinates(rod, currentCoord);
            Assert.IsTrue(result.Length == playerCount);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset, result[0]);
        }

        [TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveThree()
        {
            int playerCount = 3;
            int playerDistance = 15;
            int rodOffset = 10;
            int currentCoord = 40;
            RodWrapper rod = new RodWrapper(eRod.Attack, 0, 0, 0, playerDistance, playerCount, rodOffset, 500, 0);
            int[] result = _treeHelper.AllCurrentPlayersYCoordinates(rod, currentCoord);
            Assert.IsTrue(result.Length == playerCount);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset, result[0]);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset + playerDistance, result[1]);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset + playerDistance + playerDistance, result[2]);
        }

        [TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveFour()
        {
            int playerCount = 3;
            int playerDistance = 15;
            int rodOffset = 10;
            int currentCoord = 80;
            RodWrapper rod = new RodWrapper(eRod.Attack, 0, 0, 0, playerDistance, playerCount, rodOffset, 500, 0);
            int[] result = _treeHelper.AllCurrentPlayersYCoordinates(rod, currentCoord);
            Assert.IsTrue(result.Length == playerCount);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset, result[0]);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset + playerDistance, result[1]);
            Assert.AreEqual(rodYstart + currentCoord + rodOffset + playerDistance + playerDistance, result[2]);
        }


        #endregion AllCurrentPlayersYCoordinates

        #region IsEnoughSpaceToMove

        [TestMethod]
        public void IsEnoughSpaceToMove_Positive_Move50()
        {
            Assert.IsTrue(_treeHelper.IsEnoughSpaceToMove(_rodGoalKeaper, 30, 50));
        }

        [TestMethod]
        public void IsEnoughSpaceToMove_Positive_Move140()
        {
            Assert.IsTrue(_treeHelper.IsEnoughSpaceToMove(_rodMidfield, 30, 105));
        }

        [TestMethod]
        public void IsEnoughSpaceToMove_Negative_MoveMinus31()
        {
            Assert.IsFalse(_treeHelper.IsEnoughSpaceToMove(_rodGoalKeaper, 60, -31));
        }

        [TestMethod]
        public void IsEnoughSpaceToMove_Negative_Move140()
        {
            Assert.IsFalse(_treeHelper.IsEnoughSpaceToMove(_rodMidfield, 30, 140));
        }

        #endregion IsEnoughSpaceToMove

        #region CalculatedYMovementForAllPlayers

        [TestMethod]
        public void CalculatedYMovementForAllPlayers_PositiveThreePlayers()
        {
            int ballY = 50;
            int [] currentPlayerYs = { 0, 20, 40, 60 };
            int [] result = _treeHelper.CalculateYMovementForAllPlayers(currentPlayerYs, ballY);
            Assert.AreEqual(currentPlayerYs.Length, result.Length);
            Assert.AreEqual(50, result[0]);
            Assert.AreEqual(30, result[1]);
            Assert.AreEqual(10, result[2]);
            Assert.AreEqual(-10, result[3]);
        }

        [TestMethod]
        public void CalculatedYMovementForAllPlayers_PositiveFivePlayers()
        {
            int ballY = 50;
            int[] currentPlayerYs = { 5, 20, 35, 50, 65 };
            int[] result = _treeHelper.CalculateYMovementForAllPlayers(currentPlayerYs, ballY);
            Assert.AreEqual(currentPlayerYs.Length, result.Length);
            Assert.AreEqual(45,  result[0]);
            Assert.AreEqual(30, result[1]);
            Assert.AreEqual(15, result[2]);
            Assert.AreEqual(0, result[3]);
            Assert.AreEqual(-15, result[4]);
        }

        #endregion CallculatedYMovementForAllPlayers

        #region IsBallVectorToRod

        [TestMethod]
        public void IsBallVectorToRod_Positive_VectorNull()
        {
            Assert.IsFalse(_treeHelper.IsBallVectorToRod(null));
        }

        [TestMethod]
        public void IsBallVectorToRod_Positive_VectorUndefined()
        {
            Assert.IsFalse(_treeHelper.IsBallVectorToRod(new Vector2D()));
        }

        [TestMethod]
        public void IsBallVectorToRod_Positive_VectorSpeed0()
        {
            Assert.IsFalse(_treeHelper.IsBallVectorToRod(new Vector2D(0, 0)));
        }

        [TestMethod]
        public void IsBallVectorToRod_Positive_VectorToRod()
        {
            Assert.IsTrue(_treeHelper.IsBallVectorToRod(new Vector2D(-10, -15)));
        }

        [TestMethod]
        public void IsBallVectorToRod_Positive_VectorFromRod()
        {
            Assert.IsFalse(_treeHelper.IsBallVectorToRod(new Vector2D(10, 15)));
        }

        #endregion IsBallVectorToRod

        #region IsBallInSector

        [TestMethod]
        public void IsBallInSector_Positive_Before()
        {
            eXPositionSectorRelative result = _treeHelper.IsBallInSector(-30, -29, 9);
            Assert.AreEqual(eXPositionSectorRelative.BEHIND_SECTOR, result);
        }

        [TestMethod]
        public void IsBallInSector_Positive_InSector()
        {
            eXPositionSectorRelative result = _treeHelper.IsBallInSector(5, -29, 9);
            Assert.AreEqual(eXPositionSectorRelative.IN_SECTOR, result);
        }

        [TestMethod]
        public void IsBallInSector_Positive_Ahead()
        {
            eXPositionSectorRelative result = _treeHelper.IsBallInSector(10, -29, 9);
            Assert.AreEqual(eXPositionSectorRelative.AHEAD_SECTOR, result);
        }

        #endregion IsBallInSector

        #region CalculateCurrentPlayerYCoordinate

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CalculateCurrentPlayerYCoordinate_PlayerIndexOutOfRangeToSmall()
        {
            _treeHelper.CalculateCurrentPlayerYCoordinate(_rodGoalKeaper, 100, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CalculateCurrentPlayerYCoordinate_PlayerIndexOutOfRangeToBig()
        {
            _treeHelper.CalculateCurrentPlayerYCoordinate(_rodMidfield, 100, 6);
        }

        [TestMethod]
        public void CalculateCurrentPlayerYCoordinate_PositivePlayerOneForGK()
        {
            int playerIndex = 1;
            int currentRodPos = 100;
            int expected = currentRodPos + _rodGoalKeaper.OffsetY + (playerIndex - 1) * _rodGoalKeaper.PlayerDistance;
            int actual = _treeHelper.CalculateCurrentPlayerYCoordinate(_rodGoalKeaper, currentRodPos, playerIndex);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculateCurrentPlayerYCoordinate_PositivePlayerFourForMF()
        {
            int playerIndex = 4;
            int currentRodPos = 40;
            int expected = currentRodPos + _rodMidfield.OffsetY + (playerIndex - 1) * _rodMidfield.PlayerDistance;
            int actual = _treeHelper.CalculateCurrentPlayerYCoordinate(_rodMidfield, currentRodPos, playerIndex);
            Assert.AreEqual(expected, actual);
        }

        #endregion CalculateCurrentPlayerYCoordinate

        #region VerifyYRodCoordinate

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VerifyYRodCoordinate_CoordinateOutOfRangeToSmall()
        {
            int stopperDist = 300;
            int coord = rodYstart - 10;
            _treeHelper.VerifyYRodCoordinate(stopperDist, coord);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VerifyYRodCoordinate_CoordinateOutOfRangeToBig()
        {
            int stopperDist = 300;
            int coord = rodYend - stopperDist + 1;
            _treeHelper.VerifyYRodCoordinate(stopperDist, coord);
        }

        [TestMethod]
        public void VerifyYRodCoordinate_Positive()
        {
            int stopperDist = 300;
            int coord = stopperDist - 10;
            _treeHelper.VerifyYRodCoordinate(stopperDist, coord);
        }

        #endregion VerifyYRodCoordinate
    }
}
