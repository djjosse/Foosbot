// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using EasyLog;
using Foosbot.Common.Enums;
using Foosbot.Common.Logs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Surface (Canvas wrapper) to draw system state in GUI
    /// </summary>
    internal class Surface
    {
        /// <summary>
        /// Initialization flag
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// Instatce of drawing utilities class
        /// </summary>
        private DrawUtils _utils;

        /// <summary>
        /// Instance of drawing data class
        /// </summary>
        private DrawData _data;

        /// <summary>
        /// The canvas object 
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// Dispatcher for drawing on the canvas in the UI thread
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// Markup shape dictionary
        /// Contains markup key and related shape
        /// </summary>
        private Dictionary<int, FrameworkElement> _markups;

        /// <summary>
        /// Surface Constructor
        /// </summary>
        public Surface()
        {
            _utils = new DrawUtils();
        }

        /// <summary>
        /// Surface initialization method
        /// </summary>
        /// <param name="dispatcher">Dispatcher of drawing window</param>
        /// <param name="canvas">Canvas to draw marks on it</param>
        /// <param name="actualWidthRate">Canvas width</param>
        /// <param name="actualHeightRate">Canvas height</param>
        public void Initialize(Dispatcher dispatcher, Canvas canvas, double actualWidthRate, double actualHeightRate)
        {
            if (!_isInitialized)
            {
                _canvas = canvas;
                _dispatcher = dispatcher;

                _data = new DrawData(_utils, actualWidthRate, actualHeightRate);
                _data.ReadConfiguration();

                CreateMarkup();
                CreateElementsOnCanvas();

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Get current first player position of selected rod
        /// </summary>
        /// <param name="type">Rod Type</param>
        /// <returns>Selected rod first player position point</returns>
        public Point PlayerPosition(eRod type)
        {
            eMarks mark = _utils.RodTypeToFirstPlayerMark(type);
            return _data[mark];
        }

        /// <summary>
        /// Drawing the ball on the canvas
        /// </summary>
        /// <param name="center">Ball circle center</param>
        /// <param name="radius">Ball radius</param>
        /// <param name="circleColor">Ball color : optional</param>
        public void DrawBall(Point center, int radius, SolidColorBrush circleColor = null)
        {

            const int key = (int)eMarks.BallMark;
            Point presentationCenter = new Point(center.X * _data.WidthRate, center.Y * _data.HeightRate);
            int presentationRadius = Convert.ToInt32(radius * ((_data.WidthRate + _data.HeightRate) / 2));

            _dispatcher.Invoke(new ThreadStart(delegate
            {
                (_markups[key] as Shape).Fill = Brushes.White;
                (_markups[key] as Shape).StrokeThickness = 2;
                (_markups[key] as Shape).Stroke = (circleColor == null) ? Brushes.Red : circleColor;

                _markups[key].Width = presentationRadius * 2;
                _markups[key].Height = presentationRadius * 2;

                Canvas.SetLeft(_markups[key], presentationCenter.X - presentationRadius);
                Canvas.SetTop(_markups[key], presentationCenter.Y - presentationRadius);
            }));

        }

        /// <summary>
        /// Draw the calibration circles on the canvas
        /// </summary>
        /// <param name="mark">The wanted calibration mark for drawing</param>
        /// <param name="center">Center of the given calibration mark</param>
        /// <param name="radius">Radius of the given calibration mark</param>
        /// <param name="circleColor">Optional color [default : Pink]</param>
        /// <param name="textColor">Optional text color [default : OrangeRed]</param>
        /// <param name="fontSize">Optional font size [default : 12 ]</param>
        /// <param name="text"></param>
        public void DrawCallibrationCircle(eCallibrationMark mark, Point center, int radius,
            SolidColorBrush circleColor = null, SolidColorBrush textColor = null, double fontSize = 12,
                string text = "")
        {
            Point presentationCenter = new Point(center.X * _data.WidthRate, center.Y * _data.HeightRate);
            int presentationRadius = Convert.ToInt32(radius * ((_data.WidthRate + _data.HeightRate) / 2));

            _dispatcher.Invoke(new ThreadStart(delegate
            {
                int markNum = 0;
                int textNum = 0;
                switch (mark)
                {
                    case eCallibrationMark.BL:
                        markNum = (int)eMarks.ButtomLeftMark;
                        textNum = (int)eMarks.ButtomLeftText;
                        break;
                    case eCallibrationMark.BR:
                        markNum = (int)eMarks.ButtomRightMark;
                        textNum = (int)eMarks.ButtomRightText;
                        break;
                    case eCallibrationMark.TL:
                        markNum = (int)eMarks.TopLeftMark;
                        textNum = (int)eMarks.TopLeftText;
                        break;
                    case eCallibrationMark.TR:
                        markNum = (int)eMarks.TopRightMark;
                        textNum = (int)eMarks.TopRightText;
                        break;
                }

                (_markups[markNum] as Shape).StrokeThickness = 2;
                (_markups[markNum] as Shape).Stroke = (circleColor == null) ? Brushes.Pink : circleColor;
                (_markups[textNum] as TextBlock).FontSize = fontSize;
                (_markups[textNum] as TextBlock).Text = (String.IsNullOrEmpty(text)) ? String.Format("{0}:{1}x{2}", mark, Convert.ToInt32(center.X), Convert.ToInt32(center.Y)) : text;
                (_markups[textNum] as TextBlock).Foreground = (textColor == null) ? Brushes.OrangeRed : textColor;

                _markups[markNum].Width = presentationRadius * 2;
                _markups[markNum].Height = presentationRadius * 2;
                Canvas.SetLeft(_markups[markNum], presentationCenter.X - presentationRadius);
                Canvas.SetTop(_markups[markNum], presentationCenter.Y - presentationRadius);
                Canvas.SetLeft(_markups[textNum], presentationCenter.X - presentationRadius);
                Canvas.SetTop(_markups[textNum], presentationCenter.Y - presentationRadius);
            }));

        }

        /// <summary>
        /// Drawing the vector of the ball
        /// </summary>
        /// <param name="center">Center point to draw vector from</param>
        /// <param name="vector">The end of the vector</param>
        /// <param name="isLocation">Optional is location [default : true]</param>
        /// <param name="color">Optional color [default : Aqua]</param>
        public void DrawBallVector(Point center, Point vector, bool isLocation = true, SolidColorBrush color = null)
        {
            if (isLocation)
            {
                int x = Convert.ToInt32(center.X);
                int y = Convert.ToInt32(center.Y);
                _utils.ConvertToLocation(ref x, ref y);
                center = new Point(x, y);

                x = Convert.ToInt32(vector.X);
                y = Convert.ToInt32(vector.Y);

                //we don't want to convert 0 of vector
                int convX = x;
                int convY = y;
                _utils.ConvertToLocation(ref convX, ref convY);
                if (x != 0) x = convX;
                if (y != 0) y = convY;
                vector = new Point(x, y);
            }

            Point presentationStartPoint = new Point(center.X * _data.WidthRate, center.Y * _data.HeightRate);
            Point presentationVector = new Point(vector.X * _data.WidthRate, vector.Y * _data.HeightRate);
            Point presentationEndPoint = new Point(presentationStartPoint.X + presentationVector.X,
                presentationStartPoint.Y + presentationVector.Y);
            const int key = (int)eMarks.BallVectorArrow;

            _dispatcher.Invoke(new ThreadStart(delegate
            {
                for (int i = key; i <= key + 2; i++)
                {
                    (_markups[i] as Line).StrokeThickness = 2;
                    (_markups[i] as Line).Stroke = (color == null) ? System.Windows.Media.Brushes.Aqua : color;
                    (_markups[i] as Line).X1 = presentationStartPoint.X;
                    (_markups[i] as Line).Y1 = presentationStartPoint.Y;
                }
                (_markups[key] as Line).X2 = presentationEndPoint.X;
                (_markups[key] as Line).Y2 = presentationEndPoint.Y;

                (_markups[key + 1] as Line).X2 = presentationEndPoint.X;
                (_markups[key + 1] as Line).Y2 = presentationEndPoint.Y;
                (_markups[key + 2] as Line).X2 = presentationEndPoint.X;
                (_markups[key + 2] as Line).Y2 = presentationEndPoint.Y;

                for (int i = key; i <= key /*+ 2*/; i++)
                {
                    Canvas.SetLeft(_markups[i], 0);
                    Canvas.SetTop(_markups[i], 0);
                }
            }));

        }

        /// <summary>
        /// Draw the ricochet point of the ball on the border of the table 
        /// </summary>
        /// <param name="x">X coordinate of the ricochet on the canvas</param>
        /// <param name="y">Y coordinate of the ricochet on the canvas</param>
        /// <param name="isLocation">Bool that convert between location and coordinate : optional</param>
        /// <param name="circleColor">The color of the stroke of the ricochet mark : optional</param>
        public void DrawRicochetMark(int x, int y, bool isLocation = false, SolidColorBrush circleColor = null)
        {
            if (isLocation) _utils.ConvertToLocation(ref x, ref y);

            const int radius = 10;
            const int key = (int)eMarks.RicochetMark;
            Point presentationCenter = new Point(x * _data.WidthRate, y * _data.HeightRate);
            int presentationRadius = Convert.ToInt32(radius * ((_data.WidthRate + _data.HeightRate) / 2));

            _dispatcher.Invoke(new ThreadStart(delegate
            {
                (_markups[key] as Shape).StrokeThickness = 2;
                (_markups[key] as Shape).Stroke = (circleColor == null) ? Brushes.Chocolate : circleColor;
                _markups[key].Width = presentationRadius * 2;
                _markups[key].Height = presentationRadius * 2;
                Canvas.SetLeft(_markups[key], presentationCenter.X - presentationRadius);
                Canvas.SetTop(_markups[key], presentationCenter.Y - presentationRadius);
            }));

        }

        /// <summary>
        /// Show table borders calculated by IP Unit
        /// </summary>
        /// <param name="corners"></param>
        public void DrawTableBorders(Dictionary<eCallibrationMark, Emgu.CV.Structure.CircleF> marks)
        {

            Point[] corners = new Point[]
                    {
                        new Point(marks[eCallibrationMark.BL].Center.X, marks[eCallibrationMark.BL].Center.Y),
                        new Point(marks[eCallibrationMark.TL].Center.X, marks[eCallibrationMark.TL].Center.Y),
                        new Point(marks[eCallibrationMark.TR].Center.X, marks[eCallibrationMark.TR].Center.Y),
                        new Point(marks[eCallibrationMark.BR].Center.X, marks[eCallibrationMark.BR].Center.Y)
                    };

            const int key = (int)eMarks.LeftBorder;
            for (int start = 0; start < 4; start++)
            {
                int end = (start + 1 > 3) ? 0 : start + 1;

                _dispatcher.Invoke(new ThreadStart(delegate
                {
                    (_markups[key + start] as Shape).StrokeThickness = 2;
                    (_markups[key + start] as Shape).Stroke = Brushes.Pink;

                    (_markups[key + start] as Line).X1 = corners[start].X * _data.WidthRate;
                    (_markups[key + start] as Line).Y1 = corners[start].Y * _data.HeightRate;
                    (_markups[key + start] as Line).X2 = corners[end].X * _data.WidthRate;
                    (_markups[key + start] as Line).Y2 = corners[end].Y * _data.HeightRate;

                    Canvas.SetLeft(_markups[key + start], 0);
                    Canvas.SetTop(_markups[key + start], 0);
                }));
            }

        }

        /// <summary>
        /// Draw rod lines on the canvas
        /// </summary>
        /// <param name="thickness">Optional thickness of the rods [default : 6]</param>
        /// <param name="isLocation">Optional isLocation [default : true]</param>
        public void DrawRods(int thickness = 6, bool isLocation = true)
        {
            _dispatcher.Invoke(new ThreadStart(() =>
            {
                const int key = (int)eMarks.GoalKeeper;

                for (int eMarksCounter = 0; eMarksCounter < 4; eMarksCounter++)
                {

                    int buttomX = _data.XTableToDeviceCoordinates(_data.Rods[(eMarksCounter + eMarks.GoalKeeper)]);
                    int buttomY = ((int)_data.DEVICE_MAX_Y);
                    int topX = buttomX;
                    int topY = 0;

                    if (isLocation)
                    {
                        _utils.ConvertToLocation(ref buttomX, ref buttomY);
                        _utils.ConvertToLocation(ref topX, ref topY);
                    }

                    (_markups[key + eMarksCounter] as Shape).StrokeThickness = thickness;
                    (_markups[key + eMarksCounter] as Shape).Stroke = Brushes.White;
                    (_markups[key + eMarksCounter] as Line).X1 = topX * _data.WidthRate;
                    (_markups[key + eMarksCounter] as Line).Y1 = topY * _data.HeightRate;
                    (_markups[key + eMarksCounter] as Line).X2 = buttomX * _data.WidthRate;
                    (_markups[key + eMarksCounter] as Line).Y2 = buttomY * _data.HeightRate;

                    Canvas.SetLeft(_markups[key + eMarksCounter], 0);
                    Canvas.SetTop(_markups[key + eMarksCounter], 0);
                }
            }));

        }

        /// <summary>
        /// Draw the given rod dynamic sector
        /// </summary>
        /// <param name="rod">The rod for sector calculation</param>
        /// <param name="dynamicSectorWidth">The dynamic sector of the rod</param>
        /// <param name="thickness">Optional thickness of the sector line [default : 2]</param>
        /// <param name="isLocation">Optional isLocation [default : true]</param>
        public void DrawSector(eRod rod, int dynamicSectorWidth, int thickness = 2, bool isLocation = true)
        {
            Brush color = null;
            eMarks mark;
            Enum.TryParse<eMarks>(rod.ToString(), out mark);

            int key = (int)mark * 10;
            int x = _data.XTableToDeviceCoordinates(_data.Rods[mark]);

            int sectorStart = Convert.ToInt32(x - dynamicSectorWidth / 2.0);
            int sectorStartTop = sectorStart;
            int sectorStartButtom = sectorStart;

            int sectorEnd = Convert.ToInt32(x + dynamicSectorWidth / 2.0);
            int sectorEndTop = sectorEnd;
            int sectorEndButtom = sectorEnd;

            int yTop = 0;
            int yButtom = ((int)_data.DEVICE_MAX_Y);

            _dispatcher.Invoke(new ThreadStart(delegate
            {

                switch (mark)
                {
                    case eMarks.GoalKeeper: color = Brushes.Yellow; break;
                    case eMarks.Defence: color = Brushes.Pink; break;
                    case eMarks.Midfield: color = Brushes.Red; break;
                    case eMarks.Attack: color = Brushes.DarkRed; break;
                    default: break;
                }

                if (isLocation)
                {
                    int yButtomTemp = yButtom;
                    int yTopTemp = yTop;
                    _utils.ConvertToLocation(ref sectorStartTop, ref yTop);
                    _utils.ConvertToLocation(ref sectorStartButtom, ref yButtom);
                    _utils.ConvertToLocation(ref sectorEndTop, ref yTopTemp);
                    _utils.ConvertToLocation(ref sectorEndButtom, ref yButtomTemp);
                }

                (_markups[key] as Shape).StrokeThickness = thickness;
                (_markups[key] as Shape).Stroke = color;
                (_markups[key] as Line).X1 = sectorStartTop * _data.WidthRate;
                (_markups[key] as Line).Y1 = yTop * _data.HeightRate;
                (_markups[key] as Line).X2 = sectorStartButtom * _data.WidthRate;
                (_markups[key] as Line).Y2 = yButtom * _data.HeightRate;

                (_markups[key + 1] as Shape).StrokeThickness = thickness;
                (_markups[key + 1] as Shape).Stroke = color;
                (_markups[key + 1] as Line).X1 = sectorEndTop * _data.WidthRate;
                (_markups[key + 1] as Line).Y1 = yTop * _data.HeightRate;
                (_markups[key + 1] as Line).X2 = sectorEndButtom * _data.WidthRate;
                (_markups[key + 1] as Line).Y2 = yButtom * _data.HeightRate;

                Canvas.SetLeft(_markups[key], 0);
                Canvas.SetTop(_markups[key], 0);
                Canvas.SetLeft(_markups[key + 1], 0);
                Canvas.SetTop(_markups[key + 1], 0);

            }));

        }

        /// <summary>
        /// Draw a rod by a given eMark sign , moving the rod on the linear and rotational axes
        /// </summary>
        /// <param name="rod">eMark of the wanted rod : GoalKeeper, Defense , Midfield, Attack</param>
        /// <param name="deltaYMovment">The change on the linear movement</param>
        /// <param name="rotationalMove">eRotationalMove of the rod : DEFENCE, RISE, ATTACK</param>
        public void DrawRodPlayers(eRod rod, int linearMoveDestination, eRotationalMove rotationalMove, bool isLocation = true)
        {
            eMarks playersBase = 0;
            eMarks mark;
            Enum.TryParse<eMarks>(rod.ToString(), out mark);

            int eMarkType = ((int)mark);
            int rodPlayersCount = _data.PlayerCount[mark];
            int yDistance = _data.PlayersDistance[mark];
            int firstPlayerOffsetY = _data.OffsetY[mark];
            int x = _data.XTableToDeviceCoordinates(_data.Rods[mark]);

            switch (eMarkType)
            {
                case (int)eMarks.GoalKeeper: playersBase = eMarks.GoalKeeperPlayer; break;
                case (int)eMarks.Defence: playersBase = eMarks.DefencePlayer1; break;
                case (int)eMarks.Midfield: playersBase = eMarks.MidfieldPlayer1; break;
                case (int)eMarks.Attack: playersBase = eMarks.AttackPlayer1; break;
                default: break;
            }

            for (int rodPlayer = 0; rodPlayer < rodPlayersCount; rodPlayer++)
            {
                int y = linearMoveDestination + firstPlayerOffsetY + yDistance * rodPlayer;
                int y1 = y;
                int x1 = x;
                if (isLocation)
                {
                    _utils.ConvertToLocation(ref x1, ref y);
                }
                DrawPlayer(playersBase + rodPlayer, new Point(x1 * _data.WidthRate, y1 * _data.HeightRate), 12, rotationalMove);
            }

        }

        /// <summary>
        /// Drawing a single player at a time , used by the DrawRodPlayers method to draw all rods players
        /// </summary>
        /// <param name="eNumKey">eMark of the wanted rod : GoalKeeper, Defense , Midfield, Attack</param>
        /// <param name="center">Center of the players radius</param>
        /// <param name="radius">Radius size</param>
        /// <param name="circleColor">Color for the rotational current position</param>
        private void DrawPlayer(eMarks eNumKey, Point center, int radius,
                    eRotationalMove rotationalMove, SolidColorBrush playerColor = null)
        {
            try
            {
                if (_utils.IsMarkOfFirstPlayerInRod(eNumKey)) _data[eNumKey] = new Point(center.X / Convert.ToInt32(_data.WidthRate),
                                                                                                     center.Y / Convert.ToInt32(_data.HeightRate));

                int key = (int)eNumKey;
                int rotationalMoveFactor = 0;
                SolidColorBrush rotationalColor = Brushes.DarkBlue;

                if (rotationalMove == eRotationalMove.DEFENCE)
                {
                    rotationalMoveFactor = radius;
                    rotationalColor = Brushes.Blue;
                }
                else if (rotationalMove == eRotationalMove.RISE)
                {
                    rotationalMoveFactor = (int)(radius * 2.5);
                    rotationalColor = Brushes.Cyan;
                }

                _dispatcher.Invoke(new ThreadStart(delegate
                {
                    (_markups[key] as Shape).Fill = (rotationalColor == null) ? Brushes.White : rotationalColor;

                    _markups[key].Width = radius * 2;
                    _markups[key].Height = radius * 2;

                    Canvas.SetLeft(_markups[key], center.X - radius);
                    Canvas.SetTop(_markups[key], center.Y - radius);

                    (_markups[key + 5] as Shape).Height = 24;
                    (_markups[key + 5] as Shape).Width = 30;
                    (_markups[key + 5] as Shape).StrokeThickness = 2;
                    (_markups[key + 5] as Shape).Fill = rotationalColor;

                    Canvas.SetLeft(_markups[key + 5], center.X - rotationalMoveFactor);
                    Canvas.SetTop(_markups[key + 5], center.Y - radius);

                }));
            }
            catch (Exception e)
            {
                Log.Print(String.Format("Unable to draw player mark. Reason: {0}", e.Message), eCategory.Warn, LogTag.COMMON);
            }
        }

        /// <summary>
        /// Create and initialize all marks in order to draw them
        /// </summary>
        private void CreateMarkup()
        {
            _markups = new Dictionary<int, FrameworkElement>();
            foreach (eMarks mark in Enum.GetValues(typeof(eMarks)))
            {
                switch (mark)
                {
                    case eMarks.BallMark:
                    case eMarks.ButtomLeftMark:
                    case eMarks.ButtomRightMark:
                    case eMarks.TopLeftMark:
                    case eMarks.TopRightMark:
                    case eMarks.RicochetMark:
                    case eMarks.GoalKeeperPlayer:
                    case eMarks.DefencePlayer1:
                    case eMarks.DefencePlayer2:
                    case eMarks.MidfieldPlayer1:
                    case eMarks.MidfieldPlayer2:
                    case eMarks.MidfieldPlayer3:
                    case eMarks.MidfieldPlayer4:
                    case eMarks.MidfieldPlayer5:
                    case eMarks.AttackPlayer1:
                    case eMarks.AttackPlayer2:
                    case eMarks.AttackPlayer3:
                        _markups.Add((int)mark, new Ellipse());
                        break;
                    case eMarks.GoalKeeperPlayerRect:
                    case eMarks.DefencePlayer1Rect:
                    case eMarks.DefencePlayer2Rect:
                    case eMarks.MidfieldPlayer1Rect:
                    case eMarks.MidfieldPlayer2Rect:
                    case eMarks.MidfieldPlayer3Rect:
                    case eMarks.MidfieldPlayer4Rect:
                    case eMarks.MidfieldPlayer5Rect:
                    case eMarks.AttackPlayer1Rect:
                    case eMarks.AttackPlayer2Rect:
                    case eMarks.AttackPlayer3Rect:
                        _markups.Add((int)mark, new Rectangle());
                        break;
                    case eMarks.ButtomLeftText:
                    case eMarks.ButtomRightText:
                    case eMarks.TopLeftText:
                    case eMarks.TopRightText:
                        _markups.Add((int)mark, new TextBlock());
                        break;
                    case eMarks.BallVectorArrow:
                        _markups.Add((int)mark, new Line());
                        _markups.Add((int)eMarks.BallVectorArrow + 1, new Line());
                        _markups.Add((int)eMarks.BallVectorArrow + 2, new Line());
                        break;
                    case eMarks.LeftBorder:
                    case eMarks.RightBorder:
                    case eMarks.TopBorder:
                    case eMarks.BottomBorder:
                    case eMarks.GoalKeeper:
                    case eMarks.Defence:
                    case eMarks.Midfield:
                    case eMarks.Attack:
                    case eMarks.GoalKeeperSector1:
                    case eMarks.GoalKeeperSector2:
                    case eMarks.DefenceSector1:
                    case eMarks.DefenceSector2:
                    case eMarks.MidfieldSector1:
                    case eMarks.MidfieldSector2:
                    case eMarks.AttackSector1:
                    case eMarks.AttackSector2:
                        _markups.Add((int)mark, new Line());
                        break;
                }
            }
        }

        /// <summary>
        /// Add all marks on canvas
        /// </summary>
        private void CreateElementsOnCanvas()
        {
            foreach (FrameworkElement element in _markups.Values)
            {
                _canvas.Children.Add(element);
            }
        }
    }
}
