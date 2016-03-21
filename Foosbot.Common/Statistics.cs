using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
