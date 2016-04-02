// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Image Processing Unit Class
    /// </summary>
    public class ImageProcessingUnit : AbstractImageProcessingUnit
    {        
        #region private members 

        /// <summary>
        /// Ball Tracker Instance
        /// </summary>
        private Tracker _ballTracker;

        /// <summary>
        /// Ball Calibration Unit Instance
        /// </summary>
        private CalibrationUnit _calibrator;

        /// <summary>
        /// Represents a flag for calibration
        /// </summary>
        private bool _isCallibrated;

        /// <summary>
        /// Last Received Frame Time stamp
        /// </summary>
        private DateTime _lastFrameTimeStamp;

        /// <summary>
        /// Detection Analyzer to calculate statistic and diagnostic info
        /// </summary>
        private DetectionStatisticAnalyzer _detectionAnalyzer;

        #endregion private members

        /// <summary>
        /// Image Processing Unit Constructor
        /// </summary>
        /// <param name="streamer">Video streamer to get frames from</param>
        public ImageProcessingUnit(Publisher<Frame> streamer) : base(streamer)
        {
            _calibrator = new CalibrationUnit();
            _ballTracker = new Tracker(_calibrator, _publisher);
            LastBallLocationPublisher = new BallLocationPublisher(_ballTracker);
            _lastFrameTimeStamp = DateTime.Now;
            _detectionAnalyzer = new DetectionStatisticAnalyzer(/*onUpdateStatistics*/);
        }

        /// <summary>
        /// Main Image Processing Unit Flow
        /// </summary>
        public override void Job()
        {
            _publisher.Dettach(this);
            Image<Gray, byte> image = _publisher.Data.Image;
            DateTime timestamp = _publisher.Data.Timestamp;
            if (timestamp != _lastFrameTimeStamp)
            {
                _lastFrameTimeStamp = timestamp;
                if (_isCallibrated)
                {
                    //Start statisctics and stopwatch
                    _detectionAnalyzer.Next();

                    //Call Tracker & Set new data to be taken by observers
                    _ballTracker.InvokeTracking(image.Clone(), timestamp);

                    //Stop statics and stopwatch
                    _detectionAnalyzer.Finalize(_ballTracker.IsBallLocationFound);

                    //Notify Observers
                    LastBallLocationPublisher.UpdateAndNotify();
                }
                else
                {
                    //Call Callibrator
                    _isCallibrated = _calibrator.InvokeCallibration(image);
                }
            }
            else
            {
                Log.Image.Error("We received same image twice. Good to check timestamp.");
            }
            _publisher.Attach(this);
        }
    }
}
