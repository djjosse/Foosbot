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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Data Used by drawing on UI classes
    /// </summary>
    internal static class Data
    {
        #region private members

        /// <summary>
        /// Data per each rod
        /// </summary>
        private static List<RodData> _rods = new List<RodData>();

        /// <summary>
        /// Device Max y coordinate (Foosbot World)
        /// </summary>
        private static double _deviceMaxYCoordinate =  Double.PositiveInfinity;

        /// <summary>
        /// Device Max x coordinate (Foosbot World)
        /// </summary>
        private static double _deviceMaxXCoorindate =  Double.PositiveInfinity;

        /// <summary>
        /// Table Max x coordinate (Real World)
        /// </summary>
        private static double _tableMaxXCoorindate =  Double.PositiveInfinity;

        /// <summary>
        /// Table Max y coordinate (Real World)
        /// </summary>
        private static double _tableMaxYCoorindate =  Double.PositiveInfinity;

        #endregion private members

        /// <summary>
        /// Holds the maximum of the y coordinate in the device world
        /// </summary>
        public static double DeviceMaxY
        {
            get
            {
                if (Double.IsPositiveInfinity(_deviceMaxYCoordinate))
                {
                    _deviceMaxYCoordinate = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
                }
                return _deviceMaxYCoordinate;
            }
        }

        /// <summary>
        /// Holds the maximum of the x coordinate in the device world
        /// </summary>
        public static double DeviceMaxX
        {
            get
            {
                if (Double.IsPositiveInfinity(_deviceMaxXCoorindate))
                {
                    _deviceMaxXCoorindate = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
                }
                return _deviceMaxXCoorindate;
            }
        }

        /// <summary>
        /// Holds the real life table max y in MM
        /// </summary>
        public static double TableMaxX
        {
            get
            {
                if (Double.IsPositiveInfinity(_tableMaxXCoorindate))
                {
                    _tableMaxXCoorindate = Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_WIDTH);
                }
                return _tableMaxXCoorindate;
            }
        }

        /// <summary>
        /// Holds the real life table max x in MM
        /// </summary>
        public static double TableMaxY
        {
            get
            {
                if (Double.IsPositiveInfinity(_tableMaxYCoorindate))
                {
                    _tableMaxYCoorindate = Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_HEIGHT);
                }
                return _tableMaxYCoorindate;
            }
        }

        /// <summary>
        /// Returns Rod Data per specified rod type
        /// </summary>
        /// <param name="type">Rod Type</param>
        /// <returns>Rod Data</returns>
        public static RodData GetRod(eRod type)
        {
            RodData rod = _rods.FirstOrDefault(x => x.RodType.Equals(type));
            if (rod == default(RodData))
            {
                rod = new RodData(type);
                _rods.Add(rod);
            }
            return rod;
        }

        /// <summary>
        /// Convert Table (Real World) to Device Coordinates (Foosbot Coordinates)
        /// </summary>
        /// <param name="tableCoordinates">Point in real world</param>
        /// <returns>Point in Foosbot World</returns>
        public static Point TableToDeviceCoordinates(Point tableCoordinates)
        {
            Point deviceCoordinates = new Point(
                Convert.ToInt32(tableCoordinates.X * DeviceMaxX / TableMaxX),
                Convert.ToInt32(tableCoordinates.Y * DeviceMaxY / TableMaxY));
            return deviceCoordinates;
        }
    }
}
