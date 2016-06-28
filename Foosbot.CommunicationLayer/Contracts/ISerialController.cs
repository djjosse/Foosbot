// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using Foosbot.Common.Enums;
using Foosbot.CommunicationLayer.Enums;
using System;

namespace Foosbot.CommunicationLayer.Contracts
{
    /// <summary>
    /// Interface for micro-controller on with serial port
    /// </summary>
    public interface ISerialController : IInitializable
    {
        /// <summary>
        /// Delegate for a Method to be called on arduino servo state changed
        /// </summary>
        Action<eRod, eRotationalMove> OnServoChangeState { get; set; }

        /// <summary>
        /// Rod Type of current controller
        /// </summary>
        eRod RodType { get; set; }

        /// <summary>
        /// Current COM port name
        /// </summary>
        string ComPortName { get; }

        /// <summary>
        /// Maximum ticks per current rod
        /// </summary>
        int MaxTicks { get; set; }

        /// <summary>
        /// Calibrate controller method
        /// </summary>
        void RequestCalibration();

        /// <summary>
        /// Open Serial Communication Port
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown in case of error while opening port</exception>
        void OpenSerialPort();

        /// <summary>
        /// Move arduino DC and Servo
        /// </summary>
        /// <param name="dc">DC movement (-1) for no movement</param>
        /// <param name="servo">Servo Position</param>
        /// <exception cref="InitializationException">Thrown in case arduino was not initialized</exception>
        /// <exception cref="InvalidOperationException">Thrown in of wrong DC parameter</exception>
        void Move(int dc = -1, eRotationalMove servo = eRotationalMove.NA);

        /// <summary>
        /// Update last arduino servo state
        /// </summary>
        /// <param name="newState">Servo state as arduino response code</param>
        void SetLastServoState(eResponseCode newState);

        /// <summary>
        /// Read received data from Serial port
        /// </summary>
        void Read();
    }
}
