// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using System.Threading;

namespace Foosbot.Common.Multithreading
{
    public abstract class BackgroundFlowPublisher<T> : Publisher<T>
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
        public void Start()
        {
            _thread = new Thread(() => { Flow(); });
            _thread.IsBackground = true;
            _thread.Start();
        }
    }
}
