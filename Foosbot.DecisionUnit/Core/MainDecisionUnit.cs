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
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using Foosbot.VectorCalculation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Decision Unit Main Class
    /// </summary>
    public class MainDecisionUnit : Observer<BallCoordinates>
    {
        /// <summary>
        /// Manager which decides on action to be taken
        /// </summary>
        private IActionProvider _manager;

        /// <summary>
        /// Rod Action Publisher
        /// </summary>
        public Dictionary<eRod, RodActionPublisher> RodActionPublishers;

        /// <summary>
        /// Decision Unit Constructor
        /// </summary>
        /// <param name="vectorPublisher">Vector and Coordinates Publisher instance</param>
        /// <param name="decisionManager">Decision Manager (if null [default] will be created in constructor)</param>
        public MainDecisionUnit(Publisher<BallCoordinates> vectorPublisher, IActionProvider decisionManager = null)
            : base(vectorPublisher) 
        {
            _manager = (decisionManager != null) ? _manager : new DecisionManager();
        }

        /// <summary>
        /// Initialize publishers and start decision continious process
        /// </summary>
        public override void Start()
        {
            InitializeActionPublishers();
            base.Start();
        }

        /// <summary>
        /// Performed in defferent thread in loop task each time
        /// we receive new coordinates and vector
        /// </summary>
        public override void Job()
        {
            try
            {
                _publisher.Dettach(this);
                BallCoordinates ballCoordinates = _currentData;//_publisher.Data;
                List<RodAction> actions = _manager.Decide(ballCoordinates);
                foreach (RodAction action in actions)
                {
                    RodActionPublishers[action.RodType].UpdateAndNotify(action);

                    //Log.Common.Debug(String.Format("[{0}] New action for [{1}] Rotate: [{2}] Linear: [{3}] Coordinate: [{4}] mm",
                    //   MethodBase.GetCurrentMethod().Name, action.RodType.ToString(), action.Rotation.ToString(),
                    //      action.Linear.ToString(), action.DcCoordinate));
                }
            }
            catch (Exception ex)
            {
                Log.Common.Error(String.Format("[{0}] [{1}]",
                    MethodBase.GetCurrentMethod().DeclaringType.Name, ex.Message));
            }
            finally
            {
                _publisher.Attach(this);
            }
        }

        /// <summary>
        /// Initialize Action Publishers for Communication Layer as observer private method
        /// </summary>
        private void InitializeActionPublishers()
        {
            RodActionPublishers = new Dictionary<eRod, RodActionPublisher>();
            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                RodActionPublishers.Add(rodType, new RodActionPublisher());
            }
        }
    }
}
