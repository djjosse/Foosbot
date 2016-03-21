using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
