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
        #region FindActionTime

        [ClassInitialize]
        public static void DecisionTestInitialize(TestContext context)
        {
            Configuration.Attributes.IsKeyExist(Configuration.Names.KEY_IS_DEMO_MODE);
        }

        [TestMethod]
        public void FindActionTime_Test()
        {

            //WORK IN PROGRESS...

            TimeSpan delay = TimeSpan.FromMilliseconds(5);
           // Decision d = new Decision();
            PrivateObject po = new PrivateObject(typeof(Decision), new BallLocationPublisher(null));

            DateTime expected = DateTime.Now + delay;
            DateTime actual = (DateTime)po.Invoke("FindActionTime", new Type[] { typeof(TimeSpan) }, new Object[] { delay }, new Type[] { typeof(TimeSpan) });

            Assert.AreEqual(actual, expected);
        }

        #endregion FindActionTime
    }
}
