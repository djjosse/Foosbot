﻿// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using EasyLog;
using Foosbot.Common.Data;
using Foosbot.Common.Enums;
using Foosbot.Common.Logs;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Process.Core;
using Foosbot.ImageProcessingUnit.Streamer.Core;
using System;
using System.Threading;
using System.Windows;

namespace Foosbot.DevelopmentDemo
{
    /// <summary>
    /// Demo Image Processing Unit - generates ball location on screen
    /// Used for validation of Vector Calculation unit and Decision Units
    /// </summary>
    public class DemoProcessingUnit : ImagingProcess
    {
        #region private demo members

        private Random _random;
        private double _velocityX = 0;
        private double _velocityY = 0;
        private double _ricochectFactor;
        private double _actualButtomBorder = 0;
        private double _buttomBorder = 0;
        private double _rightBorder = 0;
        private double _upeerBorder = 0;
        private double _leftBorder = 0;
        double ballX;
        double ballY;
        double deltaX = 20;
        double deltaY = 20;
        private volatile int _x = 0;
        private volatile int _y = 0;
        private int _ballRadius = 20;

        #endregion private demo members

        /// <summary>
        /// Demo Processing Unit Constructor
        /// </summary>
        /// <param name="streamer">Demo frame streamer</param>
        /// <param name="imagingData">Imaging Data</param>
        public DemoProcessingUnit(FramePublisher streamer, IImageData imagingData = null)
            : base(streamer, imagingData) 
        {

        }

        /// <summary>
        /// Generate a new ball coordinates and vector
        /// </summary>
        /// <param name="x">Ball x coordinate to start from [default 0]</param>
        /// <param name="y">Ball y coordinate to start from [default 0]</param>
        public void GenerateCoordinates(int x=0, int y=0)
        {
            Log.Print("Demo generating ball kick!", eCategory.Info, LogTag.IMAGE);
            _x = x;
            _y = y;
            _velocityX = _random.Next(-10, 10);
            _velocityX *= 4;
            _velocityY = _random.Next(-10, 10);
            _velocityY *= 4;
        }

        /// <summary>
        /// Resumes/Restarts the demo from center of a table
        /// </summary>
        public override void Resume()
        {
            GenerateCoordinates(Convert.ToInt32(_rightBorder / 2), Convert.ToInt32(_buttomBorder / 2));
            base.Resume();
        }

        /// <summary>
        /// Initialization method
        /// </summary>
        public override void Initialize()
        {
            if (!IsInitialized)
            {
                //Set ball radius property used for Draw Mark
                ImagingData.BallRadius = _ballRadius;

                //Set Foosbot world sizes - axe X x axe Y
                _rightBorder = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
                _buttomBorder = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
                _ricochectFactor = Configuration.Attributes.GetValue<double>(Configuration.Names.KEY_RICOCHET_FACTOR);

                _actualButtomBorder = _buttomBorder;

                //Create Transfomation Matrix - to present coordinates of a Ball in GUI
                InitializeTransformation(Convert.ToSingle((_publisher as FramePublisher).FrameWidth),
                    Convert.ToSingle((_publisher as FramePublisher).FrameHeight), Convert.ToSingle(_rightBorder),
                        Convert.ToSingle(_buttomBorder));

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

                IsInitialized = true;
            }
        }

