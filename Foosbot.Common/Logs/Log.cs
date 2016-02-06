using Foosbot.Common.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Foosbot
{
    /// <summary>
    /// Log singleton Class.
    /// Can be accessed from any part of program.
    /// </summary>
    /// <author>Joseph Gleyzer</author>
    /// <date>04.02.2016</date>
    public sealed class Log : BackgroundFlow
    {
        #region Common Log

        /// <summary>
        /// Common Log Synchronization Token
        /// </summary>
        private static object _commonToken = new Object();

        /// <summary>
        /// Common Log Singleton Instance
        /// </summary>
        private static Log _common;

        /// <summary>
        /// Common Log Property of Singleton
        /// </summary>
        public static Log Common
        {
            get
            {
                if (_common == null)
                {
                    lock (_commonToken)
                    {
                        if (_common == null)
                            _common = new Log(eLogType.Common);
                    }
                }
                return _common;
            }
        }

        #endregion Common Log

        #region Image Processing

        /// <summary>
        /// Image Processing Log Synchronization Token
        /// </summary>
        private static object _imageToken = new Object();

        /// <summary>
        /// Image Processing Log Singleton Instance
        /// </summary>
        private static Log _image;

        /// <summary>
        /// Image Processing Log Property of Singleton
        /// </summary>
        public static Log Image
        {
            get
            {
                if (_image == null)
                {
                    lock (_imageToken)
                    {
                        if (_image == null)
                            _image = new Log(eLogType.Image);
                    }
                }
                return _image;
            }
        }

        #endregion Image Processing

        /// <summary>
        /// Log Message Queue
        /// </summary>
        private Queue<LogMessage> _messageQ;

        /// <summary>
        /// Log Queue Token
        /// </summary>
        private object _token = new Object();

        /// <summary>
        /// Current Log Type
        /// </summary>
        private eLogType _type;

        /// <summary>
        /// Private Log Singleton Constructor
        /// </summary>
        private Log() { }

        /// <summary>
        /// Private Log Singleton Constructor - in use
        /// </summary>
        /// <param name="type">Type of log to instantiate and start</param>
        private Log(eLogType type)
        {
            _type = type;
            _messageQ = new Queue<LogMessage>();
            Start();
        }

        /// <summary>
        /// Execution of logging background thread
        /// </summary>
        public override void Flow()
        {
            while (true)
            {
                if (_messageQ.Count != 0)
                {
                    lock (_token)
                    {
                        try
                        {
                            LogMessage message = _messageQ.Dequeue();
                            using (StreamWriter file = File.AppendText(String.Format("{0}.log", _type.ToString())))
                            {
                                file.WriteLine("{0}\t{1}\t{2}", message.TimeStamp.ToString("HH:mm:ss.ffff"),
                                    message.Category, message.Description);
                            }
                        }
                        catch(Exception)
                        {
                            //It is only log, we don't want to fail the programm
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Print Log Method
        /// </summary>
        /// <param name="message">Message to print</param>
        /// <param name="category">Category of message</param>
        /// <param name="timeStamp">Message timestamp</param>
        private void Print(string message, eLogCategory category, DateTime timeStamp)
        {
            LogMessage m = new LogMessage(message, category, timeStamp);
            lock (_token)
            {
                _messageQ.Enqueue(m);
            }
        }
        
        /// <summary>
        /// Debug Print
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Debug(string message)
        {
            Print(message, eLogCategory.Debug, DateTime.Now);
        }

        /// <summary>
        /// Info Print
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Info(string message)
        {
            Print(message, eLogCategory.Info, DateTime.Now);
        }

        /// <summary>
        /// Warning Print
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Warning(string message)
        {
            Print(message, eLogCategory.Warning, DateTime.Now);
        }

        /// <summary>
        /// Error Print
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Error(string message)
        {
            Print(message, eLogCategory.Error, DateTime.Now);
        }
    }

}