// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using System.Collections.Generic;
using System.Diagnostics;

namespace Foosbot.Common.Multithreading
{
    /// <summary>
    /// Publisher Abstract Class
    /// Observers call Attached/Detach to receive updates
    /// NotifyAll() called in derived class to update the 
    /// observers with new data stored in Data property
    /// </summary>
    /// <typeparam name="T">Type of data to be published</typeparam>
    public abstract class Publisher<T> : IPublisher<T>
    {
        /// <summary>
        /// Token for Stored data
        /// </summary>
        private object _dataToken = new object();

        /// <summary>
        /// Stored Data Private Member
        /// </summary>
        private T _data;

        /// <summary>
        /// Stored Data property to take on update
        /// </summary>
        public T Data
        {
            get
            {
                lock (_dataToken)
                {
                    return _data;
                }
            }
            protected set
            {
                lock (_dataToken)
                {
                    _data = value;
                }
            }
        }

        /// <summary>
        /// Observers list to update
        /// </summary>
        private List<IWorkingObserver<T>> _observerList = new List<IWorkingObserver<T>>();

        private object _observerListModificationToken = new object();

        /// <summary>
        /// Attach new observer
        /// </summary>
        /// <param name="observer"></param>
        public void Attach(IWorkingObserver<T> observer)
        {
            lock (_observerListModificationToken)
            {
                _observerList.Add(observer);
            }
        }

        /// <summary>
        /// Detach existing observer
        /// </summary>
        /// <param name="observer"></param>
        public void Detach(IWorkingObserver<T> observer)
        {
            lock (_observerListModificationToken)
            {
                _observerList.Remove(observer);
            }
        }

        /// <summary>
        /// Notify all exisiting observers
        /// </summary>
        protected void NotifyAll()
        {
            lock (_observerListModificationToken)
            {
                foreach (Observer<T> observer in _observerList)
                {
                    observer.Update();
                }
            }
        }

        /// <summary>
        /// Notify provided observer
        /// </summary>
        /// <param name="desiredObserver">Observer to notify</param>
        protected void Notify(Observer<T> desiredObserver)
        {
            lock (_observerListModificationToken)
            {
                foreach (Observer<T> observer in _observerList)
                {
                    if (observer == desiredObserver)
                        observer.Update();
                }
            }
        }
    }
}
