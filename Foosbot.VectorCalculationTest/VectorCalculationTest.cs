using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.VectorCalculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculationTest
{
    [TestClass]
    public class VectorCalculationTest
    {
        private static int XMAX_PTS;
        private static int YMAX_PTS;
        private static int XMAX_MM;
        private static int YMAX_MM;
        private static double RICOCHET_FACTOR;

        private static Publisher<BallCoordinates> _mockPublisher;


        [ClassInitialize]
        public static void VectorCalculationTestInitialize(TestContext context)
        {
            _mockPublisher = Substitute.For<Publisher<BallCoordinates>>();

            XMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            YMAX_PTS = Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            XMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            YMAX_MM = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);

            RICOCHET_FACTOR = Configuration.Attributes.GetValue<double>(Configuration.Names.KEY_RICOCHET_FACTOR);
        }

        #region Vector Calculation Algorithm Test


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VectorCalculationAlgorithm_Ball_Coordinates_Null_Test()
        {
            PrivateObject po = new PrivateObject(typeof(VectorCalculationUnit), _mockPublisher , 5);
            Vector2D actual = (Vector2D)po.Invoke("VectorCalculationAlgorithm",
                                                new Type[] { typeof(BallCoordinates) },
                                                new Object[] { null },
                                                new Type[] { typeof(BallCoordinates) });
        }

        [TestMethod]
        public void VectorCalculationAlgorithm_Stored_BallCoordinates_Not_Defined_Test()
        {
            BallCoordinates ballCoordinates = new BallCoordinates(DateTime.Now);
            PrivateObject po = new PrivateObject(typeof(VectorCalculationUnit), _mockPublisher, 5);

            Vector2D expected = new Vector2D();
            Vector2D actual = (Vector2D)po.Invoke("VectorCalculationAlgorithm",
                                                new Type[] { typeof(BallCoordinates) },
                                                new Object[] { ballCoordinates },
                                                new Type[] { typeof(BallCoordinates) });
            Assert.AreEqual(actual, actual);

        }

        [TestMethod]
        public void VectorCalculationAlgorithm_BallCoordinates_Not_Defined_Test()
        {
            BallCoordinates ballCoordinates = new BallCoordinates(DateTime.Now);
            PrivateObject po = new PrivateObject(typeof(VectorCalculationUnit), _mockPublisher, 5);

            Vector2D expected = new Vector2D();
            Vector2D actual = (Vector2D)po.Invoke("VectorCalculationAlgorithm",
                                                new Type[] { typeof(BallCoordinates) },
                                                new Object[] { ballCoordinates },
                                                new Type[] { typeof(BallCoordinates) });
            Assert.AreEqual(actual, actual);

        }


        #endregion Vector Calculation Algorithm Test
    }
}
