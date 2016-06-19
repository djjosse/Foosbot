// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Emgu.CV.Structure;
using Foosbot.Common.Contracts;
using Foosbot.Common.Enums;
using Foosbot.Common.Exceptions;
using Foosbot.ImageProcessingUnit.Tools.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessingUnitTest.Tools.Core
{
    [TestClass]
    public class CalibrationUnitTest
    {
        private const string CATEGORY = "CalibrationUnit";

        static CalibrationHelper _asset;
        static Dictionary<eCallibrationMark, CircleF> _marksFromImage = new Dictionary<eCallibrationMark, CircleF>()
            { 
                { eCallibrationMark.BL, new CircleF(new PointF(11,500), 10) },
                { eCallibrationMark.TL, new CircleF(new PointF(10,15), 10) },
                { eCallibrationMark.TR, new CircleF(new PointF(800,20), 10) },
                { eCallibrationMark.BR, new CircleF(new PointF(805,525), 10) }
            };

        [TestInitialize]
        public void InitTests()
        {
            _asset = new CalibrationHelper();
        }

        #region CalculateBallRadiusAndError

        [TestCategory(CATEGORY), TestMethod]
        public void CalculateBallRadiusAndError_Positive()
        {
            //Arrange
            _asset.SetTransformationMatrix(1000, 500, _marksFromImage);

            float originalRadius = 20;
            int actualRadius;
            double actualError;
            int expectedRadius = 16;
            double expectedError = 0.105;

            //Act
            _asset.CalculateBallRadiusAndError(originalRadius, out actualRadius, out actualError);
            
            //Assert
            Assert.AreEqual(expectedRadius, actualRadius);
            Assert.AreEqual(expectedError, actualError);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void CalculateBallRadiusAndError_Negative()
        {
            float originalRadius = 20;
            int actualRadius;
            double actualError;
            _asset.CalculateBallRadiusAndError(originalRadius, out actualRadius, out actualError);
        }

        #endregion CalculateBallRadiusAndError

        #region FindDiagonalMarkPairs

        [TestCategory(CATEGORY), TestMethod]
        public void FindDiagonalMarkPairs_Positive()
        {
            //Act
            Dictionary<CircleF, CircleF> result = _asset.FindDiagonalMarkPairs(_marksFromImage.Values.ToList());
            
            //Assert
            Assert.AreEqual(2, result.Count());
            int count = 0;
            foreach (var key in result.Keys)
            {
                foreach(var eMark in _marksFromImage.Keys)
                {
                    if (key.Equals(_marksFromImage[eMark]))
                    {
                        count++;
                        switch(eMark)
                        {
                            case eCallibrationMark.BL:
                                Assert.AreEqual(_marksFromImage[eCallibrationMark.TR], result[key]);
                                count++;
                                break;
                            case eCallibrationMark.BR:
                                Assert.AreEqual(_marksFromImage[eCallibrationMark.TL], result[key]);
                                break;
                            case eCallibrationMark.TL:
                                Assert.AreEqual(_marksFromImage[eCallibrationMark.BR], result[key]);
                                break;
                            case eCallibrationMark.TR:
                                Assert.AreEqual(_marksFromImage[eCallibrationMark.BL], result[key]);
                                break;
                            default:
                                throw new Exception("Unknown mark: " + eMark.ToString());
                        }
                    }
                }
            }
            Assert.AreEqual(2, count);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(CalibrationException))]
        public void FindDiagonalMarkPairs_NegativeNotAllMarks()
        {
            List<CircleF> marks = new List<CircleF>()
            {
                _marksFromImage[eCallibrationMark.TL],
                _marksFromImage[eCallibrationMark.TR]
            };
            //Act
            Dictionary<CircleF, CircleF> result = _asset.FindDiagonalMarkPairs(marks);
            
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(CalibrationException))]
        public void FindDiagonalMarkPairs_NegativeNullArg()
        {
            //Act
            Dictionary<CircleF, CircleF> result = _asset.FindDiagonalMarkPairs(null);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(CalibrationException))]
        public void FindDiagonalMarkPairs_NegativeWrongPairs()
        {
            //Arrange
            List<CircleF> marks = new List<CircleF>()
            {
                _marksFromImage[eCallibrationMark.TL],
                _marksFromImage[eCallibrationMark.TR],                
                _marksFromImage[eCallibrationMark.TR],
                _marksFromImage[eCallibrationMark.TR]
            };

            //Act
            Dictionary<CircleF, CircleF> result = _asset.FindDiagonalMarkPairs(marks);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(CalibrationException))]
        public void FindDiagonalMarkPairs_NegativeBadMarks()
        {
            //Arrange
            List<CircleF> marks = new List<CircleF>()
            {
                new CircleF(new PointF(11,500), 10),
                new CircleF(new PointF(12,500), 10),
                new CircleF(new PointF(11,1000), 10),
                new CircleF(new PointF(13,500), 10)
            };

            //Act
            Dictionary<CircleF, CircleF> result = _asset.FindDiagonalMarkPairs(marks);
        }

        #endregion FindDiagonalMarkPairs

        #region SetTransformationMatrix

        [TestCategory(CATEGORY), TestMethod]
        public void SetTransformationMatrix_Positive()
        {
            ITransformation transfromationMock = Substitute.For<ITransformation>();
            _asset = new CalibrationHelper(transfromationMock);
            _asset.SetTransformationMatrix(1000, 500, _marksFromImage);
            transfromationMock.Received().Initialize(Arg.Any<PointF[]>(), Arg.Any<PointF[]>());
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetTransformationMatrix_NegativeBadAxes()
        {
            _asset = new CalibrationHelper();
            _asset.SetTransformationMatrix(-1, 0, _marksFromImage);
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(CalibrationException))]
        public void SetTransformationMatrix_NegativeBadMarks()
        {
            _asset = new CalibrationHelper();
            Dictionary<eCallibrationMark, CircleF> marks 
                = _marksFromImage.Where(x => x.Key != eCallibrationMark.TL)
                    .ToDictionary(x => x.Key, x => x.Value);
            _asset.SetTransformationMatrix(1000, 500, marks);
        }

        #endregion SetTransformationMatrix

        #region VerifyMarksFound
        #endregion VerifyMarksFound

        #region UpdateCoverage
        #endregion UpdateCoverage

    }
}
