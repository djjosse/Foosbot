﻿// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using EasyLog;
using Foosbot.Common.Contracts;
using Foosbot.Common.Logs;
using System;
using System.Threading;

namespace Foosbot
{
    /// <summary>
    /// Backgound Flow abstract class
    /// Runs the flow in Thread
    /// </summary>
    public abstract class BackgroundFlow : IFlow
    {
        /// <summary>
        /// Running Thread
        /// </summary>
        protected Thread _thread;

        /// <summary>
        /// Function that will run in Separate Thread
        /// </summary>
        public abstract void Flow();

         /// <summary>
        /// Run the flow in Thread
        /// </summary>
        public virtual void Start()
        {
            try
            {
                _thread = new Thread(() => { Flow(); });
                _thread.IsBackground = true;
                _thread.Start();
            }
            catch(Exception ex)
            {
                Log.Print("Unable to start thread. ", ex, LogTag.COMMON, eCategory.Error, true);
                Thread.Sleep(1000);
            }
        }
    }
}
