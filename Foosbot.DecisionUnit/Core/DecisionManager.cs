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
using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using Foosbot.VectorCalculation;
using Foosbot.VectorCalculation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Decisides on actions to be taken
    /// </summary>
    public class DecisionManager : IInitializableDecisionManager
    {
        /// <summary>
        /// Mechanical, Calculation and other delays
        /// </summary>
        private TimeSpan SystemDelays;

        /// <summary>
        /// Converts mm to pts and back, stores table sizes
        /// </summary>
        private ISurveyor _surveyor;

        /// <summary>
        /// Predictor for ball coordinates
        /// </summary>
        private IPredictor _predictor;

        /// <summary>
        /// Rods to control
        /// </summary>
        private List<IInitializableRod> _controlledRods;

        private IDecisionTree _decisionTree;

        /// <summary>
        /// Decision Manager Constructor
        /// </summary>
        /// <param name="surveyor">Convertion and table size storage unit. 
        /// (If null [default] - will be instantiated in constructor)</param>
        /// <param name="ricochetCalc">Ricochet calc unit. NOTE: working in mm. 
        /// (If null [default] - will be instantiated in constructor)</param>
        /// <param name="predictor">Predictor for ball coordinates
        /// (If null [default] - will be instantiated in constructor)</param>
        /// <param name="decisionTree">Full decision tree to make decisions per eacch rod.
        /// (If null [default] - will be instantiated in constructor)</param>
        /// <param name="controlledRods">Rods to be controlled by manager
        /// (If null [default] - will be instantiated in constructor)</param>
        public DecisionManager(ISurveyor surveyor = null, IInitializableRicochet ricochetCalc = null, IPredictor predictor = null, IDecisionTree decisionTree = null, List<IInitializableRod> controlledRods = null)
        {
            _surveyor = (surveyor != null) ?  surveyor : new Surveyor();

            IInitializableRicochet ricochetCalculator = (ricochetCalc != null) ? ricochetCalc : new RicochetCalc(true, eUnits.Mm);

            _decisionTree = (decisionTree != null) ? decisionTree : new FullDecisionTree(new PartialDecisionTree());

            _predictor = (predictor != null) ? predictor : new Predictor(_surveyor, ricochetCalculator);

            //use given rods if not null
            if (controlledRods != null)
            {
                _controlledRods = controlledRods;
            }
            //create new rods and init them
            else
            {
                _controlledRods = new List<IInitializableRod>();
                foreach (eRod type in Enum.GetValues(typeof(eRod)))
                {
                    IInitializableRod rod = new ControlRod(type, _surveyor, ricochetCalculator);
                    rod.Initialize();
                    _controlledRods.Add(rod);
                }
            }
        }

        /// <summary>
        /// Decide on action to be taken for each rod
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates and vector</param>
        /// <returns>List of actions per each rod</returns>
        public List<RodAction> Decide(BallCoordinates currentCoordinates)
        {
            if (!IsInitialized) Initialize();

            if (currentCoordinates == null)
                throw new ArgumentException(String.Format("[{0}] Coordinates received from vector calculation unit are null",
                    MethodBase.GetCurrentMethod().Name));

            //Convert pts and pts/sec to mm and mm/sec
            currentCoordinates = _surveyor.PtsToMm(currentCoordinates);

            //Calculate Actual Possible Action Time
            DateTime timeOfAction = DateTime.Now + SystemDelays;

            //Calculate ball future coordinates
            BallCoordinates bfc = _predictor.FindBallFutureCoordinates(currentCoordinates, timeOfAction);

            List<RodAction> actions = new List<RodAction>();

            foreach(IRod rod in _controlledRods)
            {
                //Calculate dynamic sectors
                rod.CalculateDynamicSector(currentCoordinates);

                //Draw dynamic sector, better to use on one rod at a time because together is chaos on the screen
                Marks.DrawSector(rod.RodType, rod.DynamicSector);
                
                //Calculate intersection point 
                rod.CalculateSectorIntersection(bfc);
                //Decide on action
                RodAction action = _decisionTree.Decide(rod, bfc);
                actions.Add(action);
                //if (rod.RodType == eRod.GoalKeeper) Log.Common.Debug(action.Linear + ": " + action.DcCoordinate);
            }
            return actions;
        }

        #region Initialization Related

        /// <summary>
        /// Initialize Method
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                SystemDelays = TimeSpan.FromMilliseconds(Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_DELAY));
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Initialize with parameters
        /// </summary>
        /// <param name="systemDelays">Mechanical, Calculation and Networking system delays in ms</param>
        public void Initialize(int systemDelays)
        {
            if (!IsInitialized)
            {
                SystemDelays = TimeSpan.FromMilliseconds(systemDelays);
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Is Initialized Property
        /// </summary>
        public bool IsInitialized { get; private set; }

        #endregion Initialization Related
    }
}
