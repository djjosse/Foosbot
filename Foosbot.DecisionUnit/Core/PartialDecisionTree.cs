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
using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using Foosbot.DecisionUnit.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Inner Decision Tree is used to provide action while ball is in sector
    /// </summary>
    public class PartialDecisionTree : DecisionTree
    {
        /// <summary>
        /// Delay beetween actions inside sector
        /// This is used to normalize operation of Servo motor
        /// </summary>
        private readonly TimeSpan ACTION_DELAY = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// Watches used per each rod to set actions delay for arduino servo
        /// </summary>
        private Dictionary<eRod, Stopwatch> _sectorWatch = new Dictionary<eRod, Stopwatch>();

        /// <summary>
        /// Decision Tree Constructor for tree without Subtree
        /// </summary>
        /// <param name="decisionHelper">Decision Helper Instance [default is null then will be constructed using Configuration File]</param>
        /// <param name="ballRadius">Ball Radius in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="tableWidth">Table Width (Y Axe) in mm [default is -1 will be taken from Configuration File]</param>
        /// <param name="playerWidth">Player Width in mm [default is -1 will be taken from Configuration File]</param>
        public PartialDecisionTree(IDecisionHelper helper = null, int ballRadius = -1, int tableWidth = -1, int playerWidth = -1)
            :base(helper, ballRadius, tableWidth, playerWidth)
        {
            foreach(eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                _sectorWatch.Add(rodType, null);
            }
        }

        /// <summary>
        /// Main Decision Flow Method decides on action and sets property of responding player
        /// </summary>
        /// <param name="rod">Rod to use for decision</param>
        /// <param name="bfc">Ball Future coordinates</param>
        /// <returns>Rod Action to perform</returns>
        public override RodAction Decide(IRod rod, BallCoordinates bfc)
        {
            RodAction action = new RodAction(rod.RodType);

            //Action will be ignored if not enough time passed since last request was made inside sector
            if (!IgnoreDecision(rod.RodType))
            {
                //Get relative Y position and set Responding Player
                eYPositionPlayerRelative relativeY = BallYPositionToPlayerYCoordinate(bfc.Y, rod);

                //Get relative X position
                eXPositionRodRelative relativeX = BallXPositionToRodXPosition(bfc.X, rod);

                /*
                 * Beta Version of inner DECISION TREE
                 */
                switch (relativeX)
                {
                    case eXPositionRodRelative.FRONT:
                        switch (relativeY)
                        {
                            case eYPositionPlayerRelative.RIGHT:
                            case eYPositionPlayerRelative.LEFT:
                                switch (rod.State.ServoPosition)
                                {
                                    case eRotationalMove.RISE:
                                    case eRotationalMove.DEFENCE:
                                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.BALL_Y);
                                        break;
                                    case eRotationalMove.KICK:
                                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.NA);
                                        break;
                                }
                                break;
                            case eYPositionPlayerRelative.CENTER:
                                switch (rod.State.ServoPosition)
                                {
                                    case eRotationalMove.RISE:
                                    case eRotationalMove.DEFENCE:
                                        action = new RodAction(rod.RodType, eRotationalMove.KICK, eLinearMove.NA);
                                        break;
                                    case eRotationalMove.KICK:
                                        if (_helper.IsEnoughSpaceToMove(rod, rod.State.DcPosition, BALL_RADIUS))
                                        {
                                            action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.RIGHT_BALL_DIAMETER);
                                        }
                                        else
                                        {
                                            action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.LEFT_BALL_DIAMETER);
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case eXPositionRodRelative.CENTER:
                    case eXPositionRodRelative.BACK:
                        switch (relativeY)
                        {
                            case eYPositionPlayerRelative.RIGHT:
                            case eYPositionPlayerRelative.LEFT:
                                switch (rod.State.ServoPosition)
                                {
                                    case eRotationalMove.RISE:
                                        action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.BALL_Y);
                                        break;
                                    case eRotationalMove.DEFENCE:
                                        action = new RodAction(rod.RodType, eRotationalMove.RISE, eLinearMove.NA);
                                        break;
                                    case eRotationalMove.KICK:
                                        action = new RodAction(rod.RodType, eRotationalMove.DEFENCE, eLinearMove.NA);
                                        break;
                                }
                                break;
                            case eYPositionPlayerRelative.CENTER:
                                switch (rod.State.ServoPosition)
                                {
                                    case eRotationalMove.RISE:
                                        action = new RodAction(rod.RodType, eRotationalMove.KICK, eLinearMove.NA);
                                        break;
                                    case eRotationalMove.DEFENCE:
                                    case eRotationalMove.KICK:
                                        if (_helper.IsEnoughSpaceToMove(rod, rod.State.DcPosition, BALL_RADIUS))
                                        {
                                            action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.RIGHT_BALL_DIAMETER);
                                        }
                                        else
                                        {
                                            action = new RodAction(rod.RodType, eRotationalMove.NA, eLinearMove.LEFT_BALL_DIAMETER);
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                }
                Log.Print(String.Format("Defined action for {0}: [{1}] [{2}]", rod.RodType, action.Rotation, action.Linear),
                    eCategory.Info, LogTag.DECISION);
                ActivateDelay(action);

                //Define actual desired rod coordinate to move to
                int startStopperDesiredY = CalculateNewRodCoordinate(rod, RespondingPlayer, bfc, action.Linear);
                action.DcCoordinate = rod.NearestPossibleDcPosition(startStopperDesiredY);
            }
            else
            {
                Log.Print(String.Format("Ignoring inner tree of {0} for {1} milliseconds", rod.RodType,
                    (ACTION_DELAY - _sectorWatch[rod.RodType].Elapsed).TotalMilliseconds), eCategory.Debug, LogTag.DECISION);
            }

            //Set last decided rod and player coordinates if it was defined
            if (action.Linear!=eLinearMove.NA) rod.State.DcPosition = action.DcCoordinate;
            if (action.Rotation != eRotationalMove.NA) rod.State.ServoPosition = action.Rotation;

            return action;
        }

        /// <summary>
        /// Activate delay stoppers per current rod before next action
        /// </summary>
        /// <param name="action">Action to be perfomed</param>
        private void ActivateDelay(RodAction action)
        {
            if (action.Linear != eLinearMove.NA || action.Rotation != eRotationalMove.NA)
            {
                _sectorWatch[action.RodType] = Stopwatch.StartNew();
            }
        }

        /// <summary>
        /// Provides decision if new action must be ignored based on in-sector timers
        /// </summary>
        /// <param name="rodType">Current rod type</param>
        /// <returns>[True] if operation should be ignored, [False] otherwise</returns>
        private bool IgnoreDecision(eRod rodType)
        {
            if(_sectorWatch[rodType]!=null)
            {
                return _sectorWatch[rodType].Elapsed < ACTION_DELAY;
            }
            else
            {
                return false;
            }
        }
    }
}
