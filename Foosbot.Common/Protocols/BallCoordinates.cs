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
    /// Image Processing and Vector Calculation Units common output class
    /// </summary>
    public class BallCoordinates : DefinableCartesianCoordinate<int>
    {
        /// <summary>
        /// Constructor for defined coordinates
        /// </summary>
        /// <param name="x">X ball coordinate</param>
        /// <param name="y">Y ball coordinate</param>
        /// <param name="timestamp"></param>
        public BallCoordinates(int x, int y, DateTime timestamp) : base(x, y)
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Constructor for undefined coordinates
        /// </summary>
        /// <param name="timestamp">Timestamp of current coordinates</param>
        public BallCoordinates(DateTime timestamp):base()
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Timestamp of current coordinates
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Vector of current ball location - TBD in VectorCalculationUnit
        /// </summary>
        public Vector2D Vector { get; set; }

        /// <summary>
        /// Distance between current coordinates and given coordinates
        /// </summary>
        /// <param name="coordinates">Coordinates</param>
        /// <returns>Distance as double</returns>
        public double Distance<T>(DefinableCartesianCoordinate<T> coordinates)
            where T : struct, IComparable
        {
            dynamic x = (dynamic)coordinates.X;
            dynamic y = (dynamic)coordinates.Y;
            return Math.Sqrt(Math.Pow((x - X), 2) + Math.Pow((y - Y), 2));
        }

        public override string ToString()
        {
            return String.Format("Coordinates: {0}x{1} Time: {2} Vector: {3}", X, Y, Timestamp.ToString("HH:mm:ss.ff"), Vector.ToString());
        }
    }
}
