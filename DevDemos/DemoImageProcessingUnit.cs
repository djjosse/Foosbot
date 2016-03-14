using Foosbot;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevDemos
{
    public class DemoImageProcessingUnit : AbstractImageProcessingUnit
    {
        private DemoLastBallCoordinatesUpdater updater;

        public DemoImageProcessingUnit(Publisher<Frame> streamer,
            Helpers.UpdateMarkupCircleDelegate onUpdateMarkup, Helpers.UpdateStatisticsDelegate onUpdateStatistics) :
            base(streamer, onUpdateMarkup, onUpdateStatistics)
        {
            (streamer as DemoStreamer).DemoImageProcessingUnit = this;

            updater = new DemoLastBallCoordinatesUpdater();
            BallLocationPublisher = new BallLocationPublisher(updater);
        }

       
        public override void Job()
        {
            _publisher.Dettach(this);
            //UpdateStatistics(Helpers.eStatisticsKey.BasicImageProcessingInfo, 
            //    String.Format("New frame received, generate location: {0}", DateTime.Now.ToString("ss:ffff")));

            BallCoordinates coordinates = GenerateLocation();
            updater.LastBallCoordinates = coordinates;

            BallLocationPublisher.UpdateAndNotify();
            _publisher.Attach(this);
        }

        public BallCoordinates GenerateLocation()
        {
            Frame frame = _publisher.Data;
            int x = 500;
            int y = 500 + counter;
            counter += 10;

            Log.Common.Debug(String.Format("[{0}] Generating new location.", MethodBase.GetCurrentMethod().Name));

            //implement this...
            //TODO:...

            //try also undefined...
            //new BallCoordinates(frame.Timestamp);

            BallCoordinates coordinates = new BallCoordinates(x, y, frame.Timestamp);
            UpdateMarkup(Helpers.eMarkupKey.BALL_CIRCLE_MARK, new Point(x, y), 20);
            return coordinates;
        }

        static int counter = 0;
    }
}
