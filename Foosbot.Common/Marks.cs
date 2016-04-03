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
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Foosbot
{
    public static class Marks
    {
        private static double _actualWidthRate;
        private static double _actualHeightRate;
        private static Canvas _canvas;
        private static Dispatcher _dispatcher;
        private static double DEVICE_MAX_Y;
        private static double DEVICE_MAX_X;
        private static double TABLE_MAX_Y_MM;
        private static double TABLE_MAX_X_MM;
        private static Dictionary<eMarks, int> _rods;
        private static Dictionary<eMarks, int> _players;

        

        /// <summary>
        /// Markup shape dictionary
        /// Contains markup key and related shape
        /// </summary>
        private static Dictionary<int, FrameworkElement> _markups;

        /// <summary>
        /// Must be called to use markups
        /// </summary>
        public static void Initialize(Dispatcher dispatcher, Canvas canvas, double actualWidthRate, double actualHeightRate)
        {
            DEVICE_MAX_Y = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            DEVICE_MAX_X = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_X_SIZE);

            TABLE_MAX_Y_MM = Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_HEIGHT);
            TABLE_MAX_X_MM = Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_WIDTH); 

            _rods = new Dictionary<eMarks,int>();
            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
	        {
                int x = Configuration.Attributes.GetRodXCoordinate(rodType);
                eMarks mark;
                Enum.TryParse<eMarks>(rodType.ToString(), out mark);
                if (!mark.Equals(eMarks.NA))
                {
                    _rods.Add(mark, x);
                }
	        }


            _canvas = canvas;
            _actualWidthRate = actualWidthRate;
            _actualHeightRate = actualHeightRate;
            _dispatcher = dispatcher;

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
                    case eMarks.GoalKeeper:
                    case eMarks.Defence:
                    case eMarks.Midfield:
                    case eMarks.Attack:
                        _markups.Add((int)mark, new Line());
                        break;
                }
            }

            foreach (FrameworkElement element in _markups.Values)
            {
                _canvas.Children.Add(element);
            }
        }

        private static void ConvertToLocation(ref int x, ref int y)
        {
            double outX, outY;
            Transformation transformer = new Transformation();
            transformer.InvertTransform(x, y, out outX, out outY);
            x = Convert.ToInt32(outX);
            y = Convert.ToInt32(outY);
        }

        private static void ConvertToCoord(ref int x, ref int y)
        {
            double outX, outY;
            Transformation transformer = new Transformation();
            transformer.Transform(x, y, out outX, out outY);
            x = Convert.ToInt32(outX);
            y = Convert.ToInt32(outY);
        }

        /// <summary>
        /// draw the ricochet point of the ball on the border of the table 
        /// </summary>
        /// <param name="x">x coord of the ricochet on the canvas</param>
        /// <param name="y">y coord of the ricochet on the canvas</param>
        /// <param name="isLocation">bool that convert between location and coord : optional</param>
        /// <param name="circleColor">the color of the stroke of the ricochet mark : optional</param>
        public static void DrawRicochetMark(int x, int y, bool isLocation = false, SolidColorBrush circleColor = null)
        {
            if (isLocation)
            {
                ConvertToLocation(ref x, ref y);
            }

            const int radius = 10;
            const int key = (int)eMarks.RicochetMark;
            Point presentationCenter = new Point(x * _actualWidthRate, y * _actualHeightRate);
            int presentationRadius = Convert.ToInt32(radius * ((_actualWidthRate + _actualHeightRate) / 2));

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
        /// init the rods lines on the  canvas --> TODO fix the rods position problem
        /// </summary>
        /// <param name="thickness">the thickness of the rods</param>
        /// <param name="color">the color of the rods</param>
        public static void DrawRods(int thickness = 3, SolidColorBrush color = null ,bool isLocation = true)
        { 
            _dispatcher.Invoke(new ThreadStart(delegate
            {
                const int key = (int)eMarks.GoalKeeper;

                for (int eMarksCounter = 0; eMarksCounter < 4; eMarksCounter++)
                {
                    int x = XTableToDeviceCoordinates(_rods[(eMarksCounter + eMarks.GoalKeeper)]);
                    int y = ((int)DEVICE_MAX_Y);

                    if(isLocation)
                       ConvertToLocation(ref x, ref y);

                    (_markups[key + eMarksCounter] as Shape).StrokeThickness = thickness;
                    (_markups[key + eMarksCounter] as Shape).Stroke = (color == null) ? Brushes.White : color;

                    (_markups[key + eMarksCounter] as Line).X1 = x * _actualWidthRate;
                    (_markups[key + eMarksCounter] as Line).Y1 = 0;

                    (_markups[key + eMarksCounter] as Line).X2 = x * _actualWidthRate;
                    (_markups[key + eMarksCounter] as Line).Y2 = y * _actualHeightRate;

                    Canvas.SetLeft(_markups[key + eMarksCounter], 0);
                    Canvas.SetTop(_markups[key + eMarksCounter], 0);
                }         
            }));
        }

        public static int XTableToDeviceCoordinates(int x)
        {
            return Convert.ToInt32((x * DEVICE_MAX_X) / TABLE_MAX_X_MM);
        }

        public static int YTableToDeviceCoordinates(int y)
        {
            return Convert.ToInt32((y * DEVICE_MAX_Y) / TABLE_MAX_Y_MM);
        }

        /// <summary>
        /// draw a rod by a given eMark sign , moving the rod on the linear and rotational axes
        /// </summary>
        /// <param name="rod">eMark of the wanted rod : GOALKEEPER , Defence , Midfield, Attack</param>
        /// <param name="deltaYMovment">the change on the linear movement</param>
        /// <param name="rotationalMove">eRotationalMove of the rod : DEFENCE, RISE, ATTACK</param>
        public static void DrawRodPlayers(eMarks rod, int deltaYMovment, eRotationalMove rotationalMove ,bool isLocation = true)
        {
            eRod mark;
            Enum.TryParse<eRod>(rod.ToString(), out mark);
            eMarks playersBase = 0;    
   
            int eMarkType = ((int)rod);       
            int count = Configuration.Attributes.GetPlayersCountPerRod(mark);
            int yDistance = Configuration.Attributes.GetPlayersDistancePerRod(mark);
            int xDistance = 0;
            int x = XTableToDeviceCoordinates(_rods[rod]);
            int y = Configuration.Attributes.GetPlayersOffsetYPerRod(mark);

            int movmentOffset = y + deltaYMovment;

            if (isLocation)
            {
                ConvertToLocation(ref x, ref movmentOffset);
                ConvertToLocation(ref xDistance, ref yDistance);
            }

            switch (eMarkType)
            {
                case (int)eMarks.GoalKeeper: playersBase = eMarks.GoalKeeperPlayer; break;
                case (int)eMarks.Defence: playersBase = eMarks.DefencePlayer1; break;
                case (int)eMarks.Midfield: playersBase = eMarks.MidfieldPlayer1; break;
                case (int)eMarks.Attack: playersBase = eMarks.AttackPlayer1; break;
                default: break;
            }

            for (int i = 0; i < count; i++)
            {
                if (i == 0) DrawPlayer(playersBase + i, new Point(x * _actualWidthRate, movmentOffset * _actualHeightRate), 12, rotationalMove);
                else DrawPlayer(playersBase + i, new Point(x * _actualWidthRate, (movmentOffset += yDistance) * _actualHeightRate), 12, rotationalMove);
            }
        }


        /// <summary>
        /// drawing a single player at a time , used by the DrawRodPlayers method to draw all rods players
        /// </summary>
        /// <param name="eNumKey">eMark of the wanted rod : GOALKEEPER , Defence , Midfield, Attack</param>
        /// <param name="center">center of the players radius</param>
        /// <param name="radius">radius size</param>
        /// <param name="circleColor">color for the roational current pos</param>
        private static void DrawPlayer(eMarks eNumKey,Point center, int radius,
                    eRotationalMove rotationalMove, SolidColorBrush playerColor = null)
        {
            int key = (int)eNumKey;
            
            //Point presentationCenter = new Point(center.X * _actualWidthRate, center.Y * _actualHeightRate);
            //int presentationRadius = Convert.ToInt32(radius * ((_actualWidthRate + _actualHeightRate) / 2));

            int rotationalMoveFactor = 0;
            SolidColorBrush rotationalColor = Brushes.DarkBlue;

            if (rotationalMove == eRotationalMove.DEFENCE) {
                rotationalMoveFactor = radius;
                rotationalColor = Brushes.Blue;
            }
            else if (rotationalMove == eRotationalMove.RISE) {
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


        /// <summary>
        /// drawing the ball on the canvas
        /// </summary>
        /// <param name="center">ball circle center</param>
        /// <param name="radius">ball radius</param>
        /// <param name="circleColor">ball color : optional</param>
        public static void DrawBall(Point center, int radius, SolidColorBrush circleColor = null)
        {
            const int key = (int)eMarks.BallMark;
            Point presentationCenter = new Point(center.X * _actualWidthRate, center.Y * _actualHeightRate);
            int presentationRadius = Convert.ToInt32(radius * ((_actualWidthRate + _actualHeightRate) / 2));

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
        /// draw the calibration circles on the canvas
        /// </summary>
        /// <param name="mark"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="circleColor"></param>
        /// <param name="textColor"></param>
        /// <param name="fontSize"></param>
        /// <param name="text"></param>
        public static void DrawCallibrationCircle(eCallibrationMark mark, Point center, int radius,
            SolidColorBrush circleColor = null, SolidColorBrush textColor = null, double fontSize = 12,
                string text = "")
        {
            Point presentationCenter = new Point(center.X * _actualWidthRate, center.Y * _actualHeightRate);
            int presentationRadius = Convert.ToInt32(radius * ((_actualWidthRate + _actualHeightRate) / 2));

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
        /// drawing the vector of the ball
        /// </summary>
        /// <param name="center"></param>
        /// <param name="vector"></param>
        /// <param name="isLocation"></param>
        /// <param name="color"></param>
        public static void DrawBallVector(Point center, Point vector, bool isLocation = false, SolidColorBrush color = null)
        {
            if (isLocation)
            {
                int x = Convert.ToInt32(center.X);
                int y = Convert.ToInt32(center.Y);
                ConvertToLocation(ref x, ref y);
                center = new Point(x, y);

                x = Convert.ToInt32(vector.X);
                y = Convert.ToInt32(vector.Y);
                ConvertToLocation(ref x, ref y);
                vector = new Point(x, y);
            }

            Point presentationStartPoint = new Point(center.X * _actualWidthRate, center.Y * _actualHeightRate);
            Point presentationVector = new Point(vector.X * _actualWidthRate, vector.Y * _actualHeightRate);
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
    }
}
