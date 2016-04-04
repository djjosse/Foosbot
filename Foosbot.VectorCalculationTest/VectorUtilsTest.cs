using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foosbot.Common.Protocols;
using Foosbot.VectorCalculation;

namespace Foosbot.VectorCalculationTest
{
    [TestClass]
    public class VectorUtilsTest
    {
        BallCoordinates _initialCoordinates;
        VectorUtilsWrapper _vectorUtils;

        [TestInitialize]
        public void InitTestBed()
        {
            _vectorUtils = new VectorUtilsWrapper();
        }

        #region FindNearestIntersectionPoint() Tests

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void FindNearestIntersectionPoint_NoMove_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeft_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMIN, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRight_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMAX, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveUp_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, 100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(100, VectorUtilsWrapper.YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveDown_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, -100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(100, VectorUtilsWrapper.YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftUp_LeftFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-50, 100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMIN, 150);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRightDown_DownFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(50, -100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(75, VectorUtilsWrapper.YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftDown_DownFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-50, -100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(25, VectorUtilsWrapper.YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRightUp_UpFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(50, 100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(225, VectorUtilsWrapper.YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRight_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMAX, 100);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeft_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMIN, 100);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveUp_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, 100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(50, VectorUtilsWrapper.YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveDown_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, -100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(50, VectorUtilsWrapper.YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftUp_UpFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, 50);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(100, VectorUtilsWrapper.YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRightDown_RightFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, -50);
            
            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMAX, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftDown_LeftFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, -50);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(500, VectorUtilsWrapper.YMIN);
            
            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRightUp_RightFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, 50);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMAX, 150);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        #endregion FindNearestIntersectionPoint() Tests

        #region FindRicochetTime

        [TestMethod]
        public void FindRicochetTime_Vector_minus100_50()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(500, VectorUtilsWrapper.YMAX);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(-100, 50);
            DateTime actualTime = _vectorUtils.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(2.0);
            Assert.AreEqual(expected, actualTime);
        }

        [TestMethod]
        public void FindRicochetTime_Vector_minus140_minus20()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(VectorUtilsWrapper.XMIN, 200);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(-140, -20);
            DateTime actualTime = _vectorUtils.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(5.0);
            Assert.AreEqual(expected, actualTime);
        }

        [TestMethod]
        public void FindRicochetTime_Vector_50_minus300()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(750, VectorUtilsWrapper.YMIN);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(50, -300);
            DateTime actualTime = _vectorUtils.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(1.0);
            Assert.AreEqual(expected, actualTime);
        }

        [TestMethod]
        public void FindRicochetTime_Vector_100_50()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(VectorUtilsWrapper.XMAX, 350);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(100, 50);
            DateTime actualTime = _vectorUtils.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(1.0);
            Assert.AreEqual(expected, actualTime);
        }

        #endregion FindRicochetTime

        #region FindIntersectionVector

        [TestMethod]
        public void FindIntersectionVector_UpperBorder()
        {
            Coordinates2D intersection = new Coordinates2D(500, VectorUtilsWrapper.YMAX);
            Vector2D vector = new Vector2D(-100,50);
            Vector2D actualVector = _vectorUtils.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(-100 * VectorUtilsWrapper.RICOCHE, -50 * VectorUtilsWrapper.RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestMethod]
        public void FindIntersectionVector_LeftBorder()
        {
            Coordinates2D intersection = new Coordinates2D(VectorUtilsWrapper.XMIN, 200);
            Vector2D vector = new Vector2D(-140, -20);
            Vector2D actualVector = _vectorUtils.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(140 * VectorUtilsWrapper.RICOCHE, -20 * VectorUtilsWrapper.RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestMethod]
        public void FindIntersectionVector_ButtomBorder()
        {
            Coordinates2D intersection = new Coordinates2D(750, VectorUtilsWrapper.YMIN);
            Vector2D vector = new Vector2D(50, -300);
            Vector2D actualVector = _vectorUtils.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(50 * VectorUtilsWrapper.RICOCHE, 300 * VectorUtilsWrapper.RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestMethod]
        public void FindIntersectionVector_RightBorder()
        {
            Coordinates2D intersection = new Coordinates2D(VectorUtilsWrapper.XMAX, 350);
            Vector2D vector = new Vector2D(100, 50);
            Vector2D actualVector = _vectorUtils.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(-100 * VectorUtilsWrapper.RICOCHE, 50 * VectorUtilsWrapper.RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void FindIntersectionVector_InvalidIntersectionPoint()
        {
            Coordinates2D intersection = new Coordinates2D(VectorUtilsWrapper.XMAX - 5, VectorUtilsWrapper.XMIN - 5);
            Vector2D vector = new Vector2D(100, 50);
            Vector2D actualVector = _vectorUtils.FindIntersectionVector(vector, intersection);
        }

        #endregion FindIntersectionVector
    }
}
