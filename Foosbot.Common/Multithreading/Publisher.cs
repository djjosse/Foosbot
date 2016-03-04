using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Multithreading
{
    /// <summary>
    /// Publisher Abstract Class
    /// Observers call Attached/Detach to receive updates
    /// NotifyAll() called in derived class to update the 
    /// observers with new data stored in Data property
    /// </summary>
    /// <typeparam name="T">Type of data to be published</typeparam>
    public abstract class Publisher<T>
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
        private List<Observer<T>> _observerList = new List<Observer<T>>();

        private object _observerListModificationToken = new object();

        /// <summary>
        /// Attach new observer
        /// </summary>
        /// <param name="observer"></param>
        public void Attach(Observer<T> observer)
        {
            lock (_observerListModificationToken)
            {
                _observerList.Add(observer);
            }
        }

        /// <summary>
        /// Dettach existing observer
        /// </summary>
        /// <param name="observer"></param>
        public void Dettach(Observer<T> observer)
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
    }
}
