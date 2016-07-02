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
    public sealed class CommunicationFactory
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static CommunicationFactory _instance;

        /// <summary>
        /// Singleton creation token
        /// </summary>
        private static object _token = new object();
        
        /// <summary>
        /// Singleton instance property
        /// </summary>
        private static CommunicationFactory Instance
        { 
            get
            {
                if (_instance == null)
                {
                    lock(_token)
                    {
                        if (_instance == null)
                        {
                            _instance = new CommunicationFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Arduino Serial Com Port Names
        /// </summary>
        private static readonly Dictionary<eRod, string> ARDUINO_PORT = new Dictionary<eRod, string>();

        /// <summary>
        /// All arduino communication ports
        /// </summary>
        private Dictionary<eRod, CommunicationUnit> _allArduinos = new Dictionary<eRod, CommunicationUnit>();

        /// <summary>
        /// Private Constructor for communication unit
        /// </summary>
        private CommunicationFactory()
        {
            foreach(eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                ARDUINO_PORT.Add(rodType, Configuration.Attributes.GetArduinoSerialPortPerRod(rodType));
            }
        }

        /// <summary>
        /// Create Communication Layer for each connected Arduino
        /// </summary>
        /// <param name="publishers">Dictionary of RodActionPublishers per each rod</param>
        /// <param name="onServoChangeState">Delegates of methods in Decision Unit to update state of Servo Position per Communication Unit</param>
        public static void Create(Dictionary<eRod, RodActionPublisher> publishers, Action<eRod, eRotationalMove> onServoChangeState)
        {
            //get operation mode from configuration file
            if (Configuration.Attributes.GetValue<bool>(Configuration.Names.KEY_IS_ARDUINOS_CONNECTED))
            {
                foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
                {
                    CommunicationFactory.Instance.Create(rodType, publishers[rodType], onServoChangeState);
                }
            }
        }

        /// <summary>
        /// Start all communications with Arduino units on all rods
        /// NOTE: This method must be called after Create method
        /// </summary>
        public static void Start()
        {
            foreach (eRod rodType in Instance._allArduinos.Keys)
            {
               Instance.Start(rodType);
            }
        }

        /// <summary>
        /// Start communication with Arduino unit on provided rod
        /// </summary>
        /// <param name="rodType">Rod type</param>
        private void Start(eRod rodType)
        {
            if (_allArduinos[rodType] != null)
                _allArduinos[rodType].Start();
        }

        /// <summary>
        /// Create Communication Port for Arduino on rod if defined
        /// </summary>
        /// <param name="rodType">Rod Type</param>
        /// <param name="publisher">Publisher for arduino actions</param>
        /// <param name="onServoChangeState">Method to call on arduino servo state changed</param>
        private void Create(eRod rodType, RodActionPublisher publisher, Action<eRod, eRotationalMove> onServoChangeState)
        {
            CommunicationUnit unit = null;
            if (Configuration.Attributes.IsServoExistsWithFeedback(rodType))
            {
                unit = new CommunicationUnit(publisher, rodType, ARDUINO_PORT[rodType], onServoChangeState);
                unit.InitializeRod();
                _allArduinos.Add(rodType, unit);
            }
        }
    }
}
