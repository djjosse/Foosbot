// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

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

namespace DecisionUnitTest.Core
{
    [TestClass]
    public class PredictorTests
    {
        private const string CATEGORY = "Predictor";

        private static IPredictor _testAsset;

        private static ISurveyor _surveyorMock;
        private static IInitializableRicochet _ricochetMock;

        [ClassInitialize]
        public static void ControlRod_ClassInitialize(TestContext context)
        {
            _surveyorMock = Substitute.For<ISurveyor>();
            _ricochetMock = Substitute.For<IInitializableRicochet>();
        }

        [TestInitialize]
        public void ControlRod_TestInitialize()
        {
            _testAsset = new Predictor(_surveyorMock, _ricochetMock);
        }

        /*
         * AR Idan: Please test following methods:
         * 1. FindBallFutureCoordinates
         * 
         * Guidlines can be found in ControlRodTests Class.
         * Attached following tests code which is not good 
         * now because we haven't used mocks. 
         * It should be simplier test now also.
         *
         * 
         */

        //    [TestClass]
        //    public class DecisionTest
        //    {
        //        private static int XMAX_PTS;
        //        private static int YMAX_PTS;
        //        private static int XMAX_MM;
        //        private static int YMAX_MM;
        //        private static double RICOCHET_FACTOR;

        //        private static Publisher<BallCoordinates> _mockPublisher;
        //        private DecisionWrapper _decision;

        //        [ClassInitialize]
        //        public static void DecisionTestInitialize(TestContext context)
        //        {
        //            _mockPublisher = Substitute.For<Publisher<BallCoordinates>>();

        //            XMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
        //            YMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
        //            XMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
        //            YMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);

        //            RICOCHET_FACTOR = Configuration.Attributes.GetValue<double>(Configuration.Names.KEY_RICOCHET_FACTOR);
        //        }

        //        [TestInitialize]
        //        public void InitializeTest()
        //        {
        //            _decision = new DecisionWrapper(_mockPublisher);
        //        }

        //        #region FindBallFutureCoordinates Test

        //        [TestMethod]
        //        [ExpectedException(typeof(ArgumentException))]
        //        public void FindBallFutureCoordinates_BallCoordinatesNull()
        //        {
        //            _decision.FindBallFutureCoordinates(null, DateTime.Now);
        //        }

        //        [TestMethod]
        //        [ExpectedException(typeof(ArgumentException))]
        //        public void FindBallFutureCoordinates_BallCoordinatesUndefined()
        //        {
        //            _decision.FindBallFutureCoordinates(new BallCoordinates(DateTime.Now), DateTime.Now);
        //        }

        //        [TestMethod]
        //        public void FindBallFutureCoordinates_VectorNull()
        //        {
        //            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, DateTime.Now);
        //            DateTime actionTime = DateTime.Now + TimeSpan.FromSeconds(5);
        //            BallCoordinates actualCoordinates = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            Assert.AreEqual(currentCoordinates.X, actualCoordinates.X);
        //            Assert.AreEqual(currentCoordinates.Y, actualCoordinates.Y);
        //            Assert.AreEqual(actionTime, actualCoordinates.Timestamp);
        //            Assert.IsNull(actualCoordinates.Vector);
        //        }

        //        [TestMethod]
        //        public void FindBallFutureCoordinates_VectorUndefined()
        //        {
        //            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, DateTime.Now);
        //            currentCoordinates.Vector = new Vector2D();
        //            DateTime actionTime = DateTime.Now + TimeSpan.FromSeconds(5);
        //            BallCoordinates actualCoordinates = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            Assert.AreEqual(currentCoordinates.X, actualCoordinates.X);
        //            Assert.AreEqual(currentCoordinates.Y, actualCoordinates.Y);
        //            Assert.AreEqual(actionTime, actualCoordinates.Timestamp);
        //            Assert.IsFalse(actualCoordinates.Vector.IsDefined);
        //        }

