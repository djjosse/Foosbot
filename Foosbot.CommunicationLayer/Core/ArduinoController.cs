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
using Foosbot.Common.Enums;
using Foosbot.CommunicationLayer.Contracts;
using EasyLog;
using Foosbot.Common.Logs;
using System.Threading;
using System.Runtime.CompilerServices;
using Foosbot.CommunicationLayer.Enums;

namespace Foosbot.CommunicationLayer.Core
{
    /// <summary>
    /// Arduino Communication Unit
    /// This unit is responsible for connection to arduino serial port
    /// </summary>
    public class ArduinoController : ISerialController
    {
        #region constants

        /// <summary>
        /// Maximum number of retries opening arduino communication port
        /// </summary>
        private const int MAX_OPEN_PORT_RETRIES = 3;

        /// <summary>
        /// Wait time before retrying to open arduino communication port on fail
        /// </summary>
        private readonly TimeSpan WAIT_ON_FAIL_OPENING_PORT = TimeSpan.FromMilliseconds(1000);

        #endregion constants

        #region private members

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

        /// <summary>
        /// Arduino feedback thread
        /// </summary>
        private Thread _readThread;

        
        #endregion private members

        /// <summary>
        /// Delegate for a Method to be called on arduino servo state changed
        /// </summary>
        public Action<eRod, eRotationalMove> OnServoChangeState { get; set; }

        /// <summary>
        /// Rod Type of current controller
        /// </summary>
        public eRod RodType { get; set; }

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comPort">COM port to open as string</param>
        /// <param name="encoder">Encoder for actions</param>
        public ArduinoController(string comPort, IEncoder encoder)
        {
            _comPort = null;
            ComPortName = comPort;
            _encoder = encoder;
            _isInitialized = false;
            MaxTicks = 0;
        }

        /// <summary>
        /// Constructor receives ready serial port (USED in UT)
        /// </summary>
        /// <param name="openPort"></param>
        /// <param name="encoder">Encoder for actions</param>
        public ArduinoController(ISerialPort openPort, IEncoder encoder)
        {
            _comPort = openPort;
            _encoder = encoder;
            _isInitialized = false;
            MaxTicks = 0;
        }

        #endregion Constructors

