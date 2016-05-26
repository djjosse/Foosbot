// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessingUnitTest.Tools.Core
{
    [TestClass]
    public class DetectionStatisticAnalyzerTest
    {
        private const string CATEGORY = "DetectionStatisticAnalyzer";

        List<DateTime> _dates;
        List<TimeSpan> _spans;

        static ITime _timingObj = Substitute.For<ITime>();
        static DetectionStatisticAnalyzer _asset;

        [ClassInitialize]
        public static void InitTests(TestContext testContext)
        {
            _asset = new DetectionStatisticAnalyzer(_timingObj);
            
        }

        #region BeginFinalizeTests

        [TestCategory(CATEGORY), TestMethod]
        public void BeginFinalizeTest_SuccessTwoOfThree()
        {
            //Arrange
            decimal success = 0;
            decimal totalFrames = 3;
            
            _dates = new List<DateTime>();
            DateTime now = new DateTime(1,1,1,1,1,0,150);
            for (int i = 0; i < totalFrames+1; i++)
            {
                _dates.Add(now + TimeSpan.FromMilliseconds(300 * i));
            }

            _spans = new List<TimeSpan>() { TimeSpan.FromMilliseconds(15) };

            _timingObj.Now.Returns(_dates[0], _dates[1], _dates[2], _dates[3]);
            _timingObj.Elapsed.Returns(_spans[0]);
            double average = _spans.Average(x => x.Milliseconds);

            //Act
            for (int i = 0; i < totalFrames+1; i++)
            {
                _asset.Begin();
                bool res = i % 2 == 0;
                if (res) success++;
                _asset.Finalize(res);
            }

            //Assert
            Assert.AreEqual(success, _asset.DetectedFPS);
            Assert.AreEqual(totalFrames, _asset.TotalFPS);
            Assert.AreEqual(average, _asset.AverageDetectionTime);
            Assert.AreEqual(Math.Truncate((100*success)/totalFrames), (decimal)_asset.DetectionRate);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void BeginFinalizeTest_NotDetectedOfThree()
        {
            //Arrange
            decimal success = 0;
            decimal totalFrames = 3;

            _dates = new List<DateTime>();
            DateTime now = new DateTime(1, 1, 1, 1, 1, 0, 150);
            for (int i = 0; i < totalFrames + 1; i++)
            {
                _dates.Add(now + TimeSpan.FromMilliseconds(300 * i));
            }

            _spans = new List<TimeSpan>() { TimeSpan.FromMilliseconds(25) };

            _timingObj.Now.Returns(_dates[0], _dates[1], _dates[2], _dates[3]);
            _timingObj.Elapsed.Returns(_spans[0]);
            double average = _spans.Average(x => x.Milliseconds);

            //Act
            for (int i = 0; i < totalFrames + 1; i++)
            {
                _asset.Begin();
                _asset.Finalize(false);
            }

            //Assert
            Assert.AreEqual(success, _asset.DetectedFPS);
            Assert.AreEqual(totalFrames, _asset.TotalFPS);
            Assert.AreEqual(average, _asset.AverageDetectionTime);
            Assert.AreEqual(0, (decimal)_asset.DetectionRate);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void BeginFinalizeTest_SuccessAllOfFive()
        {
            //Arrange
            decimal success = 5;
            decimal totalFrames = 5;

            _dates = new List<DateTime>();
            DateTime now = new DateTime(1, 1, 1, 1, 1, 0, 550);
            for (int i = 0; i < totalFrames + 1; i++)
            {
                _dates.Add(now + TimeSpan.FromMilliseconds(100 * i));
            }

            _spans = new List<TimeSpan>() { TimeSpan.FromMilliseconds(25),
                                            TimeSpan.FromMilliseconds(10),
                                            TimeSpan.FromMilliseconds(9),
                                            TimeSpan.FromMilliseconds(5),
                                            TimeSpan.FromMilliseconds(30) };

            _timingObj.Now.Returns(_dates[0], _dates[1], _dates[2], _dates[3], _dates[4], _dates[5] );
            _timingObj.Elapsed.Returns(_spans[0], _spans[1], _spans[2], _spans[3], _spans[4]);
            double average = _spans.Average(x => x.Milliseconds);

            //Act
            for (int i = 0; i < totalFrames + 1; i++)
            {
                _asset.Begin();
                _asset.Finalize(true);
            }

            //Assert
            Assert.AreEqual(success, _asset.DetectedFPS);
            Assert.AreEqual(totalFrames, _asset.TotalFPS);
            Assert.AreEqual(Math.Truncate(average), _asset.AverageDetectionTime);
            Assert.AreEqual(100, (decimal)_asset.DetectionRate);
        }

        #endregion BeginFinalizeTests
    }
}
