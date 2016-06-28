// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using System.Collections.Generic;

namespace Foosbot.Common.Logs
{
    /// <summary>
    /// Logger Tags (Modules)
    /// </summary>
    public static class LogTag
    {
        /// <summary>
        /// All tags
        /// </summary>
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

        /// <summary>
        /// Vector Calculation Unit Logs
        /// </summary>
        public const string VECTOR = "Vector";

        /// <summary>
        /// Decision Unit Logs
        /// </summary>
        public const string DECISION = "Decision";

        /// <summary>
        /// Communication Unit Local Logs
        /// </summary>
        public const string COMMUNICATION = "Communication";

        /// <summary>
        /// Arduino Controller (Communication Unit Remote) Logs
        /// </summary>
        public const string ARDUINO = "Arduino";
    }
}
