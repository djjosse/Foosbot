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
    /// Abstract cartesian coordinates class for two non-nullable numbers
    /// </summary>
    /// <typeparam name="T">Non-nullable comparable type to store</typeparam>
    public abstract class DefinableCartesianCoordinate<T> where T : struct, IComparable
    {
        protected T _x;
        protected T _y;

        /// <summary>
        /// Constructor to be called if cartesian coordinates are defined
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public DefinableCartesianCoordinate(T x, T y)
        {
            _x = x;
            _y = y;
            IsDefined = true;
        }

        /// <summary>
        /// Constructor to be called in case cartesian coordinates are not defined
        /// </summary>
        public DefinableCartesianCoordinate()
        {
            IsDefined = false;
        }

        /// <summary>
        /// Definition property [true] if coordinates defined, [false] otherwise
        /// </summary>
        public bool IsDefined { get; protected set; }

        
        /// <summary>
        /// X coordinate property
        /// </summary>
        public T X
        {
            get
            {
                if (IsDefined)
                    return _x;
                else
                    throw new Exception("Cartesian coordinate is undefined, no value stored in X");
            }
        }

        /// <summary>
        /// Y coordinate property
        /// </summary>
        public T Y 
        {
            get
            {
                if (IsDefined)
                    return _y;
                else
                    throw new Exception("Cartesian coordinate is undefined, no value stored in Y");
            }
        }
    }
}
