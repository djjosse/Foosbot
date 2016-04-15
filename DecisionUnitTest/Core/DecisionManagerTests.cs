// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.DecisionUnit.Contracts;
using Foosbot.DecisionUnit.Core;
using Foosbot.VectorCalculation.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionUnitTest.Core
{
    [TestClass]
    public class DecisionManagerTests
    {
        private const string CATEGORY = "DecisionManager";

        private static IInitializableDecisionManager _testAsset;

        private static ISurveyor _surveyorMock = Substitute.For<ISurveyor>();
        private static IInitializableRicochet _ricochetCalcMock = Substitute.For<IInitializableRicochet>();
        private static IPredictor _predictorMock = Substitute.For<IPredictor>();
        private static IDecisionTree _decisionTreeMock = Substitute.For<IDecisionTree>();
        private static List<IInitializableRod> _rodMockList;
        private static IInitializableRod _rodGoalKeaperMock;
        private static IInitializableRod _rodDefenceMock;
        private static IInitializableRod _rodMidFieldMock;
        private static IInitializableRod _rodAttackMock;

        [ClassInitialize]
        public static void DecisionManager_ClassInitialize(TestContext context)
        {
            _surveyorMock = Substitute.For<ISurveyor>();
            _ricochetCalcMock = Substitute.For<IInitializableRicochet>();
            _predictorMock = Substitute.For<IPredictor>();
            _decisionTreeMock = Substitute.For<IDecisionTree>();
            
            _rodGoalKeaperMock = Substitute.For<IInitializableRod>();
            _rodDefenceMock = Substitute.For<IInitializableRod>();
            _rodMidFieldMock = Substitute.For<IInitializableRod>();
            _rodAttackMock = Substitute.For<IInitializableRod>();

            _rodMockList = new List<IInitializableRod>()
            {  
                _rodGoalKeaperMock, _rodDefenceMock,
                _rodMidFieldMock, _rodAttackMock
            };
        }

        [TestInitialize]
        public void DecisionManager_TestInitialize()
        {
            _testAsset = new DecisionManager(_surveyorMock, _ricochetCalcMock, _predictorMock, _decisionTreeMock, _rodMockList);
        }


        /* 
         * AR Idan: Define behaviour of each mock and test Decide() method.
         * Please use guidlines from ControlRodTest class
         */
    }
}
