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
using Foosbot.CommunicationLayer.Contracts;
using System;

namespace Foosbot.CommunicationLayer.Core
{
    public class ActionEncoder : IEncoder
    {
        /// <summary>
        /// Converter bits to ticks and back
        /// </summary>
        public IRodConverter Converter { get; private set; }

        /// <summary>
        /// Action encoder constructor
        /// </summary>
        /// <param name="converter">Converter to get ticks per rod</param>
        public ActionEncoder(IRodConverter converter)
        {
            Converter = converter;
        }

        /// <summary>
        /// Get encoded initialization byte to sent to Arduino
        /// </summary>
        /// <returns>Encoded initialization byte</returns>
        public byte EncodeInitialization()
        {
            return Communication.INIT_BYTE;
        }

        /// <summary>
        /// Get actions coded in one byte to sent to Arduino
        /// </summary>
        /// <param name="dcInTicks">DC coordinate in ticks</param>
        /// <param name="servo">Servo position</param>
        /// <returns>Action as command in one byte</returns>
        public byte Encode(int dcInTicks, eRotationalMove servo)
        {
            //Convert ticks to bits (0x00000000 (0) to 0x00111110 (62))
            int dcBites = Converter.TicksToBits(dcInTicks);

            //create empty command
            byte command = 0x00000000;

            /*
             * Add Servo action as binary (YY) to empty command
             * Possible Servo Actions:
             * - 0x00 - NA
             * - 0x01 - KICK
             * - 0x10 - DEFENCE
             * - 0x11 - RISE
             * Command will look like: 0x000000YY             
             */
            command = (byte)(command | Convert.ToByte((int)servo));

            /*
             * Shift dc action in bites 2 bites left.
             * For DC action 0xZZZZZZ shifted will be 0xZZZZZZ00
             * Possible DC Actions:
             * - 0x000000 - NA
             * - 0x000001 - (1) minimal coordinate
             * - 0x111110 - (62) maximal coordinate
             */
            byte dcShifted = (byte)(Convert.ToByte(dcBites) << 2);

            /*
             * Combine Servo 0x000000YY and DC 0xZZZZZZ00
             * and get command as 0xZZZZZZYY
             */
            command = (byte)(command | dcShifted);

            return command;
        }
    }
}
