// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using System;
using System.Threading;

namespace Foosbot.Common.Multithreading
{
    /// <summary>
    /// Observer Abstract Class
    /// Will be notified on each update in Publisher class
    /// and Job will be performed.
    /// To trigger the Job - use Start() method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Observer<T> : BackgroundFlow
    {
        /// <summary>
        /// Sleep After Job performed or _currentData is null
        /// </summary>
        public static readonly TimeSpan SLEEP_AFTER_JOB = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Publisher that updates this observer
        /// </summary>
        protected Publisher<T> _publisher;

        /// <summary>
        /// Current Data - last data received from publisher
        /// </summary>
        protected T _currentData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publisher">Publisher to get attached to.</param>
        protected Observer(Publisher<T> publisher)
        {
            _publisher = publisher;
            _publisher.Attach(this);
            _currentData = default(T);
        }

        /// <summary>
        /// Update to be called by publisher
        /// </summary>
        public void Update()
        {
            _thread.Interrupt();
        }

        /// <summary>
        /// Get Data from Publisher on Update and set to currentData
        /// </summary>
        private void OnUpdate()
        {
            _currentData = _publisher.Data;
        }

        /// <summary>
        /// Tasks to perfrom before go to sleep
        /// Update will be received while sleeping and sleep will be 
        /// enterrupted - Job() will be triggered again
        /// If _currentData is null Job will not be performed and thread will go to sleep
        /// </summary>
        public abstract void Job();

        /// <summary>
        /// Flow to be performed on Start() method called
        /// </summary>
        public override void Flow()
        {
            while(true)
            {
                try
                {
                    if (_currentData != null && !_currentData.Equals(default(T)))
                    {
                        Job();
                    }
                    Thread.Sleep(SLEEP_AFTER_JOB);
                }
                catch(ThreadInterruptedException)
                {
                    this.OnUpdate();
                }
            }
        }
    }
}
