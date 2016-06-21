// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.CommunicationLayer.Enums
{
    /// <summary>
    /// Arduino Response Codes
    /// </summary>
    public enum eResponseCode
    {
        /// <summary>
        /// No new data received
        /// </summary>
        NO_DATA = 0,

        /// <summary>
        /// Initialization required
        /// </summary>
        INIT_REQUERED = 100,

        /// <summary>
        /// Initialization requested
        /// </summary>
        INIT_REQUESTED = 101,

        /// <summary>
        /// Initialization started
        /// </summary>
        INIT_STARTED = 102,

        /// <summary>
        /// Initialization operations finished
        /// </summary>
        INIT_FINISHED = 103,

        /// <summary>
        /// Servo State is KICK
        /// </summary>
        SERVO_STATE_KICK = 110,

        /// <summary>
        /// Servo State is DEFENSE
        /// </summary>
        SERVO_STATE_DEFENCE = 111,

        /// <summary>
        /// Servo State is RISE
        /// </summary>
        SERVO_STATE_RISE = 112,

        /// <summary>
        /// Invalid Range for DC was requested
        /// </summary>
        DC_RANGE_INVALID = 120,

        /// <summary>
        /// DC was set as requested
        /// </summary>
        DC_RECEIVED_OK = 121,

        /// <summary>
        /// DC was calibrated
        /// </summary>
        DC_CALIBRATED = 122
    }
}
