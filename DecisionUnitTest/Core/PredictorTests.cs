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
    #region PreditorTests
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

        #region FindBallFutureCoordinates Test

        #region FindBallFutureCoordinates_BallCoordinatesVectorNull
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_BallCoordinatesVectorNull()
        {
            //arrange
            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, DateTime.Now);
            DateTime actionTime = DateTime.Now + TimeSpan.FromSeconds(5);

            //act
            BallCoordinates actualCoordinates = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);


            //assert
            Assert.AreEqual(currentCoordinates.X, actualCoordinates.X);
            Assert.AreEqual(currentCoordinates.Y, actualCoordinates.Y);
            Assert.AreEqual(actionTime, actualCoordinates.Timestamp);
            Assert.IsNull(actualCoordinates.Vector);

        }
        #endregion FindBallFutureCoordinates_BallCoordinatesVectorNull

        #region FindBallFutureCoordinates_BallCoordinatesNull
        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindBallFutureCoordinates_BallCoordinatesNull()
        {
            _testAsset.FindBallFutureCoordinates(null, DateTime.Now);

        }
        #endregion FindBallFutureCoordinates_BallCoordinatesNull

        #region FindBallFutureCoordinates_BallCoordinatesUndefined
        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindBallFutureCoordinates_BallCoordinatesUndefined()
        {
            _testAsset.FindBallFutureCoordinates(new BallCoordinates(DateTime.Now), DateTime.Now);

        }
        #endregion FindBallFutureCoordinates_BallCoordinatesUndefined

        #region FindBallFutureCoordinates_VectorUndefined
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_VectorUndefined()
        {
            //arrange
            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, DateTime.Now);
            currentCoordinates.Vector = new Vector2D();
            DateTime actionTime = DateTime.Now + TimeSpan.FromSeconds(5);

            //act
            BallCoordinates actualCoordinates = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(currentCoordinates.X, actualCoordinates.X);
            Assert.AreEqual(currentCoordinates.Y, actualCoordinates.Y);
            Assert.AreEqual(actionTime, actualCoordinates.Timestamp);
            Assert.IsFalse(actualCoordinates.Vector.IsDefined);
        }
        #endregion FindBallFutureCoordinates_VectorUndefined

        #region FindBallFutureCoordinates_VectorSpeedZero
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_VectorSpeedZero()
        {
            //arrange
            DateTime now = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, now);
            currentCoordinates.Vector = new Vector2D(0, 0);
            _surveyorMock.IsCoordinatesInRange(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            DateTime actionTime = now + TimeSpan.FromSeconds(5);

            //act
            BallCoordinates actualCoordinates = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(currentCoordinates.X, actualCoordinates.X);
            Assert.AreEqual(currentCoordinates.Y, actualCoordinates.Y);
            Assert.AreEqual(actionTime, actualCoordinates.Timestamp);
            Assert.AreEqual(currentCoordinates.Vector.X, actualCoordinates.Vector.X);
            Assert.AreEqual(currentCoordinates.Vector.Y, actualCoordinates.Vector.Y);
        }
        #endregion FindBallFutureCoordinates_VectorSpeedZero

        #region FindBallFutureCoordinates_ActionTimeIsEarlierThanTimeStamp
        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindBallFutureCoordinates_ActionTimeIsEarlierThanTimeStamp()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(100, 100, timeStamp);

            //act
            DateTime actionTime = timeStamp - TimeSpan.FromSeconds(5);

            //assert
            _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

        }
        #endregion FindBallFutureCoordinates_ActionTimeIsEarlierThanTimeStamp


        #region FindBallFutureCoordinates_NoRicochetTest
        //<summary>
        //Current Coordinates: 90, 60, time - now; Vector 50 50
        //Actual Time - current coordinates time stamp + 5 sec
        //Expected Coordinates: 340, 310, time is actual time; Vector 50 50
        //</summary>
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_NoRicochetTestOne()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(90, 60, timeStamp);
            currentCoordinates.Vector = new Vector2D(50, 50);
            _surveyorMock.IsCoordinatesInRange(Arg.Any<int>(), Arg.Any<int>()).Returns(true);
            //act
            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(5);
            BallCoordinates actualResult = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(actualResult.X, 340);
            Assert.AreEqual(actualResult.Y, 310);
            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
            Assert.AreEqual(actualResult.Timestamp, actionTime);


        }

        //<summary>
        //Current Coordinates: 700, 650, time - now; Vector -30 -40
        //Actual Time - current coordinates time stamp + 4 sec
        //Expected Coordinates: 580, 490, time is actual time; Vector -30 -40
        //</summary>
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_NoRicochetTestTwo()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(700, 650, timeStamp);
            currentCoordinates.Vector = new Vector2D(-30, -40);
            _surveyorMock.IsCoordinatesInRange(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            //act
            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(4);
            BallCoordinates actualResult = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(actualResult.X, 580);
            Assert.AreEqual(actualResult.Y, 490);
            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
            Assert.AreEqual(actualResult.Timestamp, actionTime);

        }

        //<summary>
        //Current Coordinates: 500, 400, time - now; Vector -100 0
        //Actual Time - current coordinates time stamp + 3 sec
        //Expected Coordinates: 200, 400, time is actual time; Vector -100 0
        //</summary>
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_NoRicochetTestThree()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(500, 400, timeStamp);
            currentCoordinates.Vector = new Vector2D(-100, 0);

            //act
            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(3);
            BallCoordinates actualResult = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(actualResult.X, 200);
            Assert.AreEqual(actualResult.Y, 400);
            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
            Assert.AreEqual(actualResult.Timestamp, actionTime);
        }

        //<summary>
        //Current Coordinates: 250, 450, time - now; Vector 0 25
        //Actual Time - current coordinates time stamp + 10 sec
        //Expected Coordinates: 250, 700, time is actual time; Vector 0 25
        //</summary>
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_NoRicochetTestFour()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(250, 450, timeStamp);
            currentCoordinates.Vector = new Vector2D(0, 25);

            //act
            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(10);
            BallCoordinates actualResult = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(actualResult.X, 250);
            Assert.AreEqual(actualResult.Y, 700);
            Assert.AreEqual(actualResult.Vector.X, currentCoordinates.Vector.X);
            Assert.AreEqual(actualResult.Vector.Y, currentCoordinates.Vector.Y);
            Assert.AreEqual(actualResult.Timestamp, actionTime);
        }


        #endregion FindBallFutureCoordinates_NoRicochetTest

        #region FindBallFutureCoordinates_RicochetTest
        //<summary>
        //Current Coordinates: (100, 500), time stamp = now (t1); Vector (-50, 0)
        //Actual Time For Intersection - current coordinates time stamp + 5 sec (t2=t1+5) - this is the the intersection time with Axe Y
        //Ricochet Coordinates (Intersection Coordinates) : (70, 500); Vector (30, 0)
        //Expected Coordinates: (220, 500), time is Actual Time (t3=t1+10); Vector (30, 0)
        //</summary>
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_RicochetTestOne()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(100, 500, timeStamp);
            currentCoordinates.Vector = new Vector2D(-50, 0);
            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(10);
            BallCoordinates ricocheteCoordinates = new BallCoordinates(70, 500, timeStamp + TimeSpan.FromSeconds(5));
            ricocheteCoordinates.Vector = new Vector2D(30, 0);
            _surveyorMock.IsCoordinatesInRange(Arg.Any<int>(), Arg.Any<int>()).Returns(false, true);
            _ricochetMock.Ricochet(Arg.Any<BallCoordinates>()).Returns(ricocheteCoordinates);

            double deltaT = (actionTime - ricocheteCoordinates.Timestamp).TotalSeconds;
            int expectedX = Convert.ToInt32(ricocheteCoordinates.X + ricocheteCoordinates.Vector.X * deltaT);
            int expectedY = Convert.ToInt32(ricocheteCoordinates.Y + ricocheteCoordinates.Vector.Y * deltaT);
            double expectedVectorX = ricocheteCoordinates.Vector.X;
            double expectedVectorY = ricocheteCoordinates.Vector.Y;

            //act
            BallCoordinates actualResult = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(actualResult.X, expectedX);
            Assert.AreEqual(actualResult.Y, expectedY);
            Assert.AreEqual(actualResult.Vector.X, expectedVectorX);
            Assert.AreEqual(actualResult.Vector.Y, expectedVectorY);
            Assert.AreEqual(actualResult.Timestamp, actionTime);
        }

        //<summary>
        //Current Coordinates: (450, 200), time stamp = now (t1); Vector (0, -50)
        //Actual Time For Intersection - current coordinates time stamp + 5 sec (t2=t1+5) - this is the the intersection time with Axe X
        //Ricochet Coordinates (Intersection Coordinates) : (450, 70); Vector (0, 30)
        //Expected Coordinates: (450, 220), time is Actual Time (t3=t1+10); Vector (30, 0)
        //</summary>
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_RicochetTestTwo()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(450, 200, timeStamp);
            currentCoordinates.Vector = new Vector2D(0, -50);
            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(10);
            BallCoordinates ricocheteCoordinates = new BallCoordinates(450, 70, timeStamp + TimeSpan.FromSeconds(5));
            ricocheteCoordinates.Vector = new Vector2D(0, 30);
            _surveyorMock.IsCoordinatesInRange(Arg.Any<int>(), Arg.Any<int>()).Returns(false, true);
            _ricochetMock.Ricochet(Arg.Any<BallCoordinates>()).Returns(ricocheteCoordinates);

            double deltaT = (actionTime - ricocheteCoordinates.Timestamp).TotalSeconds;
            int expectedX = Convert.ToInt32(ricocheteCoordinates.X + ricocheteCoordinates.Vector.X * deltaT);
            int expectedY = Convert.ToInt32(ricocheteCoordinates.Y + ricocheteCoordinates.Vector.Y * deltaT);
            double expectedVectorX = ricocheteCoordinates.Vector.X;
            double expectedVectorY = ricocheteCoordinates.Vector.Y;

            //act
            BallCoordinates actualResult = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(actualResult.X, expectedX);
            Assert.AreEqual(actualResult.Y, expectedY);
            Assert.AreEqual(actualResult.Vector.X, expectedVectorX);
            Assert.AreEqual(actualResult.Vector.Y, expectedVectorY);
            Assert.AreEqual(actualResult.Timestamp, actionTime);
        }


        //<summary>
        //Current Coordinates: (300, 200), time stamp = now (t1); Vector (100, -100)
        //Actual Time For Intersection - current coordinates time stamp + 2 sec (t2=t1+2) - this is the the intersection time with Axe X
        //Ricochet Coordinates (Intersection Coordinates) : (500, 0); Vector (70, -70)
        //Expected Coordinates: (1060, 560), time is Actual Time (t3=t1+10); Vector (70, -70)
        //</summary>
        [TestCategory(CATEGORY), TestMethod]
        public void FindBallFutureCoordinates_RicochetTestThree()
        {
            //arrange
            DateTime timeStamp = DateTime.Now;
            BallCoordinates currentCoordinates = new BallCoordinates(300, 200, timeStamp);
            currentCoordinates.Vector = new Vector2D(100, -100);
            DateTime actionTime = timeStamp + TimeSpan.FromSeconds(10);
            BallCoordinates ricocheteCoordinates = new BallCoordinates(500, 0, timeStamp + TimeSpan.FromSeconds(2));
            ricocheteCoordinates.Vector = new Vector2D(100, -100);
            _surveyorMock.IsCoordinatesInRange(Arg.Any<int>(), Arg.Any<int>()).Returns(false, true);
            _ricochetMock.Ricochet(Arg.Any<BallCoordinates>()).Returns(ricocheteCoordinates);

            double deltaT = (actionTime - ricocheteCoordinates.Timestamp).TotalSeconds;
            int expectedX = Convert.ToInt32(ricocheteCoordinates.X + ricocheteCoordinates.Vector.X * deltaT);
            int expectedY = Convert.ToInt32(ricocheteCoordinates.Y + ricocheteCoordinates.Vector.Y * deltaT);
            double expectedVectorX = ricocheteCoordinates.Vector.X;
            double expectedVectorY = ricocheteCoordinates.Vector.Y;

            //act
            BallCoordinates actualResult = _testAsset.FindBallFutureCoordinates(currentCoordinates, actionTime);

            //assert
            Assert.AreEqual(actualResult.X, expectedX);
            Assert.AreEqual(actualResult.Y, expectedY);
            Assert.AreEqual(actualResult.Vector.X, expectedVectorX);
            Assert.AreEqual(actualResult.Vector.Y, expectedVectorY);
            Assert.AreEqual(actualResult.Timestamp, actionTime);
        }


        #endregion FindBallFutureCoordinates_RicochetTest


        #endregion FindBallFutureCoordinates Test

    }
    #endregion PreditorTests
}
