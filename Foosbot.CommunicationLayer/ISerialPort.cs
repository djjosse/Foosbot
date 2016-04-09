using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer
{
    /// <summary>
    /// Implementing class must override all provided
    /// here functions to be a serial port wrapper class
    /// </summary>
    public interface ISerialPort
    {
        /// <summary>
        /// [True] if serial port is open, [False] otherwise
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Open port method
        /// </summary>
        void Open();

        /// <summary>
        /// Write port method
        /// </summary>
        /// <param name="command">Command to be sent to port</param>
        void Write(string command);

        /// <summary>
        /// Write port method
        /// </summary>
        /// <param name="command">Command to be sent to port</param>
        void WriteLine(string command);
    }
}
