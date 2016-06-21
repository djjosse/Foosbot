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
using Foosbot.Common.Logs;
using Foosbot.CommunicationLayer.Contracts;
using System;
using System.IO.Ports;

namespace Foosbot.CommunicationLayer.Core
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
            _port.DiscardOutBuffer();
            byte [] buffer = new byte[1];
            buffer[0] = command;
            _port.Write(buffer, 0, 1);
        }

        /// <summary>
        /// Serial port read operation
        /// </summary>
        /// <returns>One byte red</returns>
        public byte ReadByte()
        {
            try
            {
                byte received = (byte)_port.ReadByte();
                return received;
            }
            catch (Exception ex)
            {
                Log.Print(ex + " " + ex.Message, eCategory.Error, LogTag.COMMUNICATION);
                return 0;
            }
        }
    }
}
