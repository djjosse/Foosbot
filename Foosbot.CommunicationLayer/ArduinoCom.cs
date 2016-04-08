using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Foosbot.Common.Protocols;
using System.Reflection;

namespace Foosbot.CommunicationLayer
{
    public class ArduinoCom
    {
        /// <summary>
        /// Initialization key to sent to arduino
        /// </summary>
        private const string KEY_INIT = "init";

        /// <summary>
        /// Com port rate
        /// </summary>
        private const int RATE = 9600;

        /// <summary>
        /// Current COM port name
        /// </summary>
        private string _comPortName;

        /// <summary>
        /// Current COM port
        /// </summary>
        private SerialPort _comPort;

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
        /// Constructor
        /// </summary>
        /// <param name="comPort">COM port to open as string</param>
        public ArduinoCom(string comPort)
        {
            _comPortName = comPort;
            _isInitialized = false;
        }

        /// <summary>
        /// Arduino Initialization Method
        /// </summary>
        public void InitializeArduino()
        {
            if (!_comPort.IsOpen)
                throw new NotSupportedException(String.Format(
                    "[{0}] Unable to initialize arduino because the port {1} is closed!",
                        MethodBase.GetCurrentMethod().Name, _comPortName));

            Log.Common.Info(String.Format("[{0}] Initializing Arduino with key {1} on port {2} ...",
                MethodBase.GetCurrentMethod().Name, KEY_INIT, _comPortName));
            
            _comPort.WriteLine(KEY_INIT);

            _isInitialized = true;
        }

        public void OpenArduinoComPort()
        {
            _comPort = new SerialPort();
            _comPort.PortName = _comPortName;
            _comPort.BaudRate = RATE;
            try
            {
                Log.Common.Info(String.Format("[{0}] Opening Arduino port {1}...",
                        MethodBase.GetCurrentMethod().Name, _comPortName));
                _comPort.Open();
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(String.Format("[{0}] Error opening Arduino port {1}. Reason: {2}",
                        MethodBase.GetCurrentMethod().Name, _comPortName, ex.Message));
            }
            Log.Common.Info(String.Format("[{0}] Arduino port {1} is open!",
                        MethodBase.GetCurrentMethod().Name, _comPortName));
        }


        public void Move(int dc = -1, eRotationalMove servo = eRotationalMove.NA)
        {
            if (!_isInitialized)
                throw new NotSupportedException(String.Format(
                   "[{0}] Unable to move arduino because arduino is not initialized.", MethodBase.GetCurrentMethod().Name));

            Log.Common.Info(String.Format("[{0}] Moving rod on {1} DC: {2} SERVO: {3}",
                        MethodBase.GetCurrentMethod().Name, _comPortName, dc, servo.ToString()));
            if (_lastServo != servo || _lastDc != dc)
            {
                String command = String.Format("{0}&{1}", dc, (int)servo);
                _comPort.Write(command);
                _lastServo = servo;
                _lastDc = dc;
            }
        }
    }

}