// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using System;
namespace Foosbot.DecisionUnit.Contracts
{
    public interface ISurveyor
    {
        /// <summary>
        /// Table maximal X (width) in Foosbot world (POINTS)
        /// </summary>
        int XMaxPts { get; }

        /// <summary>
        /// Table maximal Y (height) in Foosbot world (POINTS)
        /// </summary>
        int YMaxPts { get; }

        /// <summary>
        /// Table maximal X (width) in (MM)
        /// </summary>
        int XMaxMm { get; }

        /// <summary>
        /// Table maximal Y (height) in (MM)
        /// </summary>
        int YMaxMm { get; }

        /// <summary>
        /// Convert BallCoordinates in points to BallCoordinates in mm
        /// </summary>
        /// <param name="pts">BallCoordinates in Points</param>
        /// <returns>BallCoordinates in Milimeters</returns>
        BallCoordinates PtsToMm(BallCoordinates pts);

        /// <summary>
        /// Convert BallCoordinates in mm to BallCoordinates in points
        /// </summary>
        /// <param name="mm">BallCoordinates in Milimeters</param>
        /// <returns>BallCoordinates in Points</returns>
        BallCoordinates MmToPts(BallCoordinates mm);

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="xCoordinate">X cooridanate to compare</param>
        /// <param name="yCoordinate">Y cooridanate to compare</param>
        /// <param name="units">Units to work in (mm/pts only) [default is mm]</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        bool IsCoordinatesInRange(int xCoordinate, int yCoordinate, eUnits units = eUnits.Mm);

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="xCoordinate">X cooridanate to compare</param>
        /// <param name="units">Units to work in (mm/pts only) [default is mm]</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        bool IsCoordinatesXInRange(int xCoordinate, eUnits units = eUnits.Mm);

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="yCoordinate">Y cooridanate to compare</param>
        /// <param name="units">Units to work in (mm/pts only) [default is mm]</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        bool IsCoordinatesYInRange(int yCoordinate, eUnits units = eUnits.Mm);
    }
}
