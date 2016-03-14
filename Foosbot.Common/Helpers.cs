using Foosbot.Common.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foosbot
{
    public static class Helpers
    {
        public delegate void UpdateStatisticsDelegate(Helpers.eStatisticsKey key, string info);

        /// <summary>
        /// Markup Update delegate
        /// </summary>
        /// <param name="key">Markup Key</param>
        /// <param name="center">Point to draw center of circle</param>
        /// <param name="radius">Radius of circle to draw</param>
        public delegate void UpdateMarkupCircleDelegate(eMarkupKey key, Point center, int radius);

        public delegate void UpdateLog(eLogType type, eLogCategory cat, DateTime timestamp, string message);

        public delegate void UpdateMarkupLineDelegate(eMarkupKey key, Point startP, Point endP);

        /// <summary>
        /// Represents Markup Keys
        /// </summary>
        public enum eMarkupKey
        {
            TOP_LEFT_CALLIBRATION_MARK = 1,
            TOP_RIGHT_CALLIBRATION_MARK = 2,
            BUTTOM_LEFT_CALLIBRATION_MARK = 3,
            BUTTOM_RIGHT_CALLIBRATION_MARK = 4,
            BALL_CIRCLE_MARK = 5,

            TOP_LEFT_CALLIBRATION_TEXT = 6,
            TOP_RIGHT_CALLIBRATION_TEXT = 7,
            BUTTOM_LEFT_CALLIBRATION_TEXT = 8,
            BUTTOM_RIGHT_CALLIBRATION_TEXT = 9,

            BALL_VECTOR = 10
        }

        /// <summary>
        /// Represents Statistics Keys
        /// </summary>
        public enum eStatisticsKey
        {
            /// <summary>
            /// Current FPS, Width and Heigth
            /// </summary>
            FrameInfo = 1,

            /// <summary>
            /// Foosbot Memory and CPU Info
            /// </summary>
            ProccessInfo = 2,

            /// <summary>
            /// Percentage of successful ball detection and average detection time
            /// </summary>
            BasicImageProcessingInfo = 3,

            /// <summary>
            /// Ball Coordinates
            /// </summary>
            BallCoordinates = 4
        }
    }
}
