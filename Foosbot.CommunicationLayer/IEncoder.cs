using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using System;
namespace Foosbot.CommunicationLayer
{
    public interface IEncoder
    {
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
