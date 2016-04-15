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
using Foosbot.DecisionUnit.Contracts;
using Foosbot.DecisionUnit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnitTest.Core
{
    [TestClass]
    public class SurveyorTest
    {
        private const string CATEGORY = "Surveyor";

        private const int X_MAX_PTS = 980;
        private const int Y_MAX_PTS = 580;
        private const int X_MAX_MM = 1100;
        private const int Y_MAX_MM = 700;
        private static ISurveyor _testAsset;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _testAsset = new Surveyor(X_MAX_PTS, Y_MAX_PTS, X_MAX_MM, Y_MAX_MM);
        }

        #region Is Coordinates In Range Test

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesInRange_X_and_Y_In_Range()
        {
            int x = 100;
            int y = 50;
            bool expected = true;
            bool actual = _testAsset.IsCoordinatesInRange(x, y);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesInRange_X_In_Range_Y_Not_In_Range()
        {
            int x = 100;
            int y = 1000;
            bool expected = false;
            bool actual = _testAsset.IsCoordinatesInRange(x, y);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesInRange_X_Not_In_Range_Y_In_Range()
        {
            int x = 1200;
            int y = 100;
            bool expected = false;
            bool actual = _testAsset.IsCoordinatesInRange(x, y);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesInRange_X_And_Y_Not_In_Range()
        {
            int x = 1000;
            int y = 1000;
            bool expected = false;
            bool actual = _testAsset.IsCoordinatesInRange(x, y);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesYInRange_Y_In_Range()
        {
            int yCoord = 1;
            bool expected = true;
            bool actual = _testAsset.IsCoordinatesYInRange(yCoord);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesYInRange_Y_Not_In_Range()
        {
            int yCoord = 1000;
            bool expected = false;
            bool actual = _testAsset.IsCoordinatesYInRange(yCoord);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesXInRange_X_In_Range()
        {
            int xCoord = 1;
            bool expected = true;
            bool actual = _testAsset.IsCoordinatesXInRange(xCoord);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void IsCoordinatesXInRange_X_Not_In_Range()
        {
            int xCoord = 1000;
            bool expected = false;
            bool actual = _testAsset.IsCoordinatesYInRange(xCoord);
            Assert.AreEqual(actual, expected);
        }

        #endregion is coordinates in range test

        #region Convert Points To Millimeters Test

        [TestCategory(CATEGORY), TestMethod]
        public void PtsToMm_Coordinates_Null()
        {
            BallCoordinates expected = null;
            BallCoordinates actual = _testAsset.PtsToMm(expected);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void PtsToMm_Coordinates_Not_Defined()
        {
            BallCoordinates expected = new BallCoordinates(DateTime.Now);
            BallCoordinates actual = _testAsset.PtsToMm(expected);
            Assert.AreEqual(actual, expected);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void PtsToMm_Coordinates_Only()
        {
            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);

            int xMm = ballCoordinates.X * X_MAX_MM / X_MAX_PTS;
            int yMm = ballCoordinates.Y * Y_MAX_MM / Y_MAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            BallCoordinates actual = _testAsset.PtsToMm(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void PtsToMm_Vector_Null()
        {
            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);

            int xMm = ballCoordinates.X * X_MAX_MM / X_MAX_PTS;
            int yMm = ballCoordinates.Y * Y_MAX_MM / Y_MAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            expected.Vector = null;

            BallCoordinates actual = _testAsset.PtsToMm(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
            Assert.AreEqual(actual.Vector, expected.Vector);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void PtsToMm_Vector_Not_Defined()
        {
            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            ballCoordinates.Vector = new Vector2D();

            int xMm = ballCoordinates.X * X_MAX_MM / X_MAX_PTS;
            int yMm = ballCoordinates.Y * Y_MAX_MM / Y_MAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            expected.Vector = new Vector2D();

            BallCoordinates actual = _testAsset.PtsToMm(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
            Assert.AreEqual(actual.Vector.IsDefined, expected.Vector.IsDefined);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void PtsToMm__Vector_And_Coordinates()
        {
            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            ballCoordinates.Vector = new Vector2D(100, 100);

            int xMm = ballCoordinates.X * X_MAX_MM / X_MAX_PTS;
            int yMm = ballCoordinates.Y * Y_MAX_MM / Y_MAX_PTS;

            double xMmVector = ballCoordinates.Vector.X * (double)X_MAX_MM / (double)X_MAX_PTS;
            double yMmVector = ballCoordinates.Vector.Y * (double)Y_MAX_MM / (double)Y_MAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            expected.Vector = new Vector2D(xMmVector, yMmVector);

            BallCoordinates actual = _testAsset.PtsToMm(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
            Assert.AreEqual(actual.Vector.X, expected.Vector.X);
            Assert.AreEqual(actual.Vector.Y, expected.Vector.Y);
        }

        #endregion Convert Points To Millimeters Test
    }
}
