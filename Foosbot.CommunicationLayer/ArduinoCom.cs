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

        private const string KEY_INIT = "init";
        private const int RATE = 9600;

        private string _comPortName;
        private SerialPort _comPort;

        public ArduinoCom(string comPort)
        {
            _comPortName = comPort;
        }

        public void InitializeArduino()
        {
            Log.Common.Info(String.Format("[{0}] Initializing Arduino with key {1} on port {2} ...",
                MethodBase.GetCurrentMethod().Name, KEY_INIT, _comPortName));
            _comPort.WriteLine(KEY_INIT);
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
                Log.Common.Error(String.Format("[{0}] Error opening Arduino port {1}. Reason: {2}",
                        MethodBase.GetCurrentMethod().Name, _comPortName, ex.Data));
            }
            Log.Common.Info(String.Format("[{0}] Arduino port {1} is open!",
                        MethodBase.GetCurrentMethod().Name, _comPortName));
        }


        private int _lastServo = 0;
        private int _lastDc = -1;

        public void Move(int dc = -1, eRotationalMove servo = eRotationalMove.NA)
        {
            Log.Common.Info(String.Format("[{0}] Moving rod on {1} DC: {2} SERVO: {3}",
                        MethodBase.GetCurrentMethod().Name, _comPortName, dc, servo.ToString()));
            if (_lastServo != (int)servo || _lastDc != dc)
            {
                String action = String.Format("{0}&{1}", dc, (int)servo);
                _comPort.Write(action);
                _lastServo = (int)servo;
                _lastDc = dc;
            }
        }
    }

}