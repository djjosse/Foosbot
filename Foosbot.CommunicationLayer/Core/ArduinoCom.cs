// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using System;
using System.Reflection;
using Foosbot.Common.Contracts;
using Foosbot.Common.Exceptions;
using System.Diagnostics;
using Foosbot.Common.Enums;
using Foosbot.CommunicationLayer.Contracts;
using EasyLog;
using Foosbot.Common.Logs;

namespace Foosbot.CommunicationLayer.Core
{
    public class ArduinoCom : IInitializable
    {
        #region private members

        /// <summary>
        /// Current COM port name
        /// </summary>
        private string _comPortName;

        /// <summary>
        /// Current COM port
        /// </summary>
        private ISerialPort _comPort;

        /// <summary>
        /// Commands encoder
        /// </summary>
        private IEncoder _encoder;

        /// <summary>
        /// Flag for Arduino [TRUE] if initialized, [FALSE] otherwise
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Last known servo position
        /// </summary>
        private eRotationalMove _lastServo = eRotationalMove.NA;

        /// <summary>
        /// Last known DC position (in ticks, -1 is for undefined)
        /// </summary>
        private int _lastDc = -1;

        #endregion private members

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comPort">COM port to open as string</param>
        /// <param name="encoder">Encoder for actions</param>
        public ArduinoCom(string comPort, IEncoder encoder)
        {
            _comPort = null;
            _comPortName = comPort;
            _encoder = encoder;
            _isInitialized = false;
            MaxTicks = 0;
        }

        /// <summary>
        /// Constructor receives ready serial port (USED in UT)
        /// </summary>
        /// <param name="openPort"></param>
        /// <param name="encoder">Encoder for actions</param>
        public ArduinoCom(ISerialPort openPort, IEncoder encoder)
        {
            _comPort = openPort;
            _encoder = encoder;
            _isInitialized = false;
            MaxTicks = 0;
        }

        #endregion Constructors

        /// <summary>
        /// Maximum ticks per current rod
        /// </summary>
        public int MaxTicks { get; set; }

        /// <summary>
        /// [True] if initialized, [False] otherwise
        /// </summary>
        public bool IsInitialized 
        {
            get
            {
                return _isInitialized;
            }
        }

        /// <summary>
        /// Arduino Initialization Method
        /// </summary>
        public void Initialize()
        {
            if (!_comPort.IsOpen)
                throw new InvalidOperationException(String.Format(
                    "[{0}] Unable to initialize arduino because the port {1} is closed!",
                        MethodBase.GetCurrentMethod().Name, _comPortName));

            Log.Print(String.Format("Initializing Arduino with initialization byte on port {0}...", _comPortName), eCategory.Error, LogTag.COMMUNICATION);

            try
            {
                byte initByte = _encoder.EncodeInitialization();
                _comPort.Write(initByte);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(String.Format(
                    "[{0}] Unable to initialize arduino. Reason: {1}",
                        MethodBase.GetCurrentMethod().Name, ex.Message), ex);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Open Arduino Com Port
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown in case of error while opening port</exception>
        public void OpenArduinoComPort()
        {
            if (_comPort == null)
                _comPort = new SerialPortWrapper(_comPortName, Communication.BAUDRATE);
            try
            {
                Log.Print(String.Format("Opening Arduino port {0}...", _comPortName), eCategory.Info, LogTag.COMMUNICATION);
                _comPort.Open();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("[{0}] Error opening Arduino port {1}. Reason: {2}",
                        MethodBase.GetCurrentMethod().Name, _comPortName, ex.Message), ex);
            }
            Log.Print(String.Format("Arduino port {0} is open!", _comPortName), eCategory.Info, LogTag.COMMUNICATION);
        }

        Stopwatch watch = null;

        /// <summary>
        /// Move arduino DC and Servo
        /// </summary>
        /// <param name="dc">DC movement (-1) for no movement</param>
        /// <param name="servo">Servo Position</param>
        /// <exception cref="InitializationException">Thrown in case arduino was not initialized</exception>
        /// <exception cref="InvalidOperationException">Thrown in of wrong DC parameter</exception>
        public void Move(int dc = -1, eRotationalMove servo = eRotationalMove.NA)
        {
            if (!_isInitialized)
                throw new InitializationException(String.Format(
                   "[{0}] Unable to move arduino because arduino is not initialized.",
                        MethodBase.GetCurrentMethod().Name));

            if (dc < -1 || dc > MaxTicks)
                throw new InvalidOperationException(String.Format(
                   "[{0}] Unable to move arduino because received DC movement: [{1}] is not in range of rod: [{2} to {3}]",
                        MethodBase.GetCurrentMethod().Name, dc, 0, MaxTicks));

            
            if (_lastServo != servo || _lastDc != dc)
            {
                if (watch == null || watch.ElapsedMilliseconds > Communication.SLEEP)
                {
                    Log.Print(String.Format("{0} DC: {1} SERVO: {2}", _comPortName, dc, servo.ToString()), eCategory.Info, LogTag.COMMUNICATION);

                    watch = Stopwatch.StartNew();
                    byte command = _encoder.Encode(dc, servo);
                    _comPort.Write(command);
                    _lastServo = servo;
                    _lastDc = dc;
                }
            }
        }
    }

}