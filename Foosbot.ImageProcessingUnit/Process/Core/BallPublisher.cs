// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using EasyLog;
using Foosbot.Common.Logs;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using System;
using System.Reflection;

namespace Foosbot.ImageProcessingUnit.Process.Core
{
    /// <summary>
    /// Publisher of ball coordinates
    /// </summary>
    public class BallPublisher : Publisher<BallCoordinates>
    {
        /// <summary>
        /// Image Data instance to get last known ball coordinates
        /// </summary>
        private IImageData _imageData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinatesUpdater">Updater to get coordinates from</param>
        public BallPublisher(IImageData imageData)
        {
            _imageData = imageData;
        }

        /// <summary>
        /// Gets the latest coordinates from coordinates updater passed in constructor
        /// and notifies all attached observers
        /// </summary>
        public void UpdateAndNotify()
        {
            if (_imageData.BallCoords != null)
            {
                Data = _imageData.BallCoords;
                NotifyAll();
            }
            else
            {
                Log.Print("Coordinates were not set by IP-Unit", eCategory.Error, LogTag.IMAGE);
            }
        }
    }
}
