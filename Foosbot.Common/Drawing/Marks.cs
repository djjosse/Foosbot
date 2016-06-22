// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Drawing;
using Foosbot.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Foosbot
{
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
        private static Surface _instance;

        /// <summary>
        /// Singleton creation token for multi threading
        /// </summary>
        private static object _token = new object();

        /// <summary>
        /// Singleton Picture instance property
        /// </summary>
        private static Surface Instance 
        {
            get
            {
                if (_instance==null)
                {
                    lock(_token)
                    {
                        if (_instance == null)
                        {
                            _instance = new Surface();
                        }
                    }
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
        /// <param name="actualWidthRate">Canvas width</param>
        /// <param name="actualHeightRate">Canvas height</param>
        public static void Initialize(Dispatcher dispatcher, Canvas canvas, double actualWidthRate, double actualHeightRate)
        {
            Instance.Initialize(dispatcher, canvas, actualWidthRate, actualHeightRate);
        }

        /// <summary>
        /// Get current first player position of selected rod
        /// </summary>
        /// <param name="type">Rod Type</param>
        /// <returns>Selected rod first player position point</returns>
        public static Point PlayerPosition(eRod type)
        {
            return Instance.PlayerPosition(type);
        }

        /// <summary>
        /// Drawing the ball on the canvas
        /// </summary>
        /// <param name="center">Ball circle center</param>
        /// <param name="radius">Ball radius</param>
        /// <param name="circleColor">Ball color : optional</param>
        public static void DrawBall(Point center, int radius, SolidColorBrush circleColor = null)
        {
            Instance.DrawBall(center, radius, circleColor);
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
            Instance.DrawCallibrationCircle(mark, center, radius, circleColor, textColor, fontSize, text);
        }

        /// <summary>
        /// Drawing the vector of the ball
        /// </summary>
        /// <param name="center">Center point to draw vector from</param>
        /// <param name="vector">The end of the vector</param>
        /// <param name="isLocation">Optional is location [default : true]</param>
        /// <param name="color">Optional color [default : Aqua]</param>
        public static void DrawBallVector(Point center, Point vector, bool isLocation = true, SolidColorBrush color = null)
        {
            Instance.DrawBallVector(center, vector, isLocation, color);
        }

        /// <summary>
        /// Draw the ricochet point of the ball on the border of the table 
        /// </summary>
        /// <param name="x">X coordinate of the ricochet on the canvas</param>
        /// <param name="y">Y coordinate of the ricochet on the canvas</param>
        /// <param name="isLocation">Bool that convert between location and coordinate : optional</param>
        /// <param name="circleColor">The color of the stroke of the ricochet mark : optional</param>
        public static void DrawRicochetMark(int x, int y, bool isLocation = false, SolidColorBrush circleColor = null)
        {
            Instance.DrawRicochetMark(x, y, isLocation, circleColor);
        }

        /// <summary>
        /// Show table borders calculated by IP Unit
        /// </summary>
        /// <param name="corners"></param>
        public static void DrawTableBorders(Dictionary<eCallibrationMark, Emgu.CV.Structure.CircleF> marks)
        {
            Instance.DrawTableBorders(marks);
        }

        /// <summary>
        /// Draw rod lines on the canvas
        /// </summary>
        /// <param name="thickness">Optional thickness of the rods [default : 6]</param>
        /// <param name="isLocation">Optional isLocation [default : true]</param>
        public static void DrawRods(int thickness = 6, bool isLocation = true)
        {
            Instance.DrawRods(thickness, isLocation);
        }

        /// <summary>
        /// Draw the given rod dynamic sector
        /// </summary>
        /// <param name="rod">The rod for sector calculation</param>
        /// <param name="dynamicSectorWidth">The dynamic sector of the rod</param>
        /// <param name="thickness">Optional thickness of the sector line [default : 2]</param>
        /// <param name="isLocation">Optional isLocation [default : true]</param>
        public static void DrawSector(eRod rod, int dynamicSectorWidth, int thickness = 2, bool isLocation = true)
        {
            Instance.DrawSector(rod, dynamicSectorWidth, thickness, isLocation);
        }

        /// <summary>
        /// Draw a rod by a given eMark sign , moving the rod on the linear and rotational axes
        /// </summary>
        /// <param name="rod">eMark of the wanted rod : GoalKeeper, Defense , Midfield, Attack</param>
        /// <param name="deltaYMovment">The change on the linear movement</param>
        /// <param name="rotationalMove">eRotationalMove of the rod : DEFENCE, RISE, ATTACK</param>
        public static void DrawRodPlayers(eRod rod, int linearMoveDestination, eRotationalMove rotationalMove, bool isLocation = true)
        {
            Instance.DrawRodPlayers(rod, linearMoveDestination, rotationalMove, isLocation);
        }
    }
}