        //        [TestMethod]
        //        public void FindBallFutureCoordinates_VectorSpeedZero()
        //        {
        //            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, DateTime.Now);
        //            currentCoordinates.Vector = new Vector2D(0, 0);
        //            DateTime actionTime = DateTime.Now + TimeSpan.FromSeconds(5);
        //            BallCoordinates actualCoordinates = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            Assert.AreEqual(currentCoordinates.X, actualCoordinates.X);
        //            Assert.AreEqual(currentCoordinates.Y, actualCoordinates.Y);
        //            Assert.AreEqual(actionTime, actualCoordinates.Timestamp);
        //            Assert.AreEqual(currentCoordinates.Vector.X, actualCoordinates.Vector.X);
        //            Assert.AreEqual(currentCoordinates.Vector.Y, actualCoordinates.Vector.Y);
        //        }

        //        [TestMethod]
        //        [ExpectedException(typeof(ArgumentException))]
        //        public void FindBallFutureCoordinates_ActionTimeIsEarlierThanTimeStamp()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, timeStamp);
        //            DateTime actionTime = timeStamp - TimeSpan.FromSeconds(5);
        //            _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //        }

        //        /// <summary>
        //        /// Current Coordinates: 90, 60, time - now; Vector 50 50
        //        /// Actual Time - current coordinates time stamp + 5 sec
        //        /// Expected Coordinates: 340, 310, time is actual time; Vector 50 50
        //        /// </summary>
        //        [TestMethod]
        //        public void FindBallFutureCoordinates_NoRicochetTestOne()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(90, 60, timeStamp);
        //            currentCoordinates.Vector = new Vector2D(50, 50);
        //            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(5);
        //            BallCoordinates actualResult = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            Assert.AreEqual(actualResult.X, 340);
        //            Assert.AreEqual(actualResult.Y, 310);
        //            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
        //            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
        //            Assert.AreEqual(actualResult.Timestamp, actionTime);
        //        }

        //        /// <summary>
        //        /// Current Coordinates: 700, 650, time - now; Vector -30 -40
        //        /// Actual Time - current coordinates time stamp + 4 sec
        //        /// Expected Coordinates: 580, 490, time is actual time; Vector -30 -40
        //        /// </summary>
        //        [TestMethod]
        //        public void FindBallFutureCoordinates_NoRicochetTestTwo()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(700, 650, timeStamp);
        //            currentCoordinates.Vector = new Vector2D(-30, -40);
        //            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(4);
        //            BallCoordinates actualResult = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            Assert.AreEqual(actualResult.X, 580);
        //            Assert.AreEqual(actualResult.Y, 490);
        //            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
        //            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
        //            Assert.AreEqual(actualResult.Timestamp, actionTime);
        //        }

        //        /// <summary>
        //        /// Current Coordinates: 500, 400, time - now; Vector -100 0
        //        /// Actual Time - current coordinates time stamp + 3 sec
        //        /// Expected Coordinates: 200, 400, time is actual time; Vector -100 0
        //        /// </summary>
        //        [TestMethod]
        //        public void FindBallFutureCoordinates_NoRicochetTestThree()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(500, 400, timeStamp);
        //            currentCoordinates.Vector = new Vector2D(-100, 0);
        //            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(3);
        //            BallCoordinates actualResult = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            Assert.AreEqual(actualResult.X, 200);
        //            Assert.AreEqual(actualResult.Y, 400);
        //            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
        //            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
        //            Assert.AreEqual(actualResult.Timestamp, actionTime);
        //        }

        //        /// <summary>
        //        /// Current Coordinates: 250, 450, time - now; Vector 0 25
        //        /// Actual Time - current coordinates time stamp + 10 sec
        //        /// Expected Coordinates: 250, 700, time is actual time; Vector 0 25
        //        /// </summary>
        //        [TestMethod]
        //        public void FindBallFutureCoordinates_NoRicochetTestFour()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(250, 450, timeStamp);
        //            currentCoordinates.Vector = new Vector2D(0, 25);
        //            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(10);
        //            BallCoordinates actualResult = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            Assert.AreEqual(actualResult.X, 250);
        //            Assert.AreEqual(actualResult.Y, 700);
        //            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
        //            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
        //            Assert.AreEqual(actualResult.Timestamp, actionTime);
        //        }

