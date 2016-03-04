using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foosbot.Common
{
    public class Utils
    {
        public static double Distance(PointF p1, PointF p2)
        {
            double dX = Math.Pow(p1.X - p2.X, 2);
            double dY = Math.Pow(p1.Y - p2.Y, 2);
            return Math.Sqrt(dX + dY);
        }
    }
}
