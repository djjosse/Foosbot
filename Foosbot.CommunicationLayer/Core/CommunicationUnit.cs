﻿// **************************************************************************************
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
using Foosbot.Common.Exceptions;
using Foosbot.Common.Logs;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.CommunicationLayer.Contracts;
using System;
using System.Threading;

namespace Foosbot.CommunicationLayer.Core
{
    /// <summary>
    /// Communication Unit to observe the publisher of RodAction and sent commands to Arduino Rod
    /// </summary>
    public class CommunicationUnit : Observer<RodAction>
    {
        #region private members

        /// <summary>
        /// Converter of ticks to bits
        /// </summary>
        private IRodConverter _converter;

        /// <summary>
        /// Arduino Communication Unit
        /// </summary>
        private ISerialController _arduino;

        /// <summary>
        /// Current rod type
        /// </summary>
        private eRod _rodType;

        /// <summary>
        /// Rod length in mm
        /// </summary>
        private double _rodLength;

        /// <summary>
        /// Rod length in ticks
        /// </summary>
        private int _ticksPerRod;

        /// <summary>
        /// Is Initialized member
        /// </summary>
        private bool _isInitialized = false;

        #endregion private members

        /// <summary>
        /// Communication Layer Constructor
        /// </summary>
        /// <param name="publisher">Rod Action publisher to observe</param>
        /// <param name="rodType">Current rod type</param>
        /// <param name="comPort">Com port of current arduino rod</param>
        public CommunicationUnit(Publisher<RodAction> publisher, eRod rodType, string comPort, Action<eRod, eRotationalMove> onServoChangeState)
            : base(publisher)
        {
            //we don't wan't to receive anything before initialization finished
            _publisher.Detach(this);

            //set rod type for current rod
            _rodType = rodType;

            _converter = new RodActionConverter(rodType);

            //Create arduino com object
            _arduino = new ArduinoController(comPort, new ActionEncoder(_converter))
            {
                OnServoChangeState = onServoChangeState,
                RodType = rodType
            };
        }

        /// <summary>
        /// Initialization method. 
        /// Open arduino and initialize it. Reads configuration file.
        /// </summary>
        public void InitializeRod()
        {
            if (!_isInitialized)
            {
                //Create and Initialize Arduino to work with
                _arduino.OpenSerialPort();
                _arduino.Initialize();

                //get rod length in mm and ticks from configuration file
                _rodLength = Configuration.Attributes.GetRodDistanceBetweenStoppers(_rodType);
                _ticksPerRod = Configuration.Attributes.GetTicksPerRod(_rodType);

                _arduino.MaxTicks = _ticksPerRod;

                _isInitialized = true;

                //subscribe to new commands
                _publisher.Attach(this);
            }
        }

        /// <summary>
        /// Main Flow for communication unit
        /// </summary>
        public override void Job()
        {
            try
            {
                if (!_isInitialized)
                    throw new InitializationException(String.Format(
                        "[{0}] Instance was not initialized. Must call initialization method before used!",
                            this.GetType().DeclaringType));

                //we don't wan't to receive new data while not finished with old one
                _publisher.Detach(this);

                //received command
                RodAction action = _currentData;// _publisher.Data;

                //Convert mm to ticks
                int proportinalMove = _converter.MmToTicks(action.DcCoordinate);

                //Invoke the arduino
                _arduino.Move(proportinalMove, action.Rotation);

            }
            catch (Exception ex)
            {
                if (ex is ThreadInterruptedException)
                {
                    /* Got new data */
                }
                else if (ex is InitializationException)
                {
                    throw ex;
                }
                else
                {
                    Log.Print(String.Format("Error occurred! Reason: [{0}]", ex.Message), eCategory.Error, LogTag.COMMUNICATION);
                }
            }
            finally
            {
                _publisher.Attach(this);
            }
        }

        
    }
}
