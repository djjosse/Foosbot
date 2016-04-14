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

namespace DecisionUnitTest.Core
{
    [TestClass]
    public class RodStateTest
    {
        private const int MIN_LIMIT = 30;
        private const int MAX_LIMIT = 260;
        private static IRodState _rodState;

        [ClassInitialize]
        public static void TestInitialize(TestContext context)
        {
            _rodState = new RodState(MIN_LIMIT, MAX_LIMIT);
        }

        #region DC Position Test

        [TestMethod]
        public void DcPosition_Positive()
        {
            int expected = MIN_LIMIT + 90;
            _rodState.DcPosition = expected;
            int actual = _rodState.DcPosition;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DcPosition_Negative_ParameterToBig()
        {
            int param = MAX_LIMIT + 90;
            _rodState.DcPosition = param;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DcPosition_Negative_ParameterToSmall()
        {
            int param = MIN_LIMIT - 90;
            _rodState.DcPosition = param;
        }

        #endregion DC Position Test

        #region Servo Position Test

        [TestMethod]
        public void ServoPosition_Positive()
        {
            eRotationalMove expected = eRotationalMove.DEFENCE;
            _rodState.ServoPosition = expected;
            eRotationalMove actual = _rodState.ServoPosition;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ServoPosition_Positive_NaNoConsiquence()
        {
            eRotationalMove expected = eRotationalMove.DEFENCE;
            _rodState.ServoPosition = expected;
            _rodState.ServoPosition = eRotationalMove.NA;
            eRotationalMove actual = _rodState.ServoPosition;
            Assert.AreEqual(expected, actual);
        }

        #endregion Servo Position Test
    }
}
