using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private static Dictionary<eMarks, int> _rods;

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
            _rods = new Dictionary<eMarks,int>();
            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
	        {
                int x = Configuration.Attributes.GetRodXCoordinate(rodType);
                eMarks mark;
                Enum.TryParse<eMarks>(rodType.ToString(), out mark);
                if (!mark.Equals(eMarks.NA))
                    _rods.Add(mark, x);
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
                        _markups.Add((int)mark, new Ellipse());
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
                _canvas.Children.Add(element);
        }

        private static void ConvertToLocation(ref int x, ref int y)
        {
            double outX, outY;
            Transformation transformer = new Transformation();
            transformer.InvertTransform(x, y, out outX, out outY);
            x = Convert.ToInt32(outX);
            y = Convert.ToInt32(outY);
        }

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


        public static void DrawRodtMark(Point start, Point end, int thickness, bool isLocation = false, SolidColorBrush color = null)
        {
            if (isLocation)
            {
                int x = Convert.ToInt32(start.X);
                int y = Convert.ToInt32(start.Y);

                ConvertToLocation(ref x, ref y);
                start = new Point(x, y);

                x = 0;
                y = Convert.ToInt32(end.Y);

                ConvertToLocation(ref x, ref y);
                end = new Point(x, y);
            }

            Point presentationStartPoint = new Point(start.X * _actualWidthRate, start.Y * _actualHeightRate);
            Point presentationVector = new Point(end.X * _actualWidthRate, end.Y * _actualHeightRate);
            Point presentationEndPoint = new Point(presentationStartPoint.X + presentationVector.X,
                presentationStartPoint.Y + presentationVector.Y);

            const int key = (int)eMarks.GoalKeeper;

            _dispatcher.Invoke(new ThreadStart(delegate
            {
                int deltaX = 0;
                for (int i = 0; i < 4; i++)
                {
                    int x = _rods[(eMarks)(i + eMarks.GoalKeeper)];
                    (_markups[key+i] as Shape).StrokeThickness = thickness;
                    (_markups[key + i] as Shape).Stroke = (color == null) ? Brushes.Chocolate : color;

                    (_markups[key + i] as Line).X1 = presentationStartPoint.X + x;
                    (_markups[key + i] as Line).Y1 = presentationStartPoint.Y;
                    (_markups[key + i] as Line).X2 = presentationEndPoint.X + x;
                    (_markups[key + i] as Line).Y2 = presentationEndPoint.Y;

                    Canvas.SetLeft(_markups[key + i], 0);
                    Canvas.SetTop(_markups[key + i], 0);
                    deltaX += 200;
                }         
            }));
        }

        public static void DrawBall(Point center, int radius,
            bool drawBall = false, SolidColorBrush circleColor = null)
        {
            const int key = (int)eMarks.BallMark;
            Point presentationCenter = new Point(center.X * _actualWidthRate, center.Y * _actualHeightRate);
            int presentationRadius = Convert.ToInt32(radius * ((_actualWidthRate + _actualHeightRate) / 2));

            _dispatcher.Invoke(new ThreadStart(delegate
            {
                if (drawBall) (_markups[key] as Shape).Fill = Brushes.White;
                (_markups[key] as Shape).StrokeThickness = 2;
                (_markups[key] as Shape).Stroke = (circleColor == null) ? Brushes.Red : circleColor;

                _markups[key].Width = presentationRadius * 2;
                _markups[key].Height = presentationRadius * 2;

                Canvas.SetLeft(_markups[key], presentationCenter.X - presentationRadius);
                Canvas.SetTop(_markups[key], presentationCenter.Y - presentationRadius);
            }));
        }

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
