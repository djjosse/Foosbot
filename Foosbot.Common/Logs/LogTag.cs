using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Logs
{
    public static class LogTag
    {
        public static List<string> ALL_TAGS = new List<string>() 
        {
            COMMON, 
            IMAGE,
            VECTOR,
            DECISION,
            COMMUNICATION,
            ARDUINO
        };

        /// <summary>
        /// Common and Shared parts of Foosbot
        /// </summary>
        public const string COMMON = "Common";

        /// <summary>
        /// Image Processing Logs
        /// </summary>
        public const string IMAGE = "Image";

        public const string VECTOR = "Vector";

        public const string DECISION = "Decision";

        public const string COMMUNICATION = "Communication";

        public const string ARDUINO = "Arduino";
    }
}
