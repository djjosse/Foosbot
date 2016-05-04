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
using Foosbot.ImageProcessingUnit.Process.Contracts;
using System;

namespace Foosbot.ImageProcessingUnit.Process.Core
{
    /// <summary>
    /// Ball Location in real world before transformation performed
    /// </summary>
    public class BallLocation : DefinableCartesianCoordinate<int>, ITimedLocation
    {
        /// <summary>
        /// Location constructor for undefined location
        /// </summary>
        /// <param name="timestamp">Frame Time Stamp</param>
        public BallLocation(DateTime timestamp)
            : base()
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Location constructor for defined location
        /// </summary>
        /// <param name="x">Ball X location in frame</param>
        /// <param name="y">Ball Y location in frame</param>
        /// <param name="timestamp">Frame Time Stamp</param>
        public BallLocation(int x, int y, DateTime timestamp)
            :base(x, y)
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Location constructor for defined location
        /// </summary>
        /// <param name="x">Ball X location in frame (will be cut to integer)</param>
        /// <param name="y">Ball Y location in frame (will be cut to integer)</param>
        /// <param name="timestamp"></param>
        public BallLocation(double x, double y, DateTime timestamp)
            : base(Convert.ToInt32(x), Convert.ToInt32(y))
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// Ball Location in Frame Time Stamp Get Property
        /// </summary>
        public DateTime Timestamp { get; private set; }
    }
}
