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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    /// <summary>
    /// Represents a class for point relevant for some timestamp
    /// </summary>
    public class TimedPoint : DefinableCartesianCoordinate<int>
    {
        /// <summary>
        /// Constructor to be called in case TimedPoint is defined
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="timestamp">Timestamp for point</param>
        public TimedPoint(int x, int y, DateTime timestamp)
        {
            _x = x;
            _y = y;
            _timestamp = timestamp;
            IsDefined = true;
        }

        /// <summary>
        /// Constructor to be called in case TimedPoint is not defined
        /// </summary>
        public TimedPoint()
        {
            IsDefined = false;
        }

        /// <summary>
        /// Timestamp for point
        /// </summary>
        private DateTime _timestamp;

        /// <summary>
        /// Timestamp for point
        /// </summary>
        public DateTime Timestamp 
        {
            get
            {
                if (IsDefined)
                    return _timestamp;
                else
                    throw new Exception("TimedPoint coordinate is undefined, no value stored in timestamp");
            }
        }

    }
}
