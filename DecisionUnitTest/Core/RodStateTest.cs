// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;
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
    public class RodStateTest
    {
        private const string CATEGORY = "RodState";

        private const int MIN_LIMIT = 30;
        private const int MAX_LIMIT = 260;
        private static IRodState _rodState;

        [ClassInitialize]
        public static void TestInitialize(TestContext context)
        {
            _rodState = new RodState(MIN_LIMIT, MAX_LIMIT);
        }

        #region DC Position Test

        [TestCategory(CATEGORY), TestMethod]
        public void DcPosition_Positive()
        {
            int expected = MIN_LIMIT + 90;
            _rodState.DcPosition = expected;
            int actual = _rodState.DcPosition;
            Assert.AreEqual(expected, actual);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DcPosition_Negative_ParameterToBig()
        {
            int param = MAX_LIMIT + 90;
            _rodState.DcPosition = param;
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DcPosition_Negative_ParameterToSmall()
        {
            int param = MIN_LIMIT - 90;
            _rodState.DcPosition = param;
        }

        #endregion DC Position Test

        #region Servo Position Test

        [TestCategory(CATEGORY), TestMethod]
        public void ServoPosition_Positive()
        {
            eRotationalMove expected = eRotationalMove.DEFENCE;
            _rodState.ServoPosition = expected;
            eRotationalMove actual = _rodState.ServoPosition;
            Assert.AreEqual(expected, actual);
        }

        [TestCategory(CATEGORY), TestMethod]
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
