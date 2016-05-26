// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Protocols;
using System;

namespace Foosbot.VectorCalculation
{
    /// <summary>
    /// 2D Definable Coordinates 
    /// </summary>
    public class Coordinates2D : DefinableCartesianCoordinate<double>
    {
        /// <summary>
        /// Constructor for Undefined coordinates
        /// </summary>
        public Coordinates2D() : base() { }

        /// <summary>
        /// Constructor for defined coordinates
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        public Coordinates2D(double x, double y) : base(x, y) { }

        /// <summary>
        /// Coordinates scalar product
        /// </summary>
        /// <param name="coord">Coordinate to calculate Scalar Product With</param>
        /// <returns>Scalar Product</returns>
        public double ScalarProduct(DefinableCartesianCoordinate<double> coord)
        {
            return X * coord.X + Y * coord.Y;
        }

        /// <summary>
        /// Coordinates scalar product
        /// </summary>
        /// <param name="coordA">Coordinate to calculate Scalar Product</param>
        /// <param name="coordB">Coordinate to calculate Scalar Product With</param>
        /// <returns>Scalar Product</returns>
        public static double ScalarProduct(Coordinates2D coordA, Coordinates2D coordB)
        {
            return coordA.X * coordB.X + coordA.Y * coordB.Y;
        }

        /// <summary>
        /// Distance from 0 to current coordinate
        /// </summary>
        /// <returns>Distance from 0 to current coordinate as double</returns>
        public double Velocity()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        /// <summary>
        /// Distance between current and provided coordinates
        /// </summary>
        /// <param name="coordinates">Additional Coordinates as DefinableCartesianCoordinate (int)</param>
        /// <returns>Distance as double</returns>
        public double Distance(DefinableCartesianCoordinate<double> coordinates)
        {
            return Math.Sqrt(Math.Pow((coordinates.X - X), 2) + Math.Pow((coordinates.Y - Y), 2));
        }

        /// <summary>
        /// Distance between current and provided coordinates
        /// </summary>
        /// <param name="coordinates">Additional Coordinates as DefinableCartesianCoordinate (int)</param>
        /// <returns>Distance as double</returns>
        public double Distance(DefinableCartesianCoordinate<int> coordinates)
        {
            return Math.Sqrt(Math.Pow((coordinates.X - X), 2) + Math.Pow((coordinates.Y - Y), 2));
        }
    }
}
