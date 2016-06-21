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
using System;
using System.Collections.Generic;

namespace Foosbot.CommunicationLayer.Core
{
    /// <summary>
    /// Factory responsible for creating Arduino communication units per each connected controller
    /// </summary>
    public class CommunicationFactory
    {
        /// <summary>
        /// Arduino Goal Keeper rod Serial Com Port
        /// </summary>
        private const string ARDUINO_GOAL_KEEPER_PORT = "COM3";

        /// <summary>
        /// Arduino Defense rod Serial Com Port
        /// </summary>
        private const string ARDUINO_DEFENCE_PORT = "COM5";

        /// <summary>
        /// Create Communication Layer for each connected Arduino
        /// </summary>
        /// <param name="publishers">Dictionary of RodActionPublishers per each rod</param>
        /// <returns>Communication Layer per each Rod</returns>
        public static Dictionary<eRod, CommunicationUnit> Create(Dictionary<eRod, RodActionPublisher> publishers, Action<eRod, eRotationalMove> onServoChangeState)
        {
            Dictionary<eRod, CommunicationUnit> allArduinos = new Dictionary<eRod,CommunicationUnit>();

            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                allArduinos.Add(rodType, null);
            }

            //TODO: Decide how to get all
            //string[] portsList = SerialPort.GetPortNames();
            //if (portsList.Length < 1) //change to 4
            //    throw new NotSupportedException("Verify arduino is connected!");

            //Ignore the Goal Keeper Rod till it will work

           // allArduinos[eRod.GoalKeeper] = new CommunicationUnit(publishers[eRod.GoalKeeper], eRod.GoalKeeper, ARDUINO_GOAL_KEEPER_PORT);
          //  allArduinos[eRod.GoalKeeper].InitializeRod();

            allArduinos[eRod.Defence] = new CommunicationUnit(publishers[eRod.Defence], eRod.Defence, ARDUINO_DEFENCE_PORT, onServoChangeState);
            allArduinos[eRod.Defence].InitializeRod();

            return allArduinos;
        }
    }
}
