using Foosbot;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            

            _borderX = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_HEIGHT);
            _borderY = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_WIDTH);

            System.Drawing.PointF[] originalPoints = new System.Drawing.PointF[4];
            originalPoints[0] = new System.Drawing.PointF(0, 0);
            originalPoints[1] = new System.Drawing.PointF(Convert.ToSingle(720), 0);
            originalPoints[2] = new System.Drawing.PointF(0, Convert.ToSingle(1280));
            originalPoints[3] = new System.Drawing.PointF(Convert.ToSingle(720), Convert.ToSingle(1280));

            System.Drawing.PointF[] transformedPoints = new System.Drawing.PointF[4];
            transformedPoints[0] = new System.Drawing.PointF(0, 0);
            transformedPoints[1] = new System.Drawing.PointF(Convert.ToSingle(_borderX), 0);
            transformedPoints[2] = new System.Drawing.PointF(0, Convert.ToSingle(_borderY));
            transformedPoints[3] = new System.Drawing.PointF(Convert.ToSingle(_borderX), Convert.ToSingle(_borderY));

            Transformation.CalculateAndSetHomographyMatrix(transformedPoints, originalPoints);

            (streamer as DemoStreamer).DemoImageProcessingUnit = this;

            updater = new DemoLastBallCoordinatesUpdater();
            BallLocationPublisher = new BallLocationPublisher(updater);

            GenerateLocation();
        }

       
        public override void Job()
        {
            _publisher.Dettach(this);
            //UpdateStatistics(Helpers.eStatisticsKey.BasicImageProcessingInfo, 
            //    String.Format("New frame received, generate location: {0}", DateTime.Now.ToString("ss:ffff")));

            BallCoordinates coordinates = SampleCoordinates();
            System.Drawing.PointF p = Transformation.Transform(new System.Drawing.PointF(_x, _y));
            UpdateMarkup(Helpers.eMarkupKey.BALL_CIRCLE_MARK, new Point(p.X, p.Y), 20);
            UpdateStatistics(Helpers.eStatisticsKey.BasicImageProcessingInfo,
                String.Format("Generated location: {0}x{1}", _x, _y));

            Log.Common.Debug(String.Format("{0}x{1} => {2}x{3}", _x, _y, p.X, p.Y));
            //BallCoordinates coordinates = GenerateLocation();
            //updater.LastBallCoordinates = coordinates;

            BallLocationPublisher.UpdateAndNotify();
            _publisher.Attach(this);
        }

        private Random random;
        private double _stepX = 0;
        private double _stepY = 0;
        private double _richisheFactor = 0.7;
        private double _borderX = 544;
        private double _borderY = 960;
        private volatile int _x = 0;
        private volatile int _y = 0;

        public void GenerateLocation()
        {
            Thread t = new Thread(() =>
            {

                _x = Convert.ToInt32(_borderY/2);
                _y = Convert.ToInt32(_borderX / 2);

                random = new Random();
                while(true)
                {
                    //generate steps if previous are 0
                    if (Convert.ToInt32(_stepX) == 0 && Convert.ToInt32(_stepY) == 0)
                    {
                        Thread.Sleep(1000);
                        _stepX = random.Next(-10, 10);
                        _stepX *= 2;
                        _stepY = random.Next(-10, 10);
                        _stepY *= 2;
                    }

                    //check if we passed the border
                    double tempX = _x + _stepX;
                    if (tempX <= 0)
                    {
                        _x = Convert.ToInt32((_stepX * (-1) - _x) * _richisheFactor);
                        _stepX = _stepX * (-1) * _richisheFactor;
                        _stepY *= _richisheFactor;
                    }
                    else if (tempX >= _borderY)
                    {
                        _x = Convert.ToInt32((2 * _borderY - _x - _stepX) + (_borderY - _x) * _richisheFactor);
                        _stepX = _stepX * (-1) * _richisheFactor;
                        _stepY *= _richisheFactor;
                    }
                    else
                    {
                        _x = Convert.ToInt32(tempX);
                    }

                    double tempY = _y + _stepY;
                    if (tempY <= 0)
                    {
                        _y = Convert.ToInt32((_stepY * (-1) - _y) * _richisheFactor);
                        _stepY = _stepY * (-1) * _richisheFactor;
                        _stepX *= _richisheFactor;
                    }
                    else if (tempY >= _borderX)
                    {
                        _y = Convert.ToInt32((2 * _borderX - _y - _stepY) + (_borderX - _y) * _richisheFactor);
                        _stepY = _stepY * (-1) * _richisheFactor;
                        _stepX *= _richisheFactor;
                    }
                    else
                    {
                        _y = Convert.ToInt32(tempY);
                    }

                    Thread.Sleep(10);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        private BallCoordinates SampleCoordinates()
        {
            return new BallCoordinates(_x, _y, DateTime.Now);
        }

    }
}
