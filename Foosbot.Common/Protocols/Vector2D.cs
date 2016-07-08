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

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Vector 2D in Cartesian Coordinates (points per second)
    /// Result of vector Calculation Unit
    /// </summary>
    public class Vector2D : DefinableCartesianCoordinate<double>
    {
        /// <summary>
        /// Constructor for defined vector
        /// </sumary>
        /// <param name="x">X coordinate (points per second)</param>
        /// <param name="y">Y coordinate (points per second)</param>
        public Vector2D(double x, double y) : base(x, y) { }

        /// <summary>
        /// Constructor for undefined vector
        /// </summary>
        public Vector2D() : base() { }

        /// <summary>
        /// Vector radius (speed) calculation method (points per second)
        /// </summary>
        /// <returns>Vector speed</returns>
        public double Velocity()
        {
            return (IsDefined) ? Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)) : 0;
        }

        /// <summary>
        /// Vector angle calculation method.
        /// Starting from X axe counter clockwise
        /// </summary>
        /// <returns>Vector angle</returns>
        public double Angle()
        {
            if (IsDefined)
            {
                if (X == 0 && Y > 0) return Math.PI / 2;
                else if (X == 0 && Y < 0) return 1.5* Math.PI;
                else if (X > 0 && Y == 0) return 0;
                else if (X < 0 && Y == 0) return Math.PI;
                else if (X > 0 && Y > 0) return Math.Atan(Y / X);
                else if (X > 0 && Y < 0) return 2 * Math.PI + Math.Atan(Y / X);
                else if (X < 0 && Y > 0) return Math.PI + Math.Atan(Y / X);
                else if (X < 0 && Y < 0) return Math.PI + Math.Atan(Y / X);
                else return 0; //(X == 0 && Y == 0)
            }
            throw new Exception("Vector coordinates is undefined, no value stored in X and Y to define angle");
        }

        /// <summary>
        /// vector and another 2D coordinate scalar product
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public double ScalarProduct(DefinableCartesianCoordinate<double> coord)
        {
            return X * coord.X + Y * coord.Y;
        }

        /// <summary>
        /// Vector to string method
        /// </summary>
        /// <returns>Vector as string</returns>
        public override string ToString()
        {
            return String.Format("{0}x{1}", X, Y);
        }

        /// <summary>
        /// Check if vector is not null and defined
        /// </summary>
        /// <param name="vector">Vector to check</param>
        /// <returns>[True] if vector is not null and is defined, [False] otherwise</returns>
        public static bool NotNullAndDefined(Vector2D vector)
        {
            return (vector != null) && vector.IsDefined;
        }
    }
}
