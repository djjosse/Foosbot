// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Protocols;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Foosbot
{
    public static class Statistics
    {

        private static Dispatcher _dispatcher;

        private static Dictionary<eStatisticsKey, Label> _elements;
        public static void Initialize(Dispatcher dispatcher, Dictionary<eStatisticsKey, Label> elements)
        {
            _dispatcher = dispatcher;
            _elements = elements;
        }

        public static void UpdateFrameInfo(string info)
        {
            _dispatcher.Invoke(new ThreadStart(delegate
            {
                _elements[eStatisticsKey.FrameInfo].Content = info;
            }));
        }

        public static void UpdateProccessInfo(string info)
        {
            _dispatcher.Invoke(new ThreadStart(delegate
            {
                _elements[eStatisticsKey.ProccessInfo].Content = info;
            }));
        }

        public static void UpdateBasicImageProcessingInfo(string info)
        {
            _dispatcher.Invoke(new ThreadStart(delegate
            {
                _elements[eStatisticsKey.BasicImageProcessingInfo].Content = info;
            }));
        }

        public static void UpdateBallCoordinates(string info)
        {
            _dispatcher.Invoke(new ThreadStart(delegate
            {
                _elements[eStatisticsKey.BallCoordinates].Content = info;
            }));
        }

    }
}
