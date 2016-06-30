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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Responsible for canvas management.
    /// Must be called from a GUI thread in order to draw on canvas or change elements.
    /// Contains all elements drawn on canvas, those can be accessed by mark type.
    /// Converts provided frame coordinates to screen canvas coordinates.
    /// </summary>
    internal class Screen : ScreenUtilities
    {
        #region private members

        /// <summary>
        /// Canvas to draw on
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// Screen Width in UI
        /// </summary>
        private int _screenWidth;

        /// <summary>
        /// Screen Heigth in UI
        /// </summary>
        private int _screenHeight;

        /// <summary>
        /// Frame Width provided by camera
        /// </summary>
        private int _frameWidth;

        /// <summary>
        /// Frame Height provided by camera
        /// </summary>
        private int _frameHeight;

        /// <summary>
        /// Screen Width / Frame Width
        /// </summary>
        private float _xRate = 0;

        /// <summary>
        /// Screen Height / Frame Height
        /// </summary>
        private float _yRate = 0;

        /// <summary>
        /// Markup shape dictionary
        /// Contains markup key and related shape
        /// </summary>
        private Dictionary<eMarks, FrameworkElement> _elements = new Dictionary<eMarks, FrameworkElement>();

        #endregion private members

        /// <summary>
        /// Screen Width / Frame Width
        /// </summary>
        private float XRate
        {
            get
            {
                if (_xRate == 0)
                {
                    _xRate = Convert.ToSingle(_screenWidth) / Convert.ToSingle(_frameWidth);
                }
                return (float)_xRate;
            }
        }

        /// <summary>
        /// Screen Height / Frame Height
        /// </summary>
        private float YRate
        {
            get
            {
                if (_yRate == 0)
                {
                    _yRate = Convert.ToSingle(_screenHeight) / Convert.ToSingle(_frameHeight);
                }
                return (float)_xRate;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dispatcher">Dispather to get UI thread</param>
        /// <param name="canvas">Canvas to draw on</param>
        /// <param name="frameWidth">Foosbot world X axe length</param>
        /// <param name="frameHeigth">Foosbot world Y axe length</param>
        public Screen(Canvas canvas, int frameWidth, int frameHeigth)
        {
            _screenWidth = Convert.ToInt32(canvas.Width);
            _screenHeight = Convert.ToInt32(canvas.Height);
            _frameWidth = frameWidth;
            _frameHeight = frameHeigth;
            _canvas = canvas;
        }

        /// <summary>
        /// Add element to canvas
        /// </summary>
        /// <param name="mark">Mark name</param>
        /// <param name="element">Element to be added</param>
        public void AddElement(eMarks mark, FrameworkElement element)
        {
            _elements.Add(mark, element);
            _canvas.Children.Add(element);
        }

        /// <summary>
        /// Get instance of element on canvas by mark
        /// Use in order to set/change element properties
        /// </summary>
        /// <typeparam name="T">Type of Framework Element to get</typeparam>
        /// <param name="mark">Mark of an element</param>
        /// <returns>Framework Element of class T or null in case of wrong type</returns>
        public T Element<T>(eMarks mark) where T : FrameworkElement
        {
            return (_elements[mark] as T);
        }

        /// <summary>
        /// Draw element on Canvas
        /// </summary>
        /// <param name="mark">Element to be drawn on canvas</param>
        /// <param name="x">X coorindate in frame</param>
        /// <param name="y">Y coorindate in frame</param>
        public void Draw(eMarks mark, int x, int y, eCoordinatesType type = eCoordinatesType.Frame, int zIndex = Int32.MinValue)
        {
            VerifyCoordinatesTypeSupported(MethodBase.GetCurrentMethod(), type, eCoordinatesType.Screen, eCoordinatesType.Frame);

            if (type == eCoordinatesType.Frame)
                FrameToScreenCoordinates(ref x, ref y);
            Canvas.SetLeft(_elements[mark], x);
            Canvas.SetTop(_elements[mark], y);

            if (zIndex != Int32.MinValue)
                Canvas.SetZIndex(_elements[mark], zIndex);
        }

        /// <summary>
        /// Draw element on Canvas
        /// </summary>
        /// <param name="mark">Element to be drawn on canvas</param>
        /// <param name="x">X coorindate in frame</param>
        /// <param name="y">Y coorindate in frame</param>
        public void Draw(eMarks mark, double x, double y, eCoordinatesType type = eCoordinatesType.Frame, int zIndex = Int32.MinValue)
        {
            Draw(mark, Convert.ToInt32(x), Convert.ToInt32(y), type, zIndex);
        }

        /// <summary>
        /// Convert Frame Coordinates to Screen
        /// </summary>
        /// <param name="x">X of point in Frame Dimensions [by ref]</param>
        /// <param name="y">Y of point in Frame Dimensions [by ref]</param>
        public void FrameToScreenCoordinates(ref int x, ref int y)
        {
            x = Convert.ToInt32(x * XRate);
            y = Convert.ToInt32(y * YRate);
        }

        /// <summary>
        /// Convert Frame Coordinates to Screen
        /// </summary>
        /// <param name="point">Point in Frame Dimensions</param>
        /// <returns>Point in Screen Dimensions</returns>
        public Point FrameToScreenCoordinates(Point point)
        {
            return new Point(point.X * XRate, point.Y * YRate);
        }

        /// <summary>
        /// Convert distance in frame to distance on screen
        /// </summary>
        /// <param name="distanceInFrame">Distance beetween points in frame</param>
        /// <returns>Distance beetween points on the screen</returns>
        public float FrameToScreenDistance(float distanceInFrame)
        {
            return (YRate + XRate)/2 * distanceInFrame;
        }
    }
}
