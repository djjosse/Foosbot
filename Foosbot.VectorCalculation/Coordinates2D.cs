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
    public class Coordinates2D : DefinableCartesianCoordinate<double>
    {
        public Coordinates2D() : base() { }

        public Coordinates2D(double x, double y) : base(x, y) { }

        public double ScalarProduct(DefinableCartesianCoordinate<double> coord)
        {
            return X * coord.X + Y * coord.Y;
        }

        public static double ScalarProduct(Coordinates2D coordA, Coordinates2D coordB)
        {
            return coordA.X * coordB.X + coordA.Y * coordB.Y;
        }

        public double Velocity()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public double Distance(DefinableCartesianCoordinate<double> coordinates)
        {
            return Math.Sqrt(Math.Pow((coordinates.X - X), 2) + Math.Pow((coordinates.Y - Y), 2));
        }

        public double Distance(DefinableCartesianCoordinate<int> coordinates)
        {
            return Math.Sqrt(Math.Pow((coordinates.X - X), 2) + Math.Pow((coordinates.Y - Y), 2));
        }
    }
}
