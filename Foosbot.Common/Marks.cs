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
                }
            }

            foreach (FrameworkElement element in _markups.Values)
                _canvas.Children.Add(element);
        }

        public static void DrawRicochetMark(int x, int y, SolidColorBrush circleColor = null)
        {
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

        public static void DrawBallVector(Point center, Point vector, SolidColorBrush color = null)
        {
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
