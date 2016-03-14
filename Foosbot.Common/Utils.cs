using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foosbot.Common
{
    /// <summary>
    /// Common Utils Class
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Distance beetween two points
        /// </summary>
        /// <param name="p1">Point A</param>
        /// <param name="p2">Point B</param>
        /// <returns>Distantce between A and B</returns>
        public static double Distance(PointF p1, PointF p2)
        {
            double dX = Math.Pow(p1.X - p2.X, 2);
            double dY = Math.Pow(p1.Y - p2.Y, 2);
            return Math.Sqrt(dX + dY);
        }
    }
}
