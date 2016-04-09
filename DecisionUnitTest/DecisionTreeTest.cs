using Foosbot;
using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DecisionUnitTest
{
    [TestClass]
    public class DecisionTreeTest
    {
        static int MIDFIELD_ROD_X;

        /// <summary>
        /// Initialize Configuration file once to be used in all tests
        /// </summary>
        /// <param name="context"></param>
        [ClassInitialize]
        public static void DecisionTreeClassInit(TestContext context)
        {
            Configuration.Attributes.IsKeyExist(Configuration.Names.KEY_IS_DEMO_MODE);
            MIDFIELD_ROD_X = Configuration.Attributes.GetValue<int>(eRod.Midfield.ToString());
        }

        /// <summary>
        /// Tested class instance
        /// </summary>
        DecisionTree _tree;


        [TestInitialize]
        public void DecisionTreeTestInit()
        {
            _tree = new DecisionTree();
        }

        #region DefineActionAndRespondingPlayer

        const string DEFINE_ACTION_AND_RESPONDING_PLAYER = "DefineActionAndRespondingPlayer";
        delegate RodAction DefineActionAndRespondingPlayerInternal(Rod rod, BallCoordinates bfc, out int respondingPlayer); 
        DefineActionAndRespondingPlayerInternal _defineActionAndRespondingPlayerInternal;

        private void Init_DefineActionAndRespondingPlayer()
        {
            _defineActionAndRespondingPlayerInternal = (DefineActionAndRespondingPlayerInternal)Delegate.CreateDelegate(
                typeof(DefineActionAndRespondingPlayerInternal),
                _tree,
                typeof(DecisionTree).GetMethod(DEFINE_ACTION_AND_RESPONDING_PLAYER, BindingFlags.NonPublic | BindingFlags.Instance));
        }

        [TestMethod]
        public void DefineActionAndRespondingPlayer_BallSpeedZeroIsAheadOfGoalKeaper_DefenceBestEffort()
        {
            Init_DefineActionAndRespondingPlayer();

            //ball coordinates define in mm
            BallCoordinates ballCoords = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoords.Vector = new Vector2D(0, 0);

            Rod rod = new Rod(eRod.GoalKeeper);
            rod.CalculateDynamicSector(ballCoords);

            int respondingPlayer;

            _tree.DefineSectorStartAndEnd(rod);
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);

            Assert.AreEqual(eRotationalMove.DEFENCE, actualResult.Rotation);
            Assert.AreEqual(eLinearMove.BEST_EFFORT, actualResult.Linear);
        }

        [TestMethod]
        public void DefineActionAndRespondingPlayer_BallSpeedZeroIsAheadOfDefence_DefenceBestEffort()
        {
            Init_DefineActionAndRespondingPlayer();

            //ball coordinates define in mm
            BallCoordinates ballCoords = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoords.Vector = new Vector2D(0, 0);

            Rod rod = new Rod(eRod.Defence);
            rod.CalculateDynamicSector(ballCoords);

            int respondingPlayer;

            _tree.DefineSectorStartAndEnd(rod);
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);

            Assert.AreEqual(eRotationalMove.DEFENCE, actualResult.Rotation);
            Assert.AreEqual(eLinearMove.BEST_EFFORT, actualResult.Linear);
        }

        [TestMethod]
        public void DefineActionAndRespondingPlayer_BallSpeedZeroIsBehindOfAttack_DefenceBestEffort()
        {
            Init_DefineActionAndRespondingPlayer();

            //ball coordinates define in mm
            BallCoordinates ballCoords = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoords.Vector = new Vector2D(0, 0);

            Rod rod = new Rod(eRod.Attack);
            rod.CalculateDynamicSector(ballCoords);

            int respondingPlayer;

            _tree.DefineSectorStartAndEnd(rod);
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);

            Assert.AreEqual(eRotationalMove.RISE, actualResult.Rotation);
            Assert.AreEqual(eLinearMove.BEST_EFFORT, actualResult.Linear);
        }

        [TestMethod]
        public void DefineActionAndRespondingPlayer_BallVectorFromRodIsAheadOfDefence_DefenceBestEffort()
        {
            Init_DefineActionAndRespondingPlayer();

            //ball coordinates define in mm
            BallCoordinates ballCoords = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoords.Vector = new Vector2D(20, 10);

            Rod rod = new Rod(eRod.Defence);
            rod.CalculateDynamicSector(ballCoords);

            int respondingPlayer;

            _tree.DefineSectorStartAndEnd(rod);
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);

            Assert.AreEqual(eRotationalMove.DEFENCE, actualResult.Rotation);
            Assert.AreEqual(eLinearMove.BEST_EFFORT, actualResult.Linear);
        }

        [TestMethod]
        public void DefineActionAndRespondingPlayer_BallVectorToRodIsAheadOfDefence_DefenceVectorBased()
        {
            Init_DefineActionAndRespondingPlayer();

            //ball coordinates define in mm
            BallCoordinates ballCoords = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoords.Vector = new Vector2D(-20, 10);

            Rod rod = new Rod(eRod.Defence);
            rod.CalculateDynamicSector(ballCoords);

            int respondingPlayer;

            _tree.DefineSectorStartAndEnd(rod);

            rod.SetBallIntersection(100, 100, DateTime.Now + TimeSpan.FromSeconds(2));
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);

            Assert.AreEqual(eRotationalMove.DEFENCE, actualResult.Rotation);
            Assert.AreEqual(eLinearMove.VECTOR_BASED, actualResult.Linear);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DefineActionAndRespondingPlayer_BallCoordinatesNull()
        {
            Init_DefineActionAndRespondingPlayer();
            Rod rod = new Rod(eRod.Defence);
            BallCoordinates ballCoords = null;
            int respondingPlayer;
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DefineActionAndRespondingPlayer_BallCoordinatesUndefined()
        {
            Init_DefineActionAndRespondingPlayer();
            Rod rod = new Rod(eRod.Defence);
            BallCoordinates ballCoords = new BallCoordinates(DateTime.Now);
            int respondingPlayer;
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);
        }

        [TestMethod]
        public void DefineActionAndRespondingPlayer_BallVectorNullIsAheadOfDefence_DefenceBestEffort()
        {
            Init_DefineActionAndRespondingPlayer();

            //ball coordinates define in mm
            BallCoordinates ballCoords = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoords.Vector = null;

            BallCoordinates ballCoordsForDynamicSectorCalc = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoordsForDynamicSectorCalc.Vector = new Vector2D();

            Rod rod = new Rod(eRod.Defence);
            rod.CalculateDynamicSector(ballCoordsForDynamicSectorCalc);

            int respondingPlayer;

            _tree.DefineSectorStartAndEnd(rod);
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);

            Assert.AreEqual(eRotationalMove.DEFENCE, actualResult.Rotation);
            Assert.AreEqual(eLinearMove.BEST_EFFORT, actualResult.Linear);
        }

        [TestMethod]
        public void DefineActionAndRespondingPlayer_BallVectorUndefinedIsBehindOfAttack_DefenceBestEffort()
        {
            Init_DefineActionAndRespondingPlayer();

            //ball coordinates define in mm
            BallCoordinates ballCoords = new BallCoordinates(MIDFIELD_ROD_X, 200, DateTime.Now);
            ballCoords.Vector = new Vector2D();

            Rod rod = new Rod(eRod.Attack);
            rod.CalculateDynamicSector(ballCoords);

            int respondingPlayer;

            _tree.DefineSectorStartAndEnd(rod);
            RodAction actualResult = _defineActionAndRespondingPlayerInternal.Invoke(rod, ballCoords, out respondingPlayer);

            Assert.AreEqual(eRotationalMove.RISE, actualResult.Rotation);
            Assert.AreEqual(eLinearMove.BEST_EFFORT, actualResult.Linear);
        }

        #endregion DefineActionAndRespondingPlayer


    }
}
