// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;

namespace Foosbot.CommunicationLayer.Contracts
{
    /// <summary>
    /// Encoder is responsible for encoding data before sending to arduino
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Converter bits to ticks and back
        /// </summary>
        IRodConverter Converter { get; }

        /// <summary>
        /// Get encoded initialization byte to sent to Arduino
        /// </summary>
        /// <returns>Encoded initialization byte</returns>
        byte EncodeInitialization();

        /// <summary>
        /// Get actions coded in one byte to sent to Arduino
        /// </summary>
        /// <param name="dcInTicks">DC coordinate in ticks</param>
        /// <param name="servo">Servo position</param>
        /// <returns>Action as command in one byte</returns>
        byte Encode(int dcInTicks, eRotationalMove servo);
    }
}
