// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Base class for drawing on screen manager class
    /// </summary>
    internal abstract class DrawerBase : ScreenUtilities
    {
        /// <summary>
        /// Dispatcher to perform operations in UI thread
        /// </summary>
        protected Dispatcher _dispatcher;

        /// <summary>
        /// Screen object to draw on
        /// </summary>
        protected Screen _screen;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dispatcher">Dispatcher to perform operations in UI thread</param>
        /// <param name="canvas">Canvas to draw elements on</param>
        /// <param name="frameWidth">Frame Width</param>
        /// <param name="frameHeigth">Frame Height</param>
        public DrawerBase(Dispatcher dispatcher, Canvas canvas, int frameWidth, int frameHeigth)
        {
            _dispatcher = dispatcher;
            _screen = new Screen(canvas, frameWidth, frameHeigth);
        }

        /// <summary>
        /// Initialization of drawer - adds all marks to canvas
        /// </summary>
        public void Initialize()
        {
            _dispatcher.Invoke(() =>
            {
                foreach (var pair in ShapesFactory.Create())
                {
                    _screen.AddElement(pair.Key, pair.Value);
                }
            });
        }

        /// <summary>
        /// Convert Point from real world coordinate to screen coordinate
        /// Real World => Device (Foosbot World) => Frame Dimensions => Screen Dimensions
        /// </summary>
        /// <param name="realWorldPoint">Point in coordinates of Real World</param>
        /// <returns>Point in coordinates of Screen</returns>
        protected Point ConvertPointFromRealWorldToScreen(Point realWorldPoint)
        {
            Point devicePoint = Data.TableToDeviceCoordinates(realWorldPoint);
            Point framePoint = TransformAgent.Data.InvertTransform(devicePoint);
            Point screenPoint = _screen.FrameToScreenCoordinates(framePoint);
            return screenPoint;
        }
    }
}
