// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using EasyLog;
using Foosbot.Common.Enums;
using Foosbot.Common.Logs;
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
            _manager = decisionManager ?? new DecisionManager();
        }

        /// <summary>
        /// Initialize publishers and start decision contentious process
        /// </summary>
        public override void Start()
        {
            InitializeActionPublishers();
            base.Start();
        }

        /// <summary>
        /// Performed in different thread in loop task each time
        /// we receive new coordinates and vector
        /// </summary>
        public override void Job()
        {
            BallCoordinates ballCoordinates = null;
            List<RodAction> actions = null;
            try
            {
                _publisher.Detach(this);
                ballCoordinates = _currentData;//_publisher.Data;
                actions = _manager.Decide(ballCoordinates);
                foreach (RodAction action in actions)
                {
                    RodActionPublishers[action.RodType].UpdateAndNotify(action);

                    Marks.DrawRods();
                    
                    /* NOTE: Marks of player postions will be drawn from main decision unit */

                    Log.Print(String.Format("Decision per rod {0}: {1}:{2}; {3}", 
                        action.RodType, action.Linear, action.DcCoordinate, action.Rotation),
                            eCategory.Debug, LogTag.DECISION);

                }
            }
            catch (Exception ex)
            {
                Log.Print(ex.Message + ": No vectors found!", eCategory.Error, LogTag.DECISION);
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

        public void UpdateRealTimeState(eRod rodType, eRotationalMove servoState)
        {
            _manager.UpdateRealTimeState(rodType, servoState);
        }
    }
}