        //        /// <summary>
        //        /// Current Coordinates: 100, 500, time - now; Vector -50 0
        //        /// Actual Time - current coordinates time stamp + 4 sec
        //        /// Expected Coordinates: 70, 500, time is actual time; Vector 35 0
        //        /// </summary>
        //        [TestMethod]
        //        public void FindBallFutureCoordinates_RicochetTestOne()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(100, 500, timeStamp);
        //            currentCoordinates.Vector = new Vector2D(-50, 0);
        //            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(4);
        //            BallCoordinates actualResult = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            double speedXAfterRicochet = (-1) * RICOCHET_FACTOR * currentCoordinates.Vector.X;
        //            double xCoordAfterRicochet = speedXAfterRicochet * 2;
        //            Assert.AreEqual(actualResult.X, xCoordAfterRicochet);
        //            Assert.AreEqual(actualResult.Y, 500);
        //            Assert.AreEqual(actualResult.Vector.X, speedXAfterRicochet);
        //            Assert.AreEqual(actualResult.Vector.Y, 0);
        //            Assert.AreEqual(actualResult.Timestamp, actionTime);
        //        }

        //        /// <summary>
        //        /// Current Coordinates: 450, 200, time - now; Vector 0 -50
        //        /// Actual Time - current coordinates time stamp + 8 sec
        //        /// Expected Coordinates: 450, 140, time is actual time; Vector 0 35
        //        /// </summary>
        //        [TestMethod]
        //        public void FindBallFutureCoordinates_RicochetTestTwo()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(450, 200, timeStamp);
        //            currentCoordinates.Vector = new Vector2D(0, -50);
        //            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(8);
        //            BallCoordinates actualResult = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);
        //            double speedYAfterRicochet = (-1) * RICOCHET_FACTOR * currentCoordinates.Vector.Y;
        //            double yCoordAfterRicochet = speedYAfterRicochet * 4;
        //            Assert.AreEqual(actualResult.X, 450);
        //            Assert.AreEqual(actualResult.Y, yCoordAfterRicochet);
        //            Assert.AreEqual(actualResult.Vector.X, 0);
        //            Assert.AreEqual(actualResult.Vector.Y, speedYAfterRicochet);
        //            Assert.AreEqual(actualResult.Timestamp, actionTime);
        //        }

        //        /// <summary>
        //        /// Current Coordinates: 300, 200, time - now; Vector 100 -100
        //        /// Actual Time - current coordinates time stamp + 10 sec
        //        /// Expected Coordinates: 1060, 560, time is actual time; Vector 70 70
        //        /// </summary>
        //        [TestMethod]
        //        public void FindBallFutureCoordinates_RicochetTestThree()
        //        {
        //            DateTime timeStamp = DateTime.Now;
        //            BallCoordinates currentCoordinates = new BallCoordinates(300, 200, timeStamp);
        //            currentCoordinates.Vector = new Vector2D(100, -100);
        //            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(10);
        //            BallCoordinates actualResult = _decision.FindBallFutureCoordinates(currentCoordinates, actionTime);

        //            double speedXAfterRicochet = RICOCHET_FACTOR * currentCoordinates.Vector.X;
        //            double xCoordAfterRicochet = 500+speedXAfterRicochet * 8;
        //            double speedYAfterRicochet = (-1) * RICOCHET_FACTOR * currentCoordinates.Vector.Y;
        //            double yCoordAfterRicochet = speedYAfterRicochet * 8;

        //            Assert.AreEqual(actualResult.X, xCoordAfterRicochet);
        //            Assert.AreEqual(actualResult.Y, yCoordAfterRicochet);
        //            Assert.AreEqual(actualResult.Vector.X, speedXAfterRicochet);
        //            Assert.AreEqual(actualResult.Vector.Y, speedYAfterRicochet);
        //            Assert.AreEqual(actualResult.Timestamp, actionTime);
        //        }


        //        #endregion FindBallFutureCoordinates Test
        //    }
        
    }
}
