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
using Foosbot.Common.Data;
using Foosbot.Common.Enums;
using Foosbot.Common.Logs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Class is responsible for Screen management
    /// </summary>
    internal class Drawer : DrawerBase
    {
        /// <summary>
        /// Player head radius drawn
        /// </summary>
        private const int PLAYER_RADIUS = 12;

        /// <summary>
        /// Players state (positions)
        /// </summary>
        private PlayersState _state;

        /// <summary>
        /// Drawer constructor
        /// </summary>
        /// <param name="dispatcher">Dispatcher to work in GUI thread</param>
        /// <param name="canvas">Canvas to draw on</param>
        /// <param name="frameWidth">Frame width</param>
        /// <param name="frameHeigth">Frame heigth</param>
        public Drawer(Dispatcher dispatcher, Canvas canvas, int frameWidth, int frameHeigth):
            base(dispatcher, canvas, frameWidth, frameHeigth)
        {
            _state = new PlayersState();
        }

        /// <summary>
        /// Draw ball method
        /// </summary>
        /// <param name="center">Ball center as Frame coordinate</param>
        /// <param name="radius">Ball radius in Frame dimensions</param>
        /// <param name="circleColor">Circle color [Default will be set as Brushes.Red]</param>
        public void DrawBall(Point center, int radius, SolidColorBrush circleColor = null)
        {
            float screenRadius = _screen.FrameToScreenDistance(radius);
            _dispatcher.Invoke(() =>
            {
                _screen.Element<Shape>(eMarks.BallMark).Fill = Brushes.White;
                _screen.Element<Shape>(eMarks.BallMark).StrokeThickness = 2;
                _screen.Element<Shape>(eMarks.BallMark).Stroke = (circleColor == null) ? Brushes.Red : circleColor;
                _screen.Element<Shape>(eMarks.BallMark).Width = screenRadius * 2;
                _screen.Element<Shape>(eMarks.BallMark).Height = screenRadius * 2;
                _screen.Draw(eMarks.BallMark, center.X - radius, center.Y - radius);
            });
        }

        /// <summary>
        /// Draw Ball Vector Method
        /// </summary>
        /// <param name="center"></param>
        /// <param name="vector"></param>
        /// <param name="type">Type of coordinates provided. Note: only FoosbotWorld and Frame are supported</param>
        /// <param name="color">Color of vector [Default will be set as Brushes.Aqua]</param>
        public void DrawBallVector(Point center, Point vector, eCoordinatesType type = eCoordinatesType.FoosbotWorld, SolidColorBrush color = null)
        {
            VerifyCoordinatesTypeSupported(MethodBase.GetCurrentMethod(), type, eCoordinatesType.FoosbotWorld, eCoordinatesType.Frame);

            //calculate line end point
            Point end = new Point(center.X + vector.X, center.Y + vector.Y);

            //Data provided as foosbot coordinates convert to frame coordinates
            if (type == eCoordinatesType.FoosbotWorld)
            {
                center = TransformAgent.Data.InvertTransform(center);
                end = TransformAgent.Data.InvertTransform(end);
            }

            _dispatcher.Invoke(() =>
            {
                _screen.Element<Line>(eMarks.BallVectorArrow).StrokeThickness = 2;
                _screen.Element<Line>(eMarks.BallVectorArrow).Stroke = (color == null) ? Brushes.Aqua : color;
                _screen.Element<Line>(eMarks.BallVectorArrow).X1 = _screen.FrameToScreenCoordinates(center).X;
                _screen.Element<Line>(eMarks.BallVectorArrow).Y1 = _screen.FrameToScreenCoordinates(center).Y;
                _screen.Element<Line>(eMarks.BallVectorArrow).X2 = _screen.FrameToScreenCoordinates(end).X;
                _screen.Element<Line>(eMarks.BallVectorArrow).Y2 = _screen.FrameToScreenCoordinates(end).Y;

                _screen.Draw(eMarks.BallVectorArrow, 0, 0, eCoordinatesType.Screen);
            });
        }

        /// <summary>
        /// Show table borders calculated by IP Unit
        /// Note only frame coordinates are supported by this method
        /// </summary>
        /// <param name="marks">Calibration marks as points dictionary</param>
        public void DrawTableBorders(Dictionary<eCallibrationMark, Emgu.CV.Structure.CircleF> marks)
        {
            //Convert calibration mark to points
            Point[] corners = new Point[] {
                        new Point(marks[eCallibrationMark.BL].Center.X, marks[eCallibrationMark.BL].Center.Y),
                        new Point(marks[eCallibrationMark.TL].Center.X, marks[eCallibrationMark.TL].Center.Y),
                        new Point(marks[eCallibrationMark.TR].Center.X, marks[eCallibrationMark.TR].Center.Y),
                        new Point(marks[eCallibrationMark.BR].Center.X, marks[eCallibrationMark.BR].Center.Y)   };

            //Draw all 4 borders starting from LeftBorder
            const int key = (int)eMarks.LeftBorder;
            for (int start = 0; start < 4; start++)
            {
                int end = (start + 1 > 3) ? 0 : start + 1;
                _dispatcher.Invoke(() =>
                {
                    _screen.Element<Line>((eMarks)(key + start)).StrokeThickness = 2;
                    _screen.Element<Line>((eMarks)(key + start)).Stroke = Brushes.Pink;
                    _screen.Element<Line>((eMarks)(key + start)).X1 = _screen.FrameToScreenCoordinates(corners[start]).X;
                    _screen.Element<Line>((eMarks)(key + start)).Y1 = _screen.FrameToScreenCoordinates(corners[start]).Y;
                    _screen.Element<Line>((eMarks)(key + start)).X2 = _screen.FrameToScreenCoordinates(corners[end]).X;
                    _screen.Element<Line>((eMarks)(key + start)).Y2 = _screen.FrameToScreenCoordinates(corners[end]).Y;
                    _screen.Draw((eMarks)(key + start), 0, 0, eCoordinatesType.Screen);
                });
            }
        }

        /// <summary>
        /// Draw calibration mark with text method
        /// Note: this method accepts only frame coordinates type
        /// </summary>
        /// <param name="mark">Calibration mark to draw</param>
        /// <param name="center">Calibration mark center as frame coordinates</param>
        /// <param name="radius">Calibration mark radius in frame dimensions</param>
        /// <param name="circleColor">Calibration mark circle color [Default will be Brushes.Pink]</param>
        /// <param name="textColor">Calibration mark text color [Default will be Brushes.OrangeRed]</param>
        /// <param name="fontSize">Calibration mark text font size [Default will be 12]</param>
        /// <param name="text">Calibration mark text [Default will be Mark Name and coordinates]</param>
        public void DrawCallibrationCircle(eCallibrationMark mark, Point center, int radius,
            SolidColorBrush circleColor = null, SolidColorBrush textColor = null, double fontSize = 12,
                string text = "")
        {
            Point screenCenter = _screen.FrameToScreenCoordinates(center);
            float screenRadius = _screen.FrameToScreenDistance(radius);

            eMarks markNum;
            eMarks textNum;
            TryParseToCalibrationCircleAndTextMark(mark, out markNum, out textNum);

            _dispatcher.Invoke(() =>
            {
                _screen.Element<Shape>(markNum).StrokeThickness = 2;
                _screen.Element<Shape>(markNum).Stroke = (circleColor == null) ? Brushes.Pink : circleColor;
                _screen.Element<Shape>(markNum).Width = screenRadius * 2;
                _screen.Element<Shape>(markNum).Height = screenRadius * 2;
                _screen.Element<TextBlock>(textNum).FontSize = fontSize;
                _screen.Element<TextBlock>(textNum).Text = (String.IsNullOrEmpty(text)) ? String.Format("{0}:{1}x{2}", mark, Convert.ToInt32(center.X), Convert.ToInt32(center.Y)) : text;
                _screen.Element<TextBlock>(textNum).Foreground = (textColor == null) ? Brushes.OrangeRed : textColor;
                _screen.Draw(markNum, screenCenter.X - screenRadius, screenCenter.Y - screenRadius, eCoordinatesType.Screen);
                _screen.Draw(textNum, screenCenter.X - screenRadius, screenCenter.Y - screenRadius, eCoordinatesType.Screen);
            });
        }

        /// <summary>
        /// Draw rod lines on the canvas
        /// </summary>
        /// <param name="thickness">Optional thickness of the rods [default : 6]</param>
        public void DrawRodLines(int thickness = 6)
        {
            foreach(eRod type in Enum.GetValues(typeof(eRod)))
            {
                //Get Rod coordinate in a real world
                Point buttom = new Point(Data.GetRod(type).XCoordinate, Data.TableMaxY);
                Point top = new Point(Data.GetRod(type).XCoordinate, 0);

                //Get Rod coordiantes as Screen coordinates
                buttom = ConvertPointFromRealWorldToScreen(buttom);
                top = ConvertPointFromRealWorldToScreen(top);

                eMarks mark = (eMarks)((int)eMarks.GoalKeeper - 1 + type);
                _dispatcher.Invoke(() =>
                {
                    _screen.Element<Line>(mark).StrokeThickness = thickness;
                    _screen.Element<Line>(mark).Stroke = Brushes.White;
                    _screen.Element<Line>(mark).X1 = top.X;
                    _screen.Element<Line>(mark).Y1 = top.Y;
                    _screen.Element<Line>(mark).X2 = buttom.X;
                    _screen.Element<Line>(mark).Y2 = buttom.Y;
                    _screen.Draw(mark, 0, 0, eCoordinatesType.Screen);
                });
            }
        }

        /// <summary>
        /// Draw sector of selected rod
        /// </summary>
        /// <param name="type">Rod type to draw sector for</param>
        /// <param name="dynamicSectorWidth">Sector width in MM (Real World coordinates)</param>
        /// <param name="thickness">Line thickness [Default is 2]</param>
        public void DrawSector(eRod type, int dynamicSectorWidth, int thickness = 2)
        {
            //Get sector line points in a real world
            Point lineAButtom = new Point(Data.GetRod(type).XCoordinate + dynamicSectorWidth / 2.0, Data.TableMaxY);
            Point lineATop = new Point(Data.GetRod(type).XCoordinate + dynamicSectorWidth / 2.0, 0);
            Point lineBButtom = new Point(Data.GetRod(type).XCoordinate - dynamicSectorWidth / 2.0, Data.TableMaxY);
            Point lineBTop = new Point(Data.GetRod(type).XCoordinate - dynamicSectorWidth / 2.0, 0);

            //Get sector line point coordiantes as Screen coordinates
            lineAButtom = ConvertPointFromRealWorldToScreen(lineAButtom);
            lineATop = ConvertPointFromRealWorldToScreen(lineATop);
            lineBButtom = ConvertPointFromRealWorldToScreen(lineBButtom);
            lineBTop = ConvertPointFromRealWorldToScreen(lineBTop);

            //Select color for current sector lines
            Brush color = Brushes.Yellow;
            switch (type)
            {
                case eRod.Defence: color = Brushes.Pink; break;
                case eRod.Midfield: color = Brushes.Red; break;
                case eRod.Attack: color = Brushes.DarkRed; break;
            }

            //get sector line marks for current rod
            eMarks markA, markB;
            TryParseToSectorLineMarks(type, out markA, out markB);

            _dispatcher.Invoke(() =>
            {
                //draw line A
                _screen.Element<Line>(markA).StrokeThickness = thickness;
                _screen.Element<Line>(markA).Stroke = color;
                _screen.Element<Line>(markA).X1 = lineATop.X;
                _screen.Element<Line>(markA).Y1 = lineATop.Y;
                _screen.Element<Line>(markA).X2 = lineAButtom.X;
                _screen.Element<Line>(markA).Y2 = lineAButtom.Y;
                _screen.Draw(markA, 0, 0, eCoordinatesType.Screen);

                //draw line B
                _screen.Element<Line>(markB).StrokeThickness = thickness;
                _screen.Element<Line>(markB).Stroke = color;
                _screen.Element<Line>(markB).X1 = lineBTop.X;
                _screen.Element<Line>(markB).Y1 = lineBTop.Y;
                _screen.Element<Line>(markB).X2 = lineBButtom.X;
                _screen.Element<Line>(markB).Y2 = lineBButtom.Y;
                _screen.Draw(markB, 0, 0, eCoordinatesType.Screen);
            });
        }

        /// <summary>
        /// Draw a rod by a given eMark sign , moving the rod on the linear and rotational axes
        /// </summary>
        /// <param name="rod">eMark of the wanted rod : GoalKeeper, Defense , Midfield, Attack</param>
        /// <param name="linearMoveDestination">Destination of a stopper in MM (Real World)</param>
        /// <param name="rotationalMove">eRotationalMove of the rod : DEFENCE, RISE, ATTACK</param>
        public void DrawRodPlayers(eRod rod, int linearMoveDestination, eRotationalMove rotationalMove)
        {
            //Get all player centers in a real world in current rod
            int x = Data.GetRod(rod).XCoordinate;
            int playersCount = Data.GetRod(rod).PlayersCount;
            int firstPlayerOffset = Data.GetRod(rod).FirstPlayerOffsetY;
            int playersDistance = Data.GetRod(rod).PlayersYDistance;
            
            for (int i = 0; i<playersCount; i++)
            {
                eMarks playerMark = _state.RodTypeToMarkOfFirstPlayerType(rod) + i;
                int y = firstPlayerOffset + i * playersDistance + linearMoveDestination;

                Point center = new Point(x, y);
                center = Data.TableToDeviceCoordinates(center);

                DrawPlayer(playerMark, center, rotationalMove);
            }
        }

        /// <summary>
        /// Get Player (Stopper) position of current rod
        /// </summary>
        /// <param name="type">Rod Type</param>
        /// <returns>Player (Stopper) position of current rod in Foosbot world</returns>
        public Point PlayerPosition(eRod type)
        {
            return _state.Get(type);
        }

        /// <summary>
        /// Drawing a single player at a time , used by the DrawRodPlayers method to draw all rods players
        /// </summary>
        /// <param name="playerMark">eMark of the wanted rod : GoalKeeper, Defense , Midfield, Attack</param>
        /// <param name="center">Center of the players radius in Foosbot WORLD</param>
        /// <param name="radius">Radius size</param>
        /// <param name="circleColor">Color for the rotational current position</param>
        private void DrawPlayer(eMarks playerMark, Point center, eRotationalMove rotationalMove)
        {
            try
            {
                if (rotationalMove != eRotationalMove.NA)
                {
                    //update position of player (only if first on rod)
                    _state.Set(playerMark, center);

                    //get coordinates in Frame dimesions
                    center = TransformAgent.Data.InvertTransform(center);
                    //get coordinates in Screen dimesions
                    center = _screen.FrameToScreenCoordinates(center);

                    //Select color based on Rotational Move
                    SolidColorBrush color = Brushes.Blue; //default for defense
                    int bodyOffset = 0;
                    switch(rotationalMove)
                    {
                        case eRotationalMove.KICK:
                            color = Brushes.DarkBlue;
                            bodyOffset = -PLAYER_RADIUS;
                            break;
                        case eRotationalMove.RISE:
                            color = Brushes.Cyan;
                            bodyOffset = Convert.ToInt32(PLAYER_RADIUS * 1.5);
                            break;
                    }

                    _dispatcher.Invoke(() =>
                    {
                        //Draw Player Body
                        _screen.Element<Shape>(playerMark+5).Fill = color;
                        _screen.Element<Shape>(playerMark + 5).Width = PLAYER_RADIUS * 2.5;
                        _screen.Element<Shape>(playerMark + 5).Height = PLAYER_RADIUS * 2;
                        _screen.Draw(playerMark + 5, center.X - PLAYER_RADIUS - bodyOffset, center.Y - PLAYER_RADIUS, eCoordinatesType.Screen, 10);


                        //Draw Player Head
                        _screen.Element<Shape>(playerMark).Fill = Brushes.DarkBlue;
                        _screen.Element<Shape>(playerMark).Stroke = Brushes.White;
                        _screen.Element<Shape>(playerMark).Width = PLAYER_RADIUS * 2;
                        _screen.Element<Shape>(playerMark).Height = PLAYER_RADIUS * 2;
                        _screen.Draw(playerMark, center.X - PLAYER_RADIUS, center.Y - PLAYER_RADIUS, eCoordinatesType.Screen, 20);
                    });
                }
            }
            catch (Exception e)
            {
                Log.Print(String.Format("Unable to draw player mark. Reason: {0}", e.Message), eCategory.Warn, LogTag.COMMON);
            }
        }
    }
}
