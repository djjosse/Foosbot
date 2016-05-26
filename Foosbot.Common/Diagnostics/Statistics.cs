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
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Foosbot
{
    /// <summary>
    /// Statistics Static Class
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// Dispatcher to run UI changes in relevant thread
        /// </summary>
        private static Dispatcher _dispatcher;

        /// <summary>
        /// Elements Dictionary: Statistics key and Label to update in UI
        /// </summary>
        private static Dictionary<eStatisticsKey, Label> _elements;

        /// <summary>
        /// Initialization Method
        /// </summary>
        /// <param name="dispatcher">Dispatcher to run UI changes in relevant thread</param>
        /// <param name="elements">Labels to update by key</param>
        public static void Initialize(Dispatcher dispatcher, Dictionary<eStatisticsKey, Label> elements)
        {
            _dispatcher = dispatcher;
            _elements = elements;
        }

        /// <summary>
        /// Update Frame info
        /// </summary>
        /// <param name="info">New content</param>
        public static void TryUpdateFrameInfo(string info)
        {
            try
            {
                _dispatcher.Invoke(new ThreadStart(delegate
                {
                    _elements[eStatisticsKey.FrameInfo].Content = info;
                }));
            }
            catch
            {
                Log.Common.Debug(String.Format("Unable to update: [{0}]",
                    MethodBase.GetCurrentMethod().Name));
            }
        }

        /// <summary>
        /// Update Proccess info
        /// </summary>
        /// <param name="info">New content</param>
        public static void TryUpdateProccessInfo(string info)
        {
            try
            {
                _dispatcher.Invoke(new ThreadStart(delegate
                {
                    _elements[eStatisticsKey.ProccessInfo].Content = info;
                }));
            }
            catch
            {
                Log.Common.Debug(String.Format("Unable to update: [{0}]",
                    MethodBase.GetCurrentMethod().Name));
            }
        }

        /// <summary>
        /// Update Basic Image Processing info
        /// </summary>
        /// <param name="info">New content</param>
        public static void TryUpdateBasicImageProcessingInfo(string info)
        {
            try
            {
                _dispatcher.Invoke(new ThreadStart(delegate
                {
                    _elements[eStatisticsKey.BasicImageProcessingInfo].Content = info;
                }));
            }
            catch
            {
                Log.Common.Debug(String.Format("Unable to update: [{0}]",
                    MethodBase.GetCurrentMethod().Name));
            }
        }

        /// <summary>
        /// Update Ball Coordinates info
        /// </summary>
        /// <param name="info">New content</param>
        public static void TryUpdateBallCoordinates(string info)
        {
            try
            {
                _dispatcher.Invoke(new ThreadStart(delegate
                {
                    _elements[eStatisticsKey.BallCoordinates].Content = info;
                }));
            }
            catch
            {
                Log.Common.Debug(String.Format("Unable to update: [{0}]",
                    MethodBase.GetCurrentMethod().Name));
            }
        }
    }
}
