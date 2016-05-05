using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer
{
    /// <summary>
    /// Communication Layer Constants Static Class
    /// </summary>
    public static class Communication
    {
        /// <summary>
        /// Sleep in millisecond - minimum time between two commands sent to arduino
        /// </summary>
        public const int SLEEP = 200;

        /// <summary>
        /// Com port rate
        /// </summary>
        public const int BAUDRATE = 115200;

        /// <summary>
        /// Initialization byte to sent to arduino (0x11111111)
        /// </summary>
        public const byte INIT_BYTE = 255;

        /// <summary>
        /// Safety Buffer for rod maximal move in Ticks
        /// </summary>
        public const int SAFETY_TICKS_BUFFER = 100;

        /// <summary>
        /// Minimal value to be encoded for DC coordinate (0x00000001)
        /// </summary>
        public const int MIN_DC_VALUE_TO_ENCODE = 1;

        /// <summary>
        /// Maximal value to be encoded for DC coordinate (0x00111110)
        /// </summary>
        public const int MAX_DC_VALUE_TO_ENCODE = 62;
    }
}
