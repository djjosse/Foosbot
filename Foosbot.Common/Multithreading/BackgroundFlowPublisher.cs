using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
