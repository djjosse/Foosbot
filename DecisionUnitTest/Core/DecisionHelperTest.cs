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
using Foosbot.DecisionUnit;
using Foosbot.DecisionUnit.Core;
using Foosbot.DecisionUnit.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Foosbot.DecisionUnit.Enums;
using Foosbot.Common.Enums;

namespace Foosbot.DecisionUnitTest.Core
{
    [TestClass]
    public class DecisionHelperTest
    {
        private const string CATEGORY = "DecisionHelper";

        private const int ROD_Y_START = 30;
        private const int ROD_Y_END = 640;
        private static IDecisionHelper _testAsset;

        private static IInitializableRod _rodGoalKeaper;
        private static IInitializableRod _rodMidfield;

        [ClassInitialize]
        public static void DecisionHelperTestInit(TestContext context)
        {
            _rodGoalKeaper = Substitute.For<IInitializableRod>();
            _rodMidfield = Substitute.For<IInitializableRod>();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            //(eRod.GoalKeeper, 50, 145, 2, 0, 1, 230, 460, 355);
            _rodGoalKeaper.RodType.ReturnsForAnyArgs(eRod.GoalKeeper);
            _rodGoalKeaper.RodXCoordinate.ReturnsForAnyArgs(50);
            _rodGoalKeaper.MinSectorWidth.ReturnsForAnyArgs(145);
            _rodGoalKeaper.SectorFactor.ReturnsForAnyArgs(2);
            _rodGoalKeaper.PlayerDistance.ReturnsForAnyArgs(0);
            _rodGoalKeaper.PlayerCount.ReturnsForAnyArgs(1);
            _rodGoalKeaper.OffsetY.ReturnsForAnyArgs(230);
            _rodGoalKeaper.StopperDistance.ReturnsForAnyArgs(460);
            _rodGoalKeaper.BestEffort.ReturnsForAnyArgs(355);

            //(eRod.Midfield, 480, 145, 2, 15, 5, 25, 505, 120);
            _rodMidfield.RodType.ReturnsForAnyArgs(eRod.Midfield);
            _rodMidfield.RodXCoordinate.ReturnsForAnyArgs(480);
            _rodMidfield.MinSectorWidth.ReturnsForAnyArgs(145);
            _rodMidfield.SectorFactor.ReturnsForAnyArgs(2);
            _rodMidfield.PlayerDistance.ReturnsForAnyArgs(15);
            _rodMidfield.PlayerCount.ReturnsForAnyArgs(5);
            _rodMidfield.OffsetY.ReturnsForAnyArgs(25);
            _rodMidfield.StopperDistance.ReturnsForAnyArgs(505);
            _rodMidfield.BestEffort.ReturnsForAnyArgs(120);

            _testAsset = new DecisionHelper(ROD_Y_START, ROD_Y_END);
        }

