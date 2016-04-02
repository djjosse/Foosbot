//// **************************************************************************************
//// **																				   **
//// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
//// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
//// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
//// **		The information and source code here belongs to Foosbot project			   **
//// **		and may not be reproduced or used without authors explicit permission.	   **
//// **																				   **
//// **************************************************************************************

//using Foosbot.Common.Multithreading;
//using Foosbot.Common.Protocols;

//namespace Foosbot.ImageProcessing
//{
//    /// <summary>
//    /// Publish Latest Ball Coordinates and notifies observers on demand
//    /// </summary>
//    public class BallLocationPublisher: Publisher<BallCoordinates>
//    {
//        /// <summary>
//        /// To get latest
//        /// </summary>
//        private ILastBallCoordinatesUpdater _coordinatesUpdater;

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="coordinatesUpdater">Updater to get coordinates from</param>
//        public BallLocationPublisher(ILastBallCoordinatesUpdater coordinatesUpdater)
//        {
//            _coordinatesUpdater = coordinatesUpdater;
//        }

//        /// <summary>
//        /// Gets the latest coordinates from coordinates updater passed in constructor
//        /// and notifies all attached observers
//        /// </summary>
//        public void UpdateAndNotify()
//        {
//            if (_coordinatesUpdater.LastBallCoordinates != null)
//            {
//                Data = _coordinatesUpdater.LastBallCoordinates;
//                NotifyAll();
//            }
//        }
//    }
//}
