using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foosbot.Common.Protocols;
using Foosbot.VectorCalculation;
using Foosbot.VectorCalculation.Contracts;

namespace Foosbot.VectorCalculationTest
{
    [TestClass]
    public class RicochetCalcTest
    {
        private const string CATEGORY = "RicochetCalc";

        BallCoordinates _initialCoordinates;
        IInitializableRicochet _testAsset;

        public const int XMIN = 0;
        public const int XMAX = 800;
        public const int YMIN = 0;
        public const int YMAX = 400;
        public const double RICOCHE = 0.7;

        [TestInitialize]
        public void InitTestBed()
        {
            _testAsset = new RicochetCalc();
            _testAsset.InitializeRicochetCalc(eUnits.Mm, XMAX, YMAX, RICOCHE);
        }

        #region FindNearestIntersectionPoint

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void FindNearestIntersectionPoint_NoMove_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, 0);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveLeft_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, 0);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(XMIN, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveRight_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, 0);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(XMAX, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveUp_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, 100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(100, YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveDown_from_100_50()
        {
            _initialCoordinates = new BallCoordinates(100, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, -100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(100, YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftUp_LeftFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-50, 100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(XMIN, 150);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveRightDown_DownFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(50, -100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(75, YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftDown_DownFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-50, -100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(25, YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveRightUp_UpFirst_from_50_50()
        {
            _initialCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(50, 100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(225, YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveRight_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, 0);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(XMAX, 100);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveLeft_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, 0);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(XMIN, 100);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveUp_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, 100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(50, YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveDown_from_50_100()
        {
            _initialCoordinates = new BallCoordinates(50, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, -100);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(50, YMIN);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftUp_UpFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, 50);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(100, YMAX);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveRightDown_RightFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, -50);
            
            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(XMAX, 50);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveLeftDown_LeftFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(-100, -50);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(500, YMIN);
            
            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindNearestIntersectionPoint_MoveRightUp_RightFirst_from_700_100()
        {
            _initialCoordinates = new BallCoordinates(700, 100, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(100, 50);

            Coordinates2D actualResult = _testAsset.FindNearestIntersectionPoint(_initialCoordinates);
            Coordinates2D expectedResult = new Coordinates2D(XMAX, 150);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
        }

        #endregion FindNearestIntersectionPoint

        #region FindRicochetTime

        [TestCategory(CATEGORY), TestMethod]
        public void FindRicochetTime_Vector_minus100_50()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(500, YMAX);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(-100, 50);
            DateTime actualTime = _testAsset.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(2.0);
            Assert.AreEqual(expected, actualTime);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindRicochetTime_Vector_minus140_minus20()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(XMIN, 200);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(-140, -20);
            DateTime actualTime = _testAsset.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(5.0);
            Assert.AreEqual(expected, actualTime);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindRicochetTime_Vector_50_minus300()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(750, YMIN);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(50, -300);
            DateTime actualTime = _testAsset.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(1.0);
            Assert.AreEqual(expected, actualTime);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindRicochetTime_Vector_100_50()
        {
            DateTime now = DateTime.Now;
            Coordinates2D intersection = new Coordinates2D(XMAX, 350);
            BallCoordinates ballCoordinates = new BallCoordinates(700, 300, now);
            ballCoordinates.Vector = new Vector2D(100, 50);
            DateTime actualTime = _testAsset.FindRicochetTime(ballCoordinates, intersection);
            DateTime expected = now + TimeSpan.FromSeconds(1.0);
            Assert.AreEqual(expected, actualTime);
        }

        #endregion FindRicochetTime

        #region FindIntersectionVector

        [TestCategory(CATEGORY), TestMethod]
        public void FindIntersectionVector_UpperBorder()
        {
            Coordinates2D intersection = new Coordinates2D(500, YMAX);
            Vector2D vector = new Vector2D(-100,50);
            Vector2D actualVector = _testAsset.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(-100 * RICOCHE, -50 * RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindIntersectionVector_LeftBorder()
        {
            Coordinates2D intersection = new Coordinates2D(XMIN, 200);
            Vector2D vector = new Vector2D(-140, -20);
            Vector2D actualVector = _testAsset.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(140 * RICOCHE, -20 * RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindIntersectionVector_ButtomBorder()
        {
            Coordinates2D intersection = new Coordinates2D(750, YMIN);
            Vector2D vector = new Vector2D(50, -300);
            Vector2D actualVector = _testAsset.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(50 * RICOCHE, 300 * RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void FindIntersectionVector_RightBorder()
        {
            Coordinates2D intersection = new Coordinates2D(XMAX, 350);
            Vector2D vector = new Vector2D(100, 50);
            Vector2D actualVector = _testAsset.FindIntersectionVector(vector, intersection);
            Vector2D expectedVector = new Vector2D(-100 * RICOCHE, 50 * RICOCHE);
            Assert.AreEqual(expectedVector.X, actualVector.X);
            Assert.AreEqual(expectedVector.Y, actualVector.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void FindIntersectionVector_InvalidIntersectionPoint()
        {
            Coordinates2D intersection = new Coordinates2D(XMAX - 5, XMIN - 5);
            Vector2D vector = new Vector2D(100, 50);
            Vector2D actualVector = _testAsset.FindIntersectionVector(vector, intersection);
        }

        #endregion FindIntersectionVector

        #region Ricochet

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Ricochet_UndefinedCoordinates()
        {
            _initialCoordinates = new BallCoordinates(DateTime.Now);
            _testAsset.Ricochet(_initialCoordinates);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Ricochet_NullCoordinates()
        {
            _initialCoordinates = null;
            _testAsset.Ricochet(_initialCoordinates);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Ricochet_UndefinedVector()
        {
            _initialCoordinates = new BallCoordinates(10, 10, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D();
            _testAsset.Ricochet(_initialCoordinates);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Ricochet_NullVector()
        {
            _initialCoordinates = new BallCoordinates(10, 10, DateTime.Now);
            _testAsset.Ricochet(_initialCoordinates);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Ricochet_ZeroVelocity()
        {
            _initialCoordinates = new BallCoordinates(10, 10, DateTime.Now);
            _initialCoordinates.Vector = new Vector2D(0, 0);
            _testAsset.Ricochet(_initialCoordinates);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void Ricochet_FromCoord100_50_SpeedMinus100_0()
        {
            DateTime initTime = DateTime.Now;
            _initialCoordinates = new BallCoordinates(100, 50, initTime);
            _initialCoordinates.Vector = new Vector2D(-100, 0);

            DateTime expectedTime = initTime + TimeSpan.FromSeconds(1);
            BallCoordinates expectedResult = new BallCoordinates(XMIN, 50, expectedTime);
            expectedResult.Vector = new Vector2D(100 * RICOCHE, 0);

            BallCoordinates actualResult = _testAsset.Ricochet(_initialCoordinates);

            Assert.AreEqual(expectedResult.X, actualResult.X);
            Assert.AreEqual(expectedResult.Y, actualResult.Y);
            Assert.AreEqual(expectedResult.Timestamp, actualResult.Timestamp);
            Assert.AreEqual(expectedResult.Vector.X, actualResult.Vector.X);
            Assert.AreEqual(expectedResult.Vector.Y, actualResult.Vector.Y);
        }

        #endregion Ricochet
    }
}
