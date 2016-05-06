using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using Foosbot.CommunicationLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayerTest
{
    [TestClass]
    public class ArduinoConverterTests
    {
        private const string CATEGORY = "ArduinoConverter";

        private static eRod ROD_TYPE = eRod.Defence;
        private static int TICKS_PER_ROD = 2600;
        private static int MIN_STOPPER_COORD_MM = 30;
        private static int TABLE_HEIGHT = 700;
        private static int STOPPER_DIST = 460;

        private static IRodConverter _testAsset;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _testAsset = new ArduinoConverter(ROD_TYPE);
            _testAsset.Initialize(TICKS_PER_ROD, MIN_STOPPER_COORD_MM, TABLE_HEIGHT, STOPPER_DIST);
        }

        

        [TestCategory(CATEGORY), TestMethod]
        public void MmToTicks_Minimal_WithFlip()
        {
            int inMm = MIN_STOPPER_COORD_MM;
            int expected = _testAsset.TicksPerRod - Communication.SAFETY_TICKS_BUFFER;
            int actual = _testAsset.MmToTicks(inMm, true);
            Assert.AreEqual(expected, actual);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void MmToTicks_Maximal_WithFlip()
        {
            int inMm = TABLE_HEIGHT - MIN_STOPPER_COORD_MM - STOPPER_DIST;
            int expected = _testAsset.TicksPerRod - (TICKS_PER_ROD - Communication.SAFETY_TICKS_BUFFER);
            int actual = _testAsset.MmToTicks(inMm, true);
            Assert.AreEqual(expected, actual);
        }
    }
}
