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
using System;

namespace Foosbot.Common.Data
{
    /// <summary>
    /// Transformation Storage Singleton Instance
    /// </summary>
    public class TransformAgent
    {
        /// <summary>
        /// Transformation Agent Singleton Instance
        /// </summary>
        private static volatile TransformAgent _instance;

        /// <summary>
        /// Synchronization Token
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Transformation Data Instance
        /// </summary>
        private ITransformation _data;

        /// <summary>
        /// Private Constructor
        /// </summary>
        private TransformAgent()
        {
            _data = new TransformData();
        }

        /// <summary>
        /// Transformation Data Static Property
        /// </summary>
        public static ITransformation Data
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new TransformAgent();
                    }
                }
                return _instance._data;
            }
        }
    }
}
