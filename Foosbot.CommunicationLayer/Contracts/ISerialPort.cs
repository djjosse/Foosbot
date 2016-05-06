// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.CommunicationLayer.Contracts
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
        void Write(byte command);
    }
}
