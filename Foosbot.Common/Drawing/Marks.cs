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
using Foosbot.Common.Drawing;
using Foosbot.Common.Enums;
using Foosbot.Common.Exceptions;
using Foosbot.Common.Logs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Foosbot
{
    /// <summary>
    /// Singleton used to draw data on canvas to present current system state in GUI
    /// </summary>
    public sealed class Marks
    {
        #region private members

        /// <summary>
        /// Singleton private constructor
        /// </summary>
        private Marks() { }

        /// <summary>
        /// Singleton Picture instance
        /// </summary>
        private static Drawer _instance;

        /// <summary>
        /// Singleton Picture instance property
        /// </summary>
        private static Drawer Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InitializationException("Mark must be initialized before it can be used!");
                }
                return _instance;
            }
        }

        #endregion private members

        /// <summary>
        /// Initialization method for drawing marks on canvas
        /// </summary>
        /// <param name="dispatcher">Dispatcher of drawing window</param>
        /// <param name="canvas">Canvas to draw marks on it</param>
        /// <param name="actualWidth">Frame width</param>
        /// <param name="actualHeight">Frame height</param>
        public static void Initialize(Dispatcher dispatcher, Canvas canvas, int frameWidth, int frameHeigth)
        {
            _instance = new Drawer(dispatcher, canvas, frameWidth, frameHeigth);
            _instance.Initialize();
        }

        /// <summary>
        /// Get current first player position of selected rod
        /// In Frame Dimensions - used for Demo
        /// </summary>
        /// <param name="type">Rod Type</param>
        /// <returns>Selected rod first player position point (In Frame Dimensions)</returns>
        public static Point PlayerPosition(eRod type)
        {
            return Instance.PlayerPosition(type);
        }

        /// <summary>
        /// Drawing the ball on the canvas
        /// </summary>
        /// <param name="center">Ball circle center (Frame Dimensions)</param>
        /// <param name="radius">Ball radius (Frame Dimensions)</param>
        /// <param name="circleColor">Ball color : optional</param>
        public static void DrawBall(Point center, int radius, SolidColorBrush circleColor = null)
        {
            try
            {
                Instance.DrawBall(center, radius, circleColor);
            }
            catch (Exception ex)
            {
                Log.Print("Unable to draw ball mark.", ex, LogTag.COMMON);
            }
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
        public static void DrawCallibrationCircle(eCallibrationMark mark, Point center, int radius,
            SolidColorBrush circleColor = null, SolidColorBrush textColor = null, double fontSize = 12,
                string text = "")
        {
            try
            {
                Instance.DrawCallibrationCircle(mark, center, radius, circleColor, textColor, fontSize, text);
            }
            catch (Exception ex)
            {
                Log.Print("Unable to draw calibration mark.", ex, LogTag.COMMON);
            }
        }

        /// <summary>
        /// Drawing the vector of the ball
        /// </summary>
        /// <param name="center">Center point to draw vector from</param>
        /// <param name="vector">The end of the vector</param>
        /// <param name="isLocation">Optional is location [default : true]</param>
        /// <param name="color">Optional color [default : Aqua]</param>
        public static void DrawBallVector(Point center, Point vector, eCoordinatesType type = eCoordinatesType.FoosbotWorld, SolidColorBrush color = null)
        {
            try
            {
                Instance.DrawBallVector(center, vector, type, color);
            }
            catch (Exception ex)
            {
                Log.Print("Unable to draw ball vector mark.", ex, LogTag.COMMON);
            }
        }

        /// <summary>
        /// Show table borders calculated by IP Unit
        /// </summary>
        /// <param name="marks">Calibration Mark Circles</param>
        public static void DrawTableBorders(Dictionary<eCallibrationMark, Emgu.CV.Structure.CircleF> marks)
        {
            try
            {
                Instance.DrawTableBorders(marks);
            }
            catch (Exception ex)
            {
                Log.Print("Unable to draw table borders.", ex, LogTag.COMMON);
            }
        }

        /// <summary>
        /// Draw rod lines on the canvas
        /// </summary>
        /// <param name="thickness">Optional thickness of the rods [default : 6]</param>
        public static void DrawRods(int thickness = 6)
        {
            try
            {
                Instance.DrawRodLines(thickness);
            }
            catch (Exception ex)
            {
                Log.Print("Unable to draw rods marks.", ex, LogTag.COMMON);
            }
        }

        /// <summary>
        /// Draw the given rod dynamic sector
        /// </summary>
        /// <param name="rod">The rod for sector calculation</param>
        /// <param name="dynamicSectorWidth">Width of dynamic sector of the rod in Real World (MM)</param>
        /// <param name="thickness">Optional thickness of the sector line [default : 2]</param>
        public static void DrawSector(eRod rod, int dynamicSectorWidth, int thickness = 2)
        {
            try
            {
                Instance.DrawSector(rod, dynamicSectorWidth, thickness);
            }
            catch (Exception ex)
            {
                Log.Print("Unable to draw rod dynamic sector marks.", ex, LogTag.COMMON);
            }
        }

        /// <summary>
        /// Draw a rod by a given eMark sign , moving the rod on the linear and rotational axes
        /// </summary>
        /// <param name="rod">eMark of the wanted rod : GoalKeeper, Defense , Midfield, Attack</param>
        /// <param name="linearMoveDestination">The change on the linear movement in Real World (MM)</param>
        /// <param name="rotationalMove">eRotationalMove of the rod : DEFENCE, RISE, ATTACK</param>
        public static void DrawRodPlayers(eRod rod, int linearMoveDestination, eRotationalMove rotationalMove)
        {
            try
            {
                Instance.DrawRodPlayers(rod, linearMoveDestination, rotationalMove);
            }
            catch (Exception ex)
            {
                Log.Print("Unable to draw rods players mark.", ex, LogTag.COMMON);
            }
        }
    }
}
