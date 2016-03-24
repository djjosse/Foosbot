using Foosbot;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessing;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace DevDemos
{
    public class DemoImageProcessingUnit : AbstractImageProcessingUnit
    {
        #region private members

        private Random _random;
        private double _velocityX = 0;
        private double _velocityY = 0;
        private readonly double RICOCHET_FACTOR;
        private double _actualButtomBorder = 0;
        private double _buttomBorder = 0;
        private double _rightBorder = 0;
        private double _upeerBorder = 0;
        private double _leftBorder = 0;
        private volatile int _x = 0;
        private volatile int _y = 0;
        private int _ballRadius = 10;
        private DemoLastBallCoordinatesUpdater _coordinatesUpdater;

        #endregion private members

        /// <summary>
        /// Demo Image Processing Unit constructor
        /// </summary>
        /// <param name="streamer">DemoStreamer instance</param>
        public DemoImageProcessingUnit(DemoStreamer streamer) : base(streamer)
        {
            //Set Foosbot world sizes - axe X x axe Y
            _rightBorder = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            _buttomBorder = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            RICOCHET_FACTOR = Configuration.Attributes.GetValue<double>(Configuration.Names.KEY_RICOCHET_FACTOR);

            _actualButtomBorder = _buttomBorder;

            //Create Transfomation Matrix - to present coordinates of a Ball in GUI
            InitializeTransformation(Convert.ToSingle(streamer.FrameWidth),
                Convert.ToSingle(streamer.FrameHeight), Convert.ToSingle(_rightBorder),
                    Convert.ToSingle(_buttomBorder));

            //Set this Demo Image Processing Unit as observer for Demo Streamer
            streamer.DemoImageProcessingUnit = this;

            //Create Ball Location Publisher for Observers
            _coordinatesUpdater = new DemoLastBallCoordinatesUpdater();
            BallLocationPublisher = new BallLocationPublisher(_coordinatesUpdater);

            //Set borders of frame to include ball radius
            _rightBorder -= _ballRadius;
            _buttomBorder -= _ballRadius;
            _leftBorder += _ballRadius;
            _upeerBorder += _ballRadius;

            //Set start ball coordinates
            _x = Convert.ToInt32(_rightBorder / 2);
            _y = Convert.ToInt32(_buttomBorder / 2);

            //Instantiate random generator
            _random = new Random();

            //Start Generating Locations in separate Thread
            BeginInvokeGenerateLocation();
        }

        /// <summary>
        /// Initialize Transformation and Invert Matrices for transforming 
        /// points from frame to Foosbot World and Back
        /// </summary>
        /// <param name="frameWidth">Frame width</param>
        /// <param name="frameHeight">Frame height</param>
        /// <param name="worldWidth">Foosbot world width</param>
        /// <param name="worldHeight">Foosbot world height</param>
        private void InitializeTransformation(float frameWidth, float frameHeight, float worldWidth, float worldHeight)
        {
            //Create corners of frame
            System.Drawing.PointF[] originalPoints = new System.Drawing.PointF[4];
            originalPoints[0] = new System.Drawing.PointF(0, 0);
            originalPoints[1] = new System.Drawing.PointF(frameWidth, 0);
            originalPoints[2] = new System.Drawing.PointF(0, frameHeight);
            originalPoints[3] = new System.Drawing.PointF(frameWidth, frameHeight);

            //Create corners of foosbot world
            System.Drawing.PointF[] transformedPoints = new System.Drawing.PointF[4];
            transformedPoints[0] = new System.Drawing.PointF(0, 0);
            transformedPoints[1] = new System.Drawing.PointF(worldWidth, 0);
            transformedPoints[2] = new System.Drawing.PointF(0, worldHeight);
            transformedPoints[3] = new System.Drawing.PointF(worldWidth, worldHeight);

            //Calculate trabsformation matrix and store in static class
            Transformation transformer = new Transformation();
            transformer.FindHomographyMatrix(originalPoints, transformedPoints);  
        }
       
        /// <summary>
        /// Get and notify new ball coordinates
        /// </summary>
        public override void Job()
        {
            //Detach from streamer
            _publisher.Dettach(this);
    
            //get current ball coordinates
            BallCoordinates coordinates = SampleCoordinates();
   
            //draw rods
            Marks.DrawRodtMark(new Point(50, 0), new Point(0, _actualButtomBorder), 10, true, System.Windows.Media.Brushes.White);

            //show current ball coordinates on screen and GUI
            Transformation transfromer = new Transformation();
            System.Drawing.PointF p = transfromer.InvertTransform(new System.Drawing.PointF(_x, _y));
            Marks.DrawBall(new Point(p.X, p.Y), _ballRadius * 2, true);

            Statistics.UpdateBasicImageProcessingInfo(String.Format("Generated coordinates: {0}x{1}", _x, _y));
            
            //set current coordinates to update
            _coordinatesUpdater.LastBallCoordinates = coordinates;

            //publish new ball coordinates
            BallLocationPublisher.UpdateAndNotify();



            //attach back to streamer
            _publisher.Attach(this);
        }

        /// <summary>
        /// Running in separate thread and generate ball locations
        /// </summary>
        private void BeginInvokeGenerateLocation()
        {
            Thread t = new Thread(() =>
            {
                while(true)
                {
                    //generate vector if previous are 0
                    if (Convert.ToInt32(_velocityX) == 0 && Convert.ToInt32(_velocityY) == 0)
                    {
                        Thread.Sleep(1000);
                        _velocityX = _random.Next(-10, 10);
                        _velocityX *= 4;
                        _velocityY = _random.Next(-10, 10);
                        _velocityY *= 4;
                    }

                    //check if we passed the border and set new X coordinate
                    double tempX = _x + _velocityX;
                    if (tempX <= _leftBorder)
                        _x = Ricochet(_x, ref _leftBorder, ref _velocityX, ref _velocityY);
                    else if (tempX >= _rightBorder)
                        _x = Ricochet(_x, ref _rightBorder, ref _velocityX, ref _velocityY);
                    else
                        _x = Convert.ToInt32(tempX);

                    //check if we passed the border and set new Y coordinate
                    double tempY = _y + _velocityY;
                    if (tempY <= _upeerBorder)
                        _y = Ricochet(_y, ref _upeerBorder, ref _velocityY, ref _velocityX);
                    else if (tempY >= _buttomBorder)
                        _y = Ricochet(_y, ref _buttomBorder, ref _velocityY, ref _velocityX);
                    else
                        _y = Convert.ToInt32(tempY);

                    //Sleep before next generation
                    Thread.Sleep(10);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Get sample coordinates
        /// </summary>
        /// <returns>Current coordinates of the ball</returns>
        private BallCoordinates SampleCoordinates()
        {
            return new BallCoordinates(_x, _y, DateTime.Now);
        }

        /// <summary>
        /// Switch direction and set new velocity in case we reached the border
        /// </summary>
        /// <param name="coordinate">Coordinate to change</param>
        /// <param name="currentBorder">Border we meet</param>
        /// <param name="directVelocity">Vector coordinate to change direction</param>
        /// <param name="secondVelocity">Other vector coordinate</param>
        /// <returns>New coordinate after applying the changes</returns>
        private int Ricochet(int coordinate, ref double currentBorder,
            ref double directVelocity, ref double secondVelocity)
        {
            coordinate = Convert.ToInt32((2 * currentBorder - coordinate - directVelocity) 
                + (currentBorder - coordinate) * RICOCHET_FACTOR);
            directVelocity = directVelocity * (-1) * RICOCHET_FACTOR;
            secondVelocity *= RICOCHET_FACTOR;
            return coordinate;
        }
    }
}
