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
        public delegate void UpdateMarkupDelegate(eMarkupKey key, Point center, int radius);

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
            BUTTOM_RIGHT_CALLIBRATION_TEXT = 9
        }

        /// <summary>
        /// Represents Statistics Keys
        /// </summary>
        public enum eStatisticsKey
        {
            IMAGE_WIDTH = 1,
            IMAGE_HEIGHT = 2,
            FPS = 3
        }
    }
}
