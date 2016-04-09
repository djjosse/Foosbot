using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foosbot.DecisionUnit;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot;

namespace DecisionUnitTest
{
    [TestClass]
    public class DecisionTest
    {
        [ClassInitialize]
        public static void DecisionTestInitialize(TestContext context)
        {
            Configuration.Attributes.IsKeyExist(Configuration.Names.KEY_IS_DEMO_MODE);
        }

        #region Find Action Time Test
        [TestMethod]
        public void FindActionTime_Test()
        {
            TimeSpan deltaMilliseconds = TimeSpan.FromMilliseconds(100);
            TimeSpan delay = TimeSpan.FromMilliseconds(5);

            PrivateObject po = new PrivateObject(typeof(Decision), new BallLocationPublisher(null));

            DateTime expected = DateTime.Now + delay;
            DateTime actual = (DateTime)po.Invoke("FindActionTime", new Type[] { typeof(TimeSpan) }, new Object[] { delay }, new Type[] { typeof(TimeSpan) });

            DateTime expectedPlusDelta = expected + deltaMilliseconds;
            DateTime expectedMinusDelta = expected - deltaMilliseconds;

            Assert.IsTrue(expectedPlusDelta > actual);
            Assert.IsTrue(expectedMinusDelta < actual);

        }

        #endregion Find Action Time Test

        #region Is Coordinates In Range Test

        [TestMethod]
        public void IsCoordinatesInRange_X_and_Y_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates coordinates = new BallCoordinates(100, 50, DateTime.Now);

            bool expected = true;
            bool actual = decision.IsCoordinatesInRange(coordinates.X, coordinates.Y);

            Assert.AreEqual(actual, expected);       
        }

        [TestMethod]
        public void IsCoordinatesInRange_X_In_Range_Y_Not_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates coordinates = new BallCoordinates(100, 1000, DateTime.Now);

            bool expected = false;
            bool actual = decision.IsCoordinatesInRange(coordinates.X, coordinates.Y);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void IsCoordinatesInRange_X_Not_In_Range_Y_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates coordinates = new BallCoordinates(1200, 100, DateTime.Now);

            bool expected = false;
            bool actual = decision.IsCoordinatesInRange(coordinates.X, coordinates.Y);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void IsCoordinatesInRange_X_And_Y_Not_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates coordinates = new BallCoordinates(1000, 1000, DateTime.Now);

            bool expected = false;
            bool actual = decision.IsCoordinatesInRange(coordinates.X, coordinates.Y);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void IsCoordinatesYInRange_Y_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            int yCoord = 1;

            bool expected = true;
            bool actual = decision.IsCoordinatesYInRange(yCoord);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void IsCoordinatesYInRange_Y_Not_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            int yCoord = 1000;

            bool expected = false;
            bool actual = decision.IsCoordinatesYInRange(yCoord);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void IsCoordinatesXInRange_X_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            int xCoord = 1;

            bool expected = true;
            bool actual = decision.IsCoordinatesXInRange(xCoord);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void IsCoordinatesXInRange_X_Not_In_Range()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            int xCoord = 1000;

            bool expected = false;
            bool actual = decision.IsCoordinatesYInRange(xCoord);

            Assert.AreEqual(actual, expected);
        }

        #endregion is coordinates in range test

        #region Convert Points To Millimeters Test

        [TestMethod]
        public void ConvertPointsToMillimeters_Coordinates_Null()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates expected = null;
            BallCoordinates actual = decision.ConvertPointsToMillimeters(expected);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void ConvertPointsToMillimeters_Coordinates_Not_Defined()
        {
            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates expected = new BallCoordinates(DateTime.Now);
            BallCoordinates actual = decision.ConvertPointsToMillimeters(expected);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void ConvertPointsToMillimeters_Coordinates_Only()
        {
            int XMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            int YMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);

            int XMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            int YMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);

            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);

            int xMm = ballCoordinates.X * XMAX_MM / XMAX_PTS;
            int yMm = ballCoordinates.Y * YMAX_MM / YMAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            BallCoordinates actual = decision.ConvertPointsToMillimeters(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
        }

        [TestMethod]
        public void ConvertPointsToMillimeters_Vector_Null()
        {
            int XMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            int YMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);

            int XMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            int YMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);

            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);

            int xMm = ballCoordinates.X * XMAX_MM / XMAX_PTS;
            int yMm = ballCoordinates.Y * YMAX_MM / YMAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            expected.Vector = null;

            BallCoordinates actual = decision.ConvertPointsToMillimeters(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
            Assert.AreEqual(actual.Vector,expected.Vector);
        }

        [TestMethod]
        public void ConvertPointsToMillimeters_Vector_Not_Defined()
        {
            int XMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            int YMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);

            int XMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            int YMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);

            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            ballCoordinates.Vector = new Vector2D();

            int xMm = ballCoordinates.X * XMAX_MM / XMAX_PTS;
            int yMm = ballCoordinates.Y * YMAX_MM / YMAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            expected.Vector = new Vector2D();

            BallCoordinates actual = decision.ConvertPointsToMillimeters(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
            Assert.AreEqual(actual.Vector.IsDefined, expected.Vector.IsDefined);
        }

        [TestMethod]
        public void ConvertPointsToMillimeters_Vector_And_Coordinates()
        {
            int XMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            int YMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);

            int XMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            int YMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);

            Decision decision = new Decision(new BallLocationPublisher(null));

            BallCoordinates ballCoordinates = new BallCoordinates(50, 50, DateTime.Now);
            ballCoordinates.Vector = new Vector2D(100,100);

            int xMm = ballCoordinates.X * XMAX_MM / XMAX_PTS;
            int yMm = ballCoordinates.Y * YMAX_MM / YMAX_PTS;

            double xMmVector = ballCoordinates.Vector.X * (double)XMAX_MM / (double)XMAX_PTS;
            double yMmVector = ballCoordinates.Vector.Y * (double)YMAX_MM / (double)YMAX_PTS;

            BallCoordinates expected = new BallCoordinates(xMm, yMm, ballCoordinates.Timestamp);
            expected.Vector = new Vector2D(xMmVector, yMmVector);

            BallCoordinates actual = decision.ConvertPointsToMillimeters(ballCoordinates);

            Assert.AreEqual(actual.X, expected.X);
            Assert.AreEqual(actual.Y, expected.Y);
            Assert.AreEqual(actual.Vector.X, expected.Vector.X);
            Assert.AreEqual(actual.Vector.Y, expected.Vector.Y);
        }

        #endregion Convert Points To Millimeters Test
        

    }
}
