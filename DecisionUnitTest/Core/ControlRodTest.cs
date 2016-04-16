// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Exceptions;
using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using Foosbot.DecisionUnit.Core;
using Foosbot.VectorCalculation.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnitTest.Core
{
    [TestClass]
    public class ControlRodTest
    {
        private const string CATEGORY = "ControlRod";

        const int SECTOR_X_COORDINATE = 195;
        const int SECTOR_MIN_WIDTH = 145;
        const double SECTOR_FACTOR = 0.3;
        const int PLAYER_DISTANCE = 225;
        const int PLAYER_COUNT = 2;
        const int OFFSET_Y = 25;
        const int STOPPER_DISTANCE = 275;
        const int BEST_EFFORT = 120;
        const int MAX_PREDICTION_TIME = 60;

        private static IInitializableRod _testAsset;

        private static ISurveyor _surveyorMock;
        private static IInitializableRicochet _ricochetMock;
        private static IRodState _stateMock;
        private static eRod _rodType;
        private const int HEIGHT = 800;
        private const int ROD_STOPPER_MINIMAL = 30;

        [ClassInitialize]
        public static void ControlRod_ClassInitialize(TestContext context)
        {
            _surveyorMock = Substitute.For<ISurveyor>();
            _ricochetMock = Substitute.For<IInitializableRicochet>();
            _stateMock = Substitute.For<IRodState>();
            _rodType = eRod.Defence;
        }

        [TestInitialize]
        public void ControlRod_TestInitialize()
        {

            _testAsset = new ControlRod(_rodType, _surveyorMock, _ricochetMock, HEIGHT, ROD_STOPPER_MINIMAL, _stateMock);
        }

        private void InitializeTestAsset()
        {
            _testAsset.Initialize(SECTOR_X_COORDINATE, SECTOR_MIN_WIDTH, SECTOR_FACTOR,
                PLAYER_DISTANCE, PLAYER_COUNT, OFFSET_Y, STOPPER_DISTANCE, BEST_EFFORT, MAX_PREDICTION_TIME);
        }

        #region Initializable Properties. Verify all properties throw InitializationException on get with no initialization

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void RodType_Negative_NotInitialized()
        {
            eRod result = _testAsset.RodType;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void PlayerDistance_Negative_NotInitialized()
        {
            int result = _testAsset.PlayerDistance;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void PlayerCount_Negative_NotInitialized()
        {
            int result = _testAsset.PlayerCount;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void OffsetY_Negative_NotInitialized()
        {
            int result = _testAsset.OffsetY;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void StopperDistance_Negative_NotInitialized()
        {
            int result = _testAsset.StopperDistance;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void RodXCoordinate_Negative_NotInitialized()
        {
            int result = _testAsset.RodXCoordinate;
        }
        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void MinSectorWidth_Negative_NotInitialized()
        {
            int result = _testAsset.MinSectorWidth;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void SectorFactor_Negative_NotInitialized()
        {
            double result = _testAsset.SectorFactor;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void BestEffort_Negative_NotInitialized()
        {
            eRod result = _testAsset.RodType;
        }

        #endregion Initializable Properties. Verify all properties throw InitializationException on get with no initialization

        #region Initialize + IsInitialized property

        [TestCategory(CATEGORY), TestMethod]
        public void IsInitialized_Positive_False()
        {
            bool result = _testAsset.IsInitialized;
            Assert.IsFalse(result);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsInitialized_Positive_True()
        {
            InitializeTestAsset();
            bool result = _testAsset.IsInitialized;
            Assert.IsTrue(result);
        }

        #endregion Initialize + IsInitialized property

        /*
         * AR Idan. Following functions need to implement Unit Tests:
         * 1. CalculateDynamicSector
         * 2. CalculateSectorIntersection
         * 3. NearestPossibleDcPosition
         * 
         * Guidlines:
         * 1. Test name convetions:
         *      a. Please use MethodTestName_PositiveOrNegative_ExplainInFewWords
         *      b. Please add TestCategory(CATEGORY) to each to easier sort tests
         *      c. Please add /// comment if test method is not trivial
         *      d. For single variable use "result", for calculations: "expected" and "actual"
         * 2. Use substitute extensions methods: http://nsubstitute.github.io/
         * 3. Use defined constants and mocks, only test class.
         * 4. Use ExpectedException tag if needed.
         * 5. Better to prepare test plan before starting.
         */

        #region NearestPossibleDcPosition

        [TestCategory(CATEGORY), TestMethod]
        public void NearestPossibleDcPosition_desiredPositionLessThanMinimumPossibleStartStopperY()
        {
            //arrange
            InitializeTestAsset();
            int desiredPosition = 20;
            int expected = ROD_STOPPER_MINIMAL;

            //act
            int actual = _testAsset.NearestPossibleDcPosition(desiredPosition);

            //assert
            Assert.AreEqual(expected, actual);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void NearestPossibleDcPosition_desiredPositionMoreThanMaximummPossibleStartStopperY()
        {
            //arrange
            InitializeTestAsset();
            int maximum = HEIGHT - STOPPER_DISTANCE - ROD_STOPPER_MINIMAL;
            int desiredPosition = maximum + 100;
            int expected = maximum;

            //act
            int actual = _testAsset.NearestPossibleDcPosition(desiredPosition);

            //assert
            Assert.AreEqual(expected, actual);

        }

        [TestCategory(CATEGORY), TestMethod]
        public void NearestPossibleDcPosition_desiredPositionInRange()
        {
            //arrange
            InitializeTestAsset();
            int maximum = HEIGHT - STOPPER_DISTANCE - ROD_STOPPER_MINIMAL;
            int desiredPosition = maximum - 100;
            int expected = desiredPosition;

            //act
            int actual = _testAsset.NearestPossibleDcPosition(desiredPosition);

            //assert
            Assert.AreEqual(expected, actual);

        }

        #endregion NearestPossibleDcPosition

        
            
        #region CalculateDynamicSector

       





        #endregion CalculateDynamicSector

    }

}
