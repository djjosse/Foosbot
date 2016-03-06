using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.Common;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Foosbot.ImageProcessing
{
    public class ImageProcessingUnit : Observer<Frame>
    {
        private Helpers.UpdateMarkupDelegate UpdateMarkup;
        private Helpers.UpdateStatisticsDelegate UpdateStatistics;

        public Tracker BallTracker;

        public CalibrationUnit Calibrator;

        public BallLocationPublisher BallLocationPublisher { get; private set; }

        /// <summary>
        /// Represents a flag for callibration
        /// </summary>
        private bool _isCallibrated;

        public ImageProcessingUnit(Streamer streamer,
            Helpers.UpdateMarkupDelegate onUpdateMarkup, Helpers.UpdateStatisticsDelegate onUpdateStatistics) :
            base(streamer)
        {
            _publisher = streamer;
            UpdateMarkup = onUpdateMarkup;
            UpdateStatistics = onUpdateStatistics;

            Calibrator = new CalibrationUnit(UpdateMarkup, UpdateStatistics);
            BallTracker = new Tracker(Calibrator, _publisher, UpdateMarkup, UpdateStatistics);
            BallLocationPublisher = new BallLocationPublisher(BallTracker);

            _lastFrameTimeStamp = DateTime.Now;
        }

        DateTime _lastFrameTimeStamp;

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
                    //Call Tracker & Set new data to be taken by observers
                    BallTracker.InvokeTracking(image.Clone(), timestamp);
                    //Notify Observers
                    BallLocationPublisher.UpdateAndNotify();
                }
                else
                {
                    //Call Callibrator
                    _isCallibrated = Calibrator.InvokeCallibration(image);
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
