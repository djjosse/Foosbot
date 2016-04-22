using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer
{
    /// <summary>
    /// Serial Port wrapper class
    /// </summary>
    public class SerialPortWrapper : ISerialPort
    {

        /// <summary>
        /// Actual serial port
        /// </summary>
        private SerialPort _port;

        /// <summary>
        /// Serial port constructor
        /// </summary>
        /// <param name="portName">Port name for ex. 'com1'</param>
        /// <param name="baudRate">Port baud rate for ex. 9600</param>
        public SerialPortWrapper(string portName, int baudRate)
        {
            _port = new SerialPort(portName, baudRate);
        }

        /// <summary>
        /// [True] if port is open, [False] otherwise
        /// </summary>
        public bool IsOpen
        {   
            get
            {
                return _port.IsOpen;
            }
        }

        /// <summary>
        /// Open Port method
        /// </summary>
        public void Open()
        {
            _port.Open();
        }

        /// <summary>
        /// Write port method
        /// </summary>
        /// <param name="command">Command to be sent to port</param>
        public void Write(byte command)
        {
            byte [] buffer = new byte[1];
            buffer[0] = command;
            _port.Write(buffer, 0, 1);
        }
    }
}
