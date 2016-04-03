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
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _vectorUtils = new VectorUtilsWrapper();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void FindNearestIntersectionPoint_NoMove()
        {
            _initialCoordinates.Vector = new Vector2D(0, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeft()
        {

            _initialCoordinates.Vector = new Vector2D(-100, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMIN, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRight()
        {
            _initialCoordinates.Vector = new Vector2D(100, 0);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMAX, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveUp()
        {
            _initialCoordinates.Vector = new Vector2D(0, 100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(50, VectorUtilsWrapper.YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveDown()
        {
            _initialCoordinates.Vector = new Vector2D(0, -100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(50, VectorUtilsWrapper.YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftUp()
        {
            _initialCoordinates.Vector = new Vector2D(-50, 100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(VectorUtilsWrapper.XMIN, 150);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRightDown()
        {
            _initialCoordinates.Vector = new Vector2D(50, -100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(75, VectorUtilsWrapper.YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftDown()
        {
            _initialCoordinates.Vector = new Vector2D(-50, -100);

            Coordinates2D actualResult = _vectorUtils.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(25, VectorUtilsWrapper.YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestMethod]
        public void FindNearestIntersectionPoint_MoveRightUp()
        {
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
    }
}
