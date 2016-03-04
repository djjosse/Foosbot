using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Vector 2D in Cartesian Coordinates
    /// Result of vector callculation unit
    /// </summary>
    public class Vector2D : DefinableCartesianCoordinate<double>
    {
        /// <summary>
        /// Constructor for defined vector
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Vector2D(double x, double y) : base(x, y) { }

        /// <summary>
        /// Constructor for undefined vector
        /// </summary>
        public Vector2D() : base() { }

        /// <summary>
        /// Vector radius (speed) calculation method
        /// </summary>
        /// <returns>Vector speed</returns>
        public double Velocity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Vector angle calculation method.
        /// Starting from X axe counter clockwise
        /// </summary>
        /// <returns>Vector angle</returns>
        public double Angle()
        {
            throw new NotImplementedException();
        }
    }
}
