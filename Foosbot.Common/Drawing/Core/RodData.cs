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

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Specific rod static data
    /// </summary>
    internal class RodData
    {
        #region private members 

        /// <summary>
        /// Player Count on rod
        /// </summary>
        private int _playersCount = Int32.MinValue;

        /// <summary>
        /// Distance between players on rod
        /// </summary>
        private int _playersYDistance = Int32.MinValue;

        /// <summary>
        /// First Player Offset from stopper
        /// </summary>
        private int _firstPlayerOffsetY = Int32.MinValue;

        /// <summary>
        /// X Coordinate (Real World) of rod
        /// </summary>
        private int _xCoordinate = Int32.MinValue;

        #endregion private members

        /// <summary>
        /// Rod Type
        /// </summary>
        public eRod RodType { get; private set; }

        /// <summary>
        /// Player Count on rod
        /// </summary>
        public int PlayersCount 
        {
            get
            {
                if (_playersCount == Int32.MinValue)
                {
                    _playersCount = Configuration.Attributes.GetPlayersCountPerRod(RodType);
                }
                return _playersCount;
            }
        }

        /// <summary>
        /// Distance between players on rod
        /// </summary>
        public int PlayersYDistance
        {
            get
            {
                if (_playersYDistance == Int32.MinValue)
                {
                    _playersYDistance = Configuration.Attributes.GetPlayersDistancePerRod(RodType);
                }
                return _playersYDistance;
            }
        }

        /// <summary>
        /// First Player Offset from stopper
        /// </summary>
        public int FirstPlayerOffsetY
        {
            get
            {
                if (_firstPlayerOffsetY == Int32.MinValue)
                {
                    _firstPlayerOffsetY = Configuration.Attributes.GetPlayersOffsetYPerRod(RodType);
                }
                return _firstPlayerOffsetY;
            }
        }

        /// <summary>
        /// X Coordinate (Real World) of rod
        /// </summary>
        public int XCoordinate 
        {
            get
            {
                if (_xCoordinate == Int32.MinValue)
                {
                    _xCoordinate = Configuration.Attributes.GetRodXCoordinate(RodType);
                }
                return _xCoordinate;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Current rod type</param>
        public RodData(eRod type)
        {
            RodType = type;
        }
    }
}
