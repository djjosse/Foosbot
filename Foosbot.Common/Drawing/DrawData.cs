using Foosbot.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foosbot.Common.Drawing
{
    internal class DrawData
    {
        /// <summary>
        /// Holds the maximum of the y coordinate in the device world
        /// </summary>
        public double DEVICE_MAX_Y { get; private set; }

        /// <summary>
        /// Holds the maximum of the x coordinate in the device world
        /// </summary>
        public double DEVICE_MAX_X { get; private set; }

        /// <summary>
        /// Holds the real life table max y in MM
        /// </summary>
        public double TABLE_MAX_Y_MM { get; private set; }

        /// <summary>
        /// Holds the real life table max x in MM
        /// </summary>
        public double TABLE_MAX_X_MM { get; private set; }

        /// <summary>
        /// The main dictionary that holds all the marks
        /// </summary>
        public Dictionary<eMarks, int> Rods { get; private set; }

        /// <summary>
        /// Holds the number of player for each rod
        /// </summary>
        public Dictionary<eMarks, int> PlayerCount { get; private set; }

        /// <summary>
        /// Holds the distance between 2 players in each rod
        /// </summary>
        public Dictionary<eMarks, int> PlayersDistance { get; private set; }

        /// <summary>
        /// Holds the offset of the first player for each rod from the stopper
        /// </summary>
        public Dictionary<eMarks, int> OffsetY { get; private set; }

        /// <summary>
        /// Holds the width rate of the table draw on the canvas
        /// </summary>
        public double WidthRate { get; private set; }

        /// <summary>
        /// Holds the height rate of the table draw on the canvas
        /// </summary>
        public double HeightRate { get; private set; }

        private Dictionary<eMarks, Point> _playerPositions;

        private Dictionary<eMarks, object> _positionToken;

        private DrawUtils _utils;

        public DrawData(DrawUtils drawUtils, double actualWidthRate, double actualHeightRate)
        {
            _utils = drawUtils;
            WidthRate = actualWidthRate;
            HeightRate = actualHeightRate;

            Rods = new Dictionary<eMarks, int>();
            PlayerCount = new Dictionary<eMarks, int>();
            PlayersDistance = new Dictionary<eMarks, int>();
            OffsetY = new Dictionary<eMarks, int>();

            _playerPositions = new Dictionary<eMarks, Point>();
            _positionToken = new Dictionary<eMarks, object>();

            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                _playerPositions.Add(_utils.RodTypeToFirstPlayerMark(rodType), default(Point));
                _positionToken.Add(_utils.RodTypeToFirstPlayerMark(rodType), new object());
            }
        }

        /// <summary>
        /// Read relevant data from configuration file
        /// </summary>
        public void ReadConfiguration()
        {
            DEVICE_MAX_Y = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            DEVICE_MAX_X = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            TABLE_MAX_Y_MM = Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_HEIGHT);
            TABLE_MAX_X_MM = Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_WIDTH);

            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                int rodPlayersCount = Configuration.Attributes.GetPlayersCountPerRod(rodType);
                int yDistance = Configuration.Attributes.GetPlayersDistancePerRod(rodType);
                int firstPlayerOffsetY = Configuration.Attributes.GetPlayersOffsetYPerRod(rodType);

                int x = Configuration.Attributes.GetRodXCoordinate(rodType);

                eMarks mark;
                Enum.TryParse<eMarks>(rodType.ToString(), out mark);
                if (!mark.Equals(eMarks.NA))
                {
                    Rods.Add(mark, x);
                    PlayerCount.Add(mark, rodPlayersCount);
                    PlayersDistance.Add(mark, yDistance);
                    OffsetY.Add(mark, firstPlayerOffsetY);
                }
            }
        }

        /// <summary>
        /// Current Player Position property
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Point this[eMarks key]
        {
            get 
            {
                if (_playerPositions.ContainsKey(key))
                {
                    Point point = default(Point);
                    lock (_positionToken[key])
                    {
                        point.X = _playerPositions[key].X;
                        point.Y = _playerPositions[key].Y;
                    }
                    return point;
                }
                throw new NotSupportedException(String.Format(
                    "Can't retrieve point mark for [{0}]. Invalid first player position mark!", key));
            }
            set
            {
                lock(_positionToken[key])
                {
                    _playerPositions[key] = value;
                }
            }
        }

        /// <summary>
        /// Convert the a given x from MM to Device coordinates
        /// </summary>
        /// <param name="x">X in MM</param>
        /// <returns>X in coordinates</returns>
        public int XTableToDeviceCoordinates(int x)
        {
            return Convert.ToInt32((x * DEVICE_MAX_X) / TABLE_MAX_X_MM);
        }
    }
}
