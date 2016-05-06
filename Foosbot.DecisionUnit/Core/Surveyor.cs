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
using Foosbot.DecisionUnit.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    public class Surveyor : ISurveyor
    {
        /// <summary>
        /// Surveyor Constructor.
        /// Initializes properies from Configuration File
        /// </summary>
        public Surveyor(int xMaxPts = -1, int yMaxPts = -1, int xMaxMm = -1, int yMaxMm = -1)
        {
            XMaxPts = (xMaxPts != -1) ? xMaxPts
                : Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            YMaxPts = (yMaxPts != -1) ? yMaxPts
                : Configuration.Attributes.GetValue<int>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            XMaxMm = (xMaxMm != -1) ? xMaxMm
                : Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_WIDTH);
            YMaxMm = (yMaxMm != -1) ? yMaxMm
                : Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);
        }

        /// <summary>
        /// Table maximal X (width) in Foosbot world (POINTS)
        /// </summary>
        public int XMaxPts { get; private set; }

        /// <summary>
        /// Table maximal Y (height) in Foosbot world (POINTS)
        /// </summary>
        public int YMaxPts { get; private set; }

        /// <summary>
        /// Table maximal X (width) in (MM)
        /// </summary>
        public int XMaxMm { get; private set; }

        /// <summary>
        /// Table maximal Y (height) in (MM)
        /// </summary>
        public int YMaxMm { get; private set; }

        /// <summary>
        /// Convert BallCoordinates in points to BallCoordinates in mm
        /// </summary>
        /// <param name="pts">BallCoordinates in Points</param>
        /// <returns>BallCoordinates in Milimeters</returns>
        public BallCoordinates PtsToMm(BallCoordinates pts)
        {
            BallCoordinates mmCoords = null;
            if (pts != null && pts.IsDefined)
            {
                int xMm = pts.X * XMaxMm / XMaxPts;
                int yMm = pts.Y * YMaxMm / YMaxPts;
                mmCoords = new BallCoordinates(xMm, yMm, pts.Timestamp);
            }
            else
            {
                return pts;
            }

            if (pts.Vector != null && pts.Vector.IsDefined)
            {
                double xMm = pts.Vector.X * (double)XMaxMm / (double)XMaxPts;
                double yMm = pts.Vector.Y * (double)YMaxMm / (double)YMaxPts;
                mmCoords.Vector = new Vector2D(xMm, yMm);
            }
            else
            {
                mmCoords.Vector = pts.Vector;
            }
            return mmCoords;
        }

        /// <summary>
        /// Convert BallCoordinates in mm to BallCoordinates in points
        /// </summary>
        /// <param name="mm">BallCoordinates in Milimeters</param>
        /// <returns>BallCoordinates in Points</returns>
        public BallCoordinates MmToPts(BallCoordinates mm)
        {
            BallCoordinates pointsCoords = null;
            if (mm != null && mm.IsDefined)
            {
                int xMm = mm.X * XMaxPts / XMaxMm;
                int yMm = mm.Y * YMaxPts / YMaxMm;
                pointsCoords = new BallCoordinates(xMm, yMm, mm.Timestamp);
            }
            else
            {
                return mm;
            }

            if (mm.Vector != null && mm.Vector.IsDefined)
            {
                double xMm = mm.Vector.X * (double)XMaxPts / (double)XMaxMm;
                double yMm = mm.Vector.Y * (double)YMaxPts / (double)YMaxMm;
                pointsCoords.Vector = new Vector2D(xMm, yMm);
            }
            else
            {
                pointsCoords.Vector = mm.Vector;
            }
            return pointsCoords;
        }

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="xCoordinate">X cooridanate to compare</param>
        /// <param name="yCoordinate">Y cooridanate to compare</param>
        /// <param name="units">Units to work in (mm/pts only) [default is mm]</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        public bool IsCoordinatesInRange(int xCoordinate, int yCoordinate, eUnits units = eUnits.Mm)
        {
            return (IsCoordinatesXInRange(xCoordinate, units) && IsCoordinatesYInRange(yCoordinate, units));
        }

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="xCoordinate">X cooridanate to compare</param>
        /// <param name="units">Units to work in (mm/pts only) [default is mm]</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        public bool IsCoordinatesXInRange(int xCoordinate, eUnits units = eUnits.Mm)
        {
            VerifyUnitsSupported(units);
            int max = (units == eUnits.Mm) ? XMaxMm : XMaxPts;
            return (xCoordinate >= 0 && xCoordinate <= max);
        }

        /// <summary>
        /// Defines if given coordinates in table range.
        /// </summary>
        /// <param name="yCoordinate">Y cooridanate to compare</param>
        /// <param name="units">Units to work in (mm/pts only) [default is mm]</param>
        /// <returns>[True] if in range, [False] otherwise</returns>
        public bool IsCoordinatesYInRange(int yCoordinate, eUnits units = eUnits.Mm)
        {
            VerifyUnitsSupported(units);
            int max = (units == eUnits.Mm) ? YMaxMm : YMaxPts;
            return (yCoordinate >= 0 && yCoordinate <= max);
        }

        /// <summary>
        /// Verify if units are supported by surveyor
        /// </summary>
        /// <param name="units">Units type</param>
        /// <exception cref="NotSupportedException">Thrown in case units not supported</exception>
        private void VerifyUnitsSupported(eUnits units)
        {
            switch(units)
            {
                case eUnits.Mm:
                case eUnits.Pts:
                    return;
                default:
                    throw new NotSupportedException(String.Format(
                        "[{0}] Units of type {1} are not supported by {2} class",
                            MethodBase.GetCurrentMethod().Name, units.ToString(), GetType().Name));
            }
        }
    }
}