        /// <summary>
        /// Main working method - runs in loop on Start
        /// </summary>
        public override void Job()
        {
            if (!IsPaused)
            {
                //Detach from streamer
                _publisher.Detach(this);

                //get current ball coordinates
                BallCoordinates coordinates = SampleCoordinates();

                //show current ball coordinates on screen and GUI
                System.Drawing.PointF p = TransformAgent.Data.InvertTransform(new System.Drawing.PointF(_x, _y));
                Marks.DrawBall(new Point(p.X, p.Y), _ballRadius);

                Statistics.TryUpdateBasicImageProcessingInfo(String.Format("Generated coordinates: {0}x{1}", _x, _y));

                //set current coordinates to update
                ImagingData.BallCoords = coordinates;

                //set current coordinates and publish new ball coordinates
                BallLocationUpdater.UpdateAndNotify();

                //attach back to streamer
                _publisher.Attach(this);
            }
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
            try
            {
                //Create corners of frame
                System.Drawing.PointF[] originalPoints = new System.Drawing.PointF[]
                {
                    new System.Drawing.PointF(0, 0),
                    new System.Drawing.PointF(frameWidth, 0),
                    new System.Drawing.PointF(0, frameHeight),
                    new System.Drawing.PointF(frameWidth, frameHeight)  
                };

                //Create corners of foosbot world
                System.Drawing.PointF[] transformedPoints = new System.Drawing.PointF[]
                {
                    new System.Drawing.PointF(0, 0),
                    new System.Drawing.PointF(worldWidth, 0),
                    new System.Drawing.PointF(0, worldHeight),
                    new System.Drawing.PointF(worldWidth, worldHeight)
                };

                //Calculate transformation matrix and store in static class
                TransformAgent.Data.Initialize(originalPoints, transformedPoints);
            }
            catch(Exception e)
            {
                MessageBox.Show("Unable to initialize transformation. Reason: " + e.Message + "\n" + e.StackTrace);
            }
        }


        /// <summary>
        /// Checks if one of the player and the ball coordinates intercept, if they are kick the ball
        /// </summary>
        private void CheckIfPlayerKickedTheBall()
        {
            ballX = _x + _velocityX;
            ballY = _y + _velocityY;
            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                Point player = Marks.PlayerPosition(rodType);
                if (ballX < player.X + deltaX && ballX > player.X - deltaX &&
                    ballY < player.Y + deltaY && ballY > player.Y - deltaY &&
                    _velocityX < 0)
                {
                    if (_random.Next(0, 100) > 30)
                    {
                        _x = Ricochet(_x, _x, ref _velocityX, ref _velocityY);
                        Log.Print(String.Format("Rod [{0}] responding to the ball!", rodType.ToString()), eCategory.Debug, LogTag.COMMON);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the ball hits on of the two X borders , if hits calculate ricochet
        /// </summary>
        private void CheckIfBallXHitBorders()
        {
            double tempX = _x + _velocityX;
            if (tempX <= _leftBorder)
            {
                _x = Ricochet(_x, _leftBorder, ref _velocityX, ref _velocityY);
            }
            else if (tempX >= _rightBorder)
            {
                _x = Ricochet(_x, _rightBorder, ref _velocityX, ref _velocityY);
            }
            else
            {
                _x = Convert.ToInt32(tempX);
            }
        }

        /// <summary>
        /// Checks if the ball hits on of the two Y borders , if hits calculate ricochet
        /// </summary>
        private void CheckIfBallYHitBorders()
        {
            double tempY = _y + _velocityY;
            if (tempY <= _upeerBorder)
            {
                _y = Ricochet(_y, _upeerBorder, ref _velocityY, ref _velocityX);
            }
            else if (tempY >= _buttomBorder)
            {
                _y = Ricochet(_y, _buttomBorder, ref _velocityY, ref _velocityX);
            }
            else
            {
                _y = Convert.ToInt32(tempY);
            }
        }

        /// <summary>
        /// Running in separate thread and generate ball locations
        /// </summary>
        private void BeginInvokeGenerateLocation()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    //generate vector if previous are 0
                    if (Convert.ToInt32(_velocityX) == 0)
                    {
                        Thread.Sleep(500);
                        GenerateCoordinates(_x, _y);
                    }

                    CheckIfPlayerKickedTheBall();
                    CheckIfBallXHitBorders();
                    CheckIfBallYHitBorders();

                    Thread.Sleep(10);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        void GenerateVector()
        {
            //generate vector if previous are 0
            if (Convert.ToInt32(_velocityX) == 0) //&& Convert.ToInt32(_velocityY) == 0)
            {
                Thread.Sleep(500);
                GenerateCoordinates(_x, _y);
            }
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
        private int Ricochet(int coordinate, double currentBorder,
            ref double directVelocity, ref double secondVelocity)
        {
            coordinate = Convert.ToInt32((2 * currentBorder - coordinate - directVelocity) + (currentBorder - coordinate) * _ricochectFactor);
            directVelocity = directVelocity * (-1) * _ricochectFactor;
            secondVelocity *= _ricochectFactor;
            return coordinate;
        }

        
    }
}
