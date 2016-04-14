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

namespace Foosbot.VectorCalculation.Contracts
{
    /// <summary>
    /// Interface for ricochet calculation class
    /// </summary>
    public interface IRicochet
    {
        /// <summary>
        /// Minimum border in x
        /// </summary>
        double MinBorderX{ get; }

        /// <summary>
        /// Minimum border in y
        /// </summary>
        double MinBorderY { get; }

        /// <summary>
        /// Maxmimum border in x
        /// </summary>
        double MaxBorderX { get; }

        /// <summary>
        /// Maximum border in x
        /// </summary>
        double MaxBorderY { get; }

        /// <summary>
        /// Ricochet factor
        /// </summary>
        double RicochetFactor { get; }

        /// <summary>
        /// Ricochet coordinate calculation
        /// </summary>
        /// <param name="ballCoordinates">Ball Coordinates</param>
        /// <returns>Ricochet Ball Coordinates</returns>
        BallCoordinates Ricochet(BallCoordinates ballCoordinates);

        /// <summary>
        /// Find nearest intersection point with table borders based on
        /// - given ball coordinates and vector
        /// </summary>
        /// <param name="ballCoordinates">Defined coordinates with defined vector</param>
        /// <returns>Coordinates of intersection with border</returns>
        Coordinates2D FindNearestIntersectionPoint(BallCoordinates ballCoordinates);

        /// <summary>
        /// Find intersection time based on ball coordinates, timestamp, vector and intersection point.
        /// </summary>
        /// <param name="ballCoordinates">Ball coordinates before intersection with border</param>
        /// <param name="intersection">Intersection with border point</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown in case calculated intersection time is too big</exception>
        /// <exception cref="NotSupportedException">Thrown in case intersection coordinates undefined.</exception>
        /// <returns>Intersection timestamp</returns>
        DateTime FindRicochetTime(BallCoordinates ballCoordinates, Coordinates2D intersection);

        /// <summary>
        /// Calculate intersection vector from intersection point
        /// </summary>
        /// <param name="vector">Last known vector before intersection</param>
        /// <param name="intersection">Intersection point with border</param>
        /// <exception cref="NotSupportedException">Thrown in case passed intersection point is not on the border</exception>
        /// <returns>Ball Vector after intersection</returns>
        Vector2D FindIntersectionVector(Vector2D vector, Coordinates2D intersection);
    }
}
