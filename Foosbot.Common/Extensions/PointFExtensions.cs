// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using System;
using System.Drawing;

namespace Foosbot.Common.Extensions
{
    /// <summary>
    /// PointF Class Extension Methods
    /// </summary>
    public static class PointFExtensions
    {
        /// <summary>
        /// Distance beetween two points
        /// </summary>
        /// <param name="p1">Point A</param>
        /// <param name="p2">Point B</param>
        /// <returns>Distantce between A and B</returns>
        public static double Distance(this PointF p1, PointF p2)
        {
            double dX = Math.Pow(p1.X - p2.X, 2);
            double dY = Math.Pow(p1.Y - p2.Y, 2);
            return Math.Sqrt(dX + dY);
        }
    }
}