        #region AllCurrentPlayersYCoordinates

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllCurrentPlayersYCoordinates_RodIsNull()
        {
            _testAsset.AllCurrentPlayersYCoordinates(null, 50);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AllCurrentPlayersYCoordinates_VerifyPlayerCountOnRod_Zero()
        {
            _rodGoalKeaper.PlayerCount.ReturnsForAnyArgs(0);
            _testAsset.AllCurrentPlayersYCoordinates(_rodGoalKeaper, 50);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AllCurrentPlayersYCoordinates_VerifyPlayerCountOnRod_Six()
        {
            _rodGoalKeaper.PlayerCount.ReturnsForAnyArgs(0);
            _testAsset.AllCurrentPlayersYCoordinates(_rodGoalKeaper, 50);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AllCurrentPlayersYCoordinates_CoordinateOutOfRangeToSmall()
        {
            _testAsset.AllCurrentPlayersYCoordinates(_rodGoalKeaper, ROD_Y_START - 10);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AllCurrentPlayersYCoordinates_CoordinateOutOfRangeToBig()
        {
            _testAsset.AllCurrentPlayersYCoordinates(_rodGoalKeaper, ROD_Y_END);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveOne()
        {
            _rodGoalKeaper.StopperDistance.ReturnsForAnyArgs(500);
            _rodGoalKeaper.OffsetY.ReturnsForAnyArgs(30);

            int currentCoord = 80;
            int[] result = _testAsset.AllCurrentPlayersYCoordinates(_rodGoalKeaper, currentCoord);
            Assert.IsTrue(result.Length == _rodGoalKeaper.PlayerCount);
            Assert.AreEqual(currentCoord + _rodGoalKeaper.OffsetY, result[0]);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveTwo()
        {
            _rodGoalKeaper.StopperDistance.ReturnsForAnyArgs(500);
            _rodGoalKeaper.OffsetY.ReturnsForAnyArgs(30);

            int currentCoord = 40;
            int[] result = _testAsset.AllCurrentPlayersYCoordinates(_rodGoalKeaper, currentCoord);
            Assert.IsTrue(result.Length == _rodGoalKeaper.PlayerCount);
            Assert.AreEqual(currentCoord + _rodGoalKeaper.OffsetY, result[0]);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveThree()
        {
            _rodMidfield.RodType.ReturnsForAnyArgs(eRod.Attack);
            _rodMidfield.StopperDistance.ReturnsForAnyArgs(500);
            _rodMidfield.OffsetY.ReturnsForAnyArgs(10);
            _rodMidfield.PlayerCount.ReturnsForAnyArgs(3);
            _rodMidfield.PlayerDistance.ReturnsForAnyArgs(15);

            int currentCoord = 40;

            int[] result = _testAsset.AllCurrentPlayersYCoordinates(_rodMidfield, currentCoord);
            Assert.IsTrue(result.Length == _rodMidfield.PlayerCount);
            for (int i = 0; i < _rodMidfield.PlayerCount; i++ )
            {
                int expected = currentCoord + _rodMidfield.OffsetY + (i * _rodMidfield.PlayerDistance);
                Assert.AreEqual(expected, result[i]);
            }
        }

        [TestCategory(CATEGORY), TestMethod]
        public void AllCurrentPlayersYCoordinates_PositiveFour()
        {
            _rodMidfield.RodType.ReturnsForAnyArgs(eRod.Attack);
            _rodMidfield.StopperDistance.ReturnsForAnyArgs(500);
            _rodMidfield.OffsetY.ReturnsForAnyArgs(10);
            _rodMidfield.PlayerCount.ReturnsForAnyArgs(3);
            _rodMidfield.PlayerDistance.ReturnsForAnyArgs(15);

            int currentCoord = 80;

            int[] result = _testAsset.AllCurrentPlayersYCoordinates(_rodMidfield, currentCoord);
            Assert.IsTrue(result.Length == _rodMidfield.PlayerCount);
            for (int i = 0; i < _rodMidfield.PlayerCount; i++)
            {
                int expected = currentCoord + _rodMidfield.OffsetY + (i * _rodMidfield.PlayerDistance);
                Assert.AreEqual(expected, result[i]);
            }
        }

        #endregion AllCurrentPlayersYCoordinates

        #region IsEnoughSpaceToMove

        [TestCategory(CATEGORY), TestMethod]
        public void IsEnoughSpaceToMove_Positive_Move50()
        {
            Assert.IsTrue(_testAsset.IsEnoughSpaceToMove(_rodGoalKeaper, 30, 50));
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsEnoughSpaceToMove_Positive_Move140()
        {
            Assert.IsTrue(_testAsset.IsEnoughSpaceToMove(_rodMidfield, 30, 105));
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsEnoughSpaceToMove_Negative_MoveMinus31()
        {
            Assert.IsFalse(_testAsset.IsEnoughSpaceToMove(_rodGoalKeaper, 60, -31));
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsEnoughSpaceToMove_Negative_Move140()
        {
            Assert.IsFalse(_testAsset.IsEnoughSpaceToMove(_rodMidfield, 30, 140));
        }

        #endregion IsEnoughSpaceToMove

        #region CalculatedYMovementForAllPlayers

        [TestCategory(CATEGORY), TestMethod]
        public void CalculatedYMovementForAllPlayers_PositiveThreePlayers()
        {
            int ballY = 50;
            int[] currentPlayerYs = { 0, 20, 40, 60 };
            int[] result = _testAsset.CalculateYMovementForAllPlayers(currentPlayerYs, ballY);
            Assert.AreEqual(currentPlayerYs.Length, result.Length);
            Assert.AreEqual(50, result[0]);
            Assert.AreEqual(30, result[1]);
            Assert.AreEqual(10, result[2]);
            Assert.AreEqual(-10, result[3]);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void CalculatedYMovementForAllPlayers_PositiveFivePlayers()
        {
            int ballY = 50;
            int[] currentPlayerYs = { 5, 20, 35, 50, 65 };
            int[] result = _testAsset.CalculateYMovementForAllPlayers(currentPlayerYs, ballY);
            Assert.AreEqual(currentPlayerYs.Length, result.Length);
            Assert.AreEqual(45, result[0]);
            Assert.AreEqual(30, result[1]);
            Assert.AreEqual(15, result[2]);
            Assert.AreEqual(0, result[3]);
            Assert.AreEqual(-15, result[4]);
        }

        #endregion CallculatedYMovementForAllPlayers

        #region IsBallVectorToRod

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallVectorToRod_Positive_VectorNull()
        {
            Assert.IsFalse(_testAsset.IsBallVectorToRod(null));
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallVectorToRod_Positive_VectorUndefined()
        {
            Assert.IsFalse(_testAsset.IsBallVectorToRod(new Vector2D()));
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallVectorToRod_Positive_VectorSpeed0()
        {
            Assert.IsFalse(_testAsset.IsBallVectorToRod(new Vector2D(0, 0)));
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallVectorToRod_Positive_VectorToRod()
        {
            Assert.IsTrue(_testAsset.IsBallVectorToRod(new Vector2D(-10, -15)));
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallVectorToRod_Positive_VectorFromRod()
        {
            Assert.IsFalse(_testAsset.IsBallVectorToRod(new Vector2D(10, 15)));
        }

        #endregion IsBallVectorToRod

        #region IsBallInSector

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallInSector_Positive_Before()
        {
            eXPositionSectorRelative result = _testAsset.IsBallInSector(-30, -10, 20);
            Assert.AreEqual(eXPositionSectorRelative.BEHIND_SECTOR, result);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallInSector_Positive_InSector()
        {
            eXPositionSectorRelative result = _testAsset.IsBallInSector(-5, -10, 20);
            Assert.AreEqual(eXPositionSectorRelative.IN_SECTOR, result);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsBallInSector_Positive_Ahead()
        {
            eXPositionSectorRelative result = _testAsset.IsBallInSector(20, -10, 20);
            Assert.AreEqual(eXPositionSectorRelative.AHEAD_SECTOR, result);
        }

        #endregion IsBallInSector  

        #region VerifyYRodCoordinate

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VerifyYRodCoordinate_CoordinateOutOfRangeToSmall()
        {
            int stopperDist = 300;
            int coord = ROD_Y_START - 10;
            _testAsset.VerifyYRodCoordinate(stopperDist, coord);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VerifyYRodCoordinate_CoordinateOutOfRangeToBig()
        {
            int stopperDist = 300;
            int coord = ROD_Y_END - stopperDist + 1;
            _testAsset.VerifyYRodCoordinate(stopperDist, coord);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void VerifyYRodCoordinate_Positive()
        {
            int stopperDist = 300;
            int coord = stopperDist - 10;
            _testAsset.VerifyYRodCoordinate(stopperDist, coord);
        }

        #endregion VerifyYRodCoordinate

        /*
         * 
         * Following Tests are currently not in use. Will be restored after completition of decision tree.
         * 
         */

        #region CalculateCurrentPlayerYCoordinate

        //[TestCategory(CATEGORY), TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void CalculateCurrentPlayerYCoordinate_PlayerIndexOutOfRangeToSmall()
        //{
        //    _testAsset.CalculateCurrentPlayerYCoordinate(_rodGoalKeaper, 100, 0);
        //}

        //[TestCategory(CATEGORY), TestMethod]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void CalculateCurrentPlayerYCoordinate_PlayerIndexOutOfRangeToBig()
        //{
        //    _testAsset.CalculateCurrentPlayerYCoordinate(_rodMidfield, 100, 6);
        //}

        //[TestCategory(CATEGORY), TestMethod]
        //public void CalculateCurrentPlayerYCoordinate_PositivePlayerOneForGK()
        //{
        //    int playerIndex = 1;
        //    int currentRodPos = 100;
        //    int expected = currentRodPos + _rodGoalKeaper.OffsetY + (playerIndex - 1) * _rodGoalKeaper.PlayerDistance;
        //    int actual = _testAsset.CalculateCurrentPlayerYCoordinate(_rodGoalKeaper, currentRodPos, playerIndex);
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestCategory(CATEGORY), TestMethod]
        //public void CalculateCurrentPlayerYCoordinate_PositivePlayerFourForMF()
        //{
        //    int playerIndex = 4;
        //    int currentRodPos = 40;
        //    int expected = currentRodPos + _rodMidfield.OffsetY + (playerIndex - 1) * _rodMidfield.PlayerDistance;
        //    int actual = _testAsset.CalculateCurrentPlayerYCoordinate(_rodMidfield, currentRodPos, playerIndex);
        //    Assert.AreEqual(expected, actual);
        //}

        #endregion CalculateCurrentPlayerYCoordinate
    }
}
