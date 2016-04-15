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
using Foosbot.DecisionUnit.Core;
using Foosbot.VectorCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Contracts
{
    public interface IRod
    {
        /// <summary>
        /// Rod type private readonly member
        /// </summary>
        eRod RodType { get; }

        /// <summary>
        /// Distance between each 2 player on current rod
        /// </summary>
        int PlayerDistance { get; }

        /// <summary>
        /// Number of players in current rod
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// Distance from table border (Y min) to head of first player
        /// </summary>
        int OffsetY { get; }

        /// <summary>
        /// Distance between stoppers of current rod
        /// </summary>
        int StopperDistance { get; }

        /// <summary>
        /// Rod X coordinate in Foosbot world private readonly member
        /// </summary>
        int RodXCoordinate { get; }

        /// <summary>
        /// Minimal Sector Width in Foosbot world private readonly member
        /// </summary>
        int MinSectorWidth { get; }

        /// <summary>
        /// Sector Factor used to calculate dynamic sector private readonly member
        /// </summary>
        double SectorFactor { get; }

        /// <summary>
        /// Best Effort First Player Y Coordinate 
        /// </summary>
        int BestEffort { get; }

        /// <summary>
        /// Intersection point and time with latest ball vector
        /// </summary>
        TimedPoint Intersection { get; }

        /// <summary>
        /// Dynamic Sector Get Property
        /// </summary>
        int DynamicSector { get; }

        /// <summary>
        /// Last Known State of Rod
        /// </summary>
        IRodState State { get; }

        /// <summary>
        /// Minimum possible start stopper Y coordinate of rod
        /// </summary>
        int MinimumPossibleStartStopperY { get; }

        /// <summary>
        /// Maximum possible start stopper Y coordinate of rod
        /// </summary>
        int MaximumPossibleStartStopperY { get; }

        /// <summary>
        /// Maximum Intersection Prediction TimeSpan in seconds
        /// (Predictions after this timespan from current time will be irrelevant in Intersection calculation)
        /// </summary>
        int PredictIntersectionMaxTimespan { get; }

        /// <summary>
        /// Dynamic sector calculation method - sets DynamicSector property value.
        /// * if coordinates and vector are defined AND vector is to the rod THEN sets dynamic sector due to ball velocity
        /// * else sets minimal sector width
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates and vector</param>
        /// <returns>Dynamic Sector Width</returns>
        int CalculateDynamicSector(BallCoordinates currentCoordinates);

        /// <summary>
        /// Calculate Rod Intersection with current rod
        /// </summary>
        /// <param name="currentCoordinates">Current ball coordinates to calculate intersection</param>
        void CalculateSectorIntersection(BallCoordinates currentCoordinates);

        /// <summary>
        /// Get nearest possible DC position in case desired position is out of range
        /// </summary>
        /// <param name="desiredPosition">Originlly desired position</param>
        /// <returns>Nearest posible position</returns>
        int NearestPossibleDcPosition(int desiredPosition);
    }
}