        /// <summary>
        /// Current COM port name
        /// </summary>
        public string ComPortName { get; private set; }

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
                        MethodBase.GetCurrentMethod().Name, ComPortName));

            InvokeReadingFromArduino();

            RequestCalibration();

            _isInitialized = true;
        }

        /// <summary>
        /// Calibrate arduino method
        /// </summary>
        public void RequestCalibration()
        {
            Log.Print(String.Format("Calibrating Arduino with initialization byte on port {0}...", ComPortName), eCategory.Info, LogTag.COMMUNICATION);

            try
            {
                byte initByte = _encoder.EncodeInitialization();
                _comPort.Write(initByte);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format(
                    "[{0}] Unable to initialize arduino. Reason: {1}",
                        MethodBase.GetCurrentMethod().Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Invoke reading thread from arduino communication port
        /// </summary>
        private void InvokeReadingFromArduino()
        {
            _readThread = new Thread(() =>
            {
                Read();
            });
            _readThread.IsBackground = true;
            _readThread.Start();
        }

        /// <summary>
        /// Open Arduino Com Port
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown in case of error while opening port</exception>
        public void OpenSerialPort()
        {
            if (_comPort == null)
                _comPort = new SerialPortWrapper(ComPortName, Communication.BAUDRATE);

            int currentRetry = 0;
            while (currentRetry < MAX_OPEN_PORT_RETRIES)
            {
                try
                {
                    Log.Print(String.Format("Opening Arduino port {0}...", ComPortName), eCategory.Info, LogTag.COMMUNICATION);
                    _comPort.Open();
                    Log.Print(String.Format("Arduino port {0} is open!", ComPortName), eCategory.Info, LogTag.COMMUNICATION);
                    return;
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    if (currentRetry < MAX_OPEN_PORT_RETRIES)
                    {
                        Log.Print(String.Format("Unable to open arduino port {0}, will retry after {1}. [Retry {2} of {3}]",
                            _comPort, WAIT_ON_FAIL_OPENING_PORT, currentRetry, MAX_OPEN_PORT_RETRIES), eCategory.Warn, LogTag.COMMUNICATION);
                        Thread.Sleep(WAIT_ON_FAIL_OPENING_PORT);
                    }
                    else
                    {
                         throw new InvalidOperationException(String.Format("[{0}] Error opening Arduino port {1}. Reason: {2}",
                            MethodBase.GetCurrentMethod().Name, ComPortName, ex.Message), ex);
                    }
                }
                
            }
        }

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
                    byte command = _encoder.Encode(dc, servo);
                    _comPort.Write(command);
                    Log.Print(String.Format("[Local: {0}] DC: {1} SERVO: {2}", ComPortName, dc, servo.ToString()), eCategory.Info, LogTag.COMMUNICATION);
                    
                    _lastDc = dc;
            }
        }

        /// <summary>
        /// Update last arduino servo state
        /// </summary>
        /// <param name="newState">Servo state as arduino response code</param>
        public void SetLastServoState(eResponseCode newState)
        {
            eRotationalMove state = ResponseCodeToServoState(newState);

            if (!state.Equals(eRotationalMove.NA))
            {
                _lastServo = state;
                if (OnServoChangeState != null)
                {
                    OnServoChangeState(RodType, _lastServo);
                }
            }
        }

        /// <summary>
        /// Read received data from COM port
        /// </summary>
        public void Read()
        {
            while (true)
            {
                try
                {
                    byte inputByte = _comPort.ReadByte();
                    eResponseCode code = (eResponseCode)inputByte;
                    switch (code)
                    {
                        case eResponseCode.INIT_REQUERED:
                            PrintArduinoResponse(code, eCategory.Info);
                            RequestCalibration();
                            break;
                        case eResponseCode.INIT_REQUESTED:
                        case eResponseCode.INIT_STARTED:
                        case eResponseCode.INIT_FINISHED:
                        case eResponseCode.DC_CALIBRATED:
                            PrintArduinoResponse(code, eCategory.Info);
                            break;
                        case eResponseCode.SERVO_STATE_KICK:
                        case eResponseCode.SERVO_STATE_RISE:
                        case eResponseCode.SERVO_STATE_DEFENCE:
                            SetLastServoState(code);
                            PrintArduinoResponse(code, eCategory.Info);
                            break;
                        case eResponseCode.DC_RECEIVED_OK:
                            PrintArduinoResponse(code, eCategory.Debug);
                            break;
                        case eResponseCode.DC_RANGE_INVALID:
                            PrintArduinoResponse(code, eCategory.Error);
                            break;
                        default:
                            Log.Print(String.Format("[Remote: {0}]: Unknown code: {1}", ComPortName, inputByte),
                                eCategory.Warn, LogTag.ARDUINO);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Print(ex + ex.Message, eCategory.Error, LogTag.COMMUNICATION);
                }
            }
        }

        /// <summary>
        /// Print received code to log
        /// </summary>
        /// <param name="code">Received code</param>
        /// <param name="category">Print category</param>
        private void PrintArduinoResponse(eResponseCode code, eCategory category,
            [CallerMemberName]string method = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (code != eResponseCode.NO_DATA)
            {
                String message = String.Format("[Remote: {0}]: {1}", ComPortName, code.ToString());
                Log.Print(message, category, LogTag.ARDUINO, method, sourceFile, lineNumber);
            }
        }

        /// <summary>
        /// Convert arduino response code to state
        /// </summary>
        /// <param name="code">Arduino response code</param>
        /// <returns>Servo state</returns>
        private eRotationalMove ResponseCodeToServoState(eResponseCode code)
        {
            eRotationalMove state = eRotationalMove.NA;
            switch (code)
            {
                case eResponseCode.SERVO_STATE_KICK:
                    state = eRotationalMove.KICK;
                    break;
                case eResponseCode.SERVO_STATE_RISE:
                    state = eRotationalMove.RISE;
                    break;
                case eResponseCode.SERVO_STATE_DEFENCE:
                    state = eRotationalMove.DEFENCE;
                    break;
            }
            return state;
        }
    }

}