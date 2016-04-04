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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Ball Tracker Class
    /// </summary>
    public class Tracker : Detector, ILastBallCoordinatesUpdater
    {
        #region Constants

        /// <summary>
        /// Maximum possible ball speen in pixels per seconds
        /// This will affect partial arial calculation in case of stored location from previous frame
        /// </summary>
        public const double MAX_BALL_SPEED = 2000;

        #endregion Constants

        #region Private members

        /// <summary>
        /// Calibrator Instance
        /// </summary>
        private CalibrationUnit _calibrator;

        /// <summary>
        /// Streamer Instance
        /// </summary>
        private Publisher<Frame> _streamer;

        /// <summary>
        /// Last Ball Detected Stored Location
        /// </summary>
        private Location _storedLocation;

        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Is Ball Location Found Property
        /// [True] if ball location found, [False] otherwise
        /// </summary>
        public bool IsBallLocationFound { get; private set; }

        /// <summary>
        /// Last Ball Coordinates after transformation applied stored
        /// </summary>
        public BallCoordinates LastBallCoordinates { get; private set; }

        #endregion Public Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callibrator">Calibrator Unit Instance</param>
        /// <param name="streamer">Streamer Unit Instance</param>
        public Tracker(CalibrationUnit callibrator, Publisher<Frame> streamer)
        {
            _calibrator = callibrator;
            _streamer = streamer;
        }

        /// <summary>
        /// Prepare Image Function
        /// Removes noises, crops image and adjusts contrasts to make ball detection easier
        /// </summary>
        /// <param name="image">Frame to prepare for ball detection</param>
        /// <returns>Image after adjustments</returns>
        private Image<Gray, byte> PrepareImage(Image<Gray, byte> image)
        {
            image = CropAndStoreOffset(image, _calibrator.CallibrationMarks.Values.ToList());
            image = NoiseRemove(image);
            image._EqualizeHist();
            return image;
        }

        /// <summary>
        /// Main tracking function
        /// </summary>
        /// <param name="image">Image to detect ball on</param>
        /// <param name="imageTimestamp">Image timestamp</param>
        public void InvokeTracking(Image<Gray, byte> image, DateTime imageTimestamp)
        {
            try
            {
                //Crop, noise remove, equalize histogram, etc.
                image = PrepareImage(image);

                //Has stored location
                if (_storedLocation!=null && _storedLocation.IsDefined) 
                {
                    bool isDetected = FindBallLocation(image, imageTimestamp, false);
                    if (!isDetected)
                    {
                        //Set location to undefined
                        _storedLocation = new Location(imageTimestamp);

                        //Last Frame in streamer timestamp
                        DateTime stamp = _streamer.Data.Timestamp;

                        //Check if there are new frames in streamer:
                        //No new Images
                        if (stamp.Equals(imageTimestamp)) 
                        {
                            Log.Image.Info(String.Format("[{0}] Unable to find in small area, searching full area: {1}x{2}",
                                MethodBase.GetCurrentMethod().Name, image.Width, image.Height));
                            FindBallLocation(image, imageTimestamp);
                        }
                        //Has new Image
                        else
                        {
                            //Skipp current image
                            Log.Image.Warning(String.Format("[{0}] Skipping current frame - already have new image",
                                MethodBase.GetCurrentMethod().Name));
                        }
                    }
                    else
                    {
                        Log.Image.Info(String.Format("[{0}] Found based on stored location. Updated to: [{1}x{2}]",
                            MethodBase.GetCurrentMethod().Name, _storedLocation.X, _storedLocation.Y));
                    }
                }
                else //We don't have stored location
                {
                    FindBallLocation(image, imageTimestamp);
                    //Log.Image.Info(String.Format(
                    //    "Searching for a ball on image from: {0}", imageTimestamp.ToString("HH:mm:ss.ffff")));
                    //LastBallCoordinates = new BallCoordinates(imageTimestamp);
                }
            }catch(Exception e)
            {
                Log.Image.Error(String.Format("[{0}] Unable to find the ball in current image. Reason: {1}", MethodBase.GetCurrentMethod().Name, e.Message));
            }
        }

        /// <summary>
        /// Find ball location in image in defined area
        /// </summary>
        /// <param name="image">Image to detect ball on</param>
        /// <param name="imageTimestamp">Image timestamp</param>
        /// <param name="isFullAreaSearch">[True] search will be performed in all image = default, 
        /// otherwise area will be defined based on last stored location adn maximum possible speed
        /// </param>
        /// <returns>[True] if ball location found, [False] otherwise</returns>
        private bool FindBallLocation(Image<Gray, byte> image, DateTime imageTimestamp, bool isFullAreaSearch = true)
        {
            image = image.Clone();

            int additionalOffsetX = 0;
            int additionalOffsetY = 0;
            string area = (isFullAreaSearch) ? "FULL" : "SELECTED";

            if (!isFullAreaSearch)
            {
                TimeSpan deltaT = imageTimestamp - _storedLocation.Timestamp;
                double searchRadius = MAX_BALL_SPEED * deltaT.TotalSeconds;

                int maxX = (Convert.ToInt32(_storedLocation.X + searchRadius) > image.Width) ? image.Width : Convert.ToInt32(_storedLocation.X + searchRadius);
                int maxY = (Convert.ToInt32(_storedLocation.Y + searchRadius) > image.Height) ? image.Height : Convert.ToInt32(_storedLocation.Y + searchRadius);
                additionalOffsetX = (Convert.ToInt32(_storedLocation.X - searchRadius) < 0) ? 0 : Convert.ToInt32(_storedLocation.X - searchRadius);
                additionalOffsetY = (Convert.ToInt32(_storedLocation.Y - searchRadius) < 0) ? 0 : Convert.ToInt32(_storedLocation.Y - searchRadius);

                List<System.Drawing.PointF> croppingPoints = new List<System.Drawing.PointF>()
                {
                    new System.Drawing.PointF(maxX, maxY),
                    new System.Drawing.PointF(maxX, additionalOffsetY),
                    new System.Drawing.PointF(additionalOffsetX, maxY),
                    new System.Drawing.PointF(additionalOffsetX, additionalOffsetY)
                };

                image = base.Crop(image, croppingPoints);
                //croppedImage.Save("test\\" + imageTimestamp.ToString("mm_ss_fff") + ".png");
            }

            CircleF[] pos = base.DetectCircles(image, _calibrator.Radius, _calibrator.ErrorRate * 2, _calibrator.Radius * 5, 250, 37, 1.5);//300, 40, 1.3);
            if (pos.Length > 0)
            {
                _storedLocation = new Location(pos[0].Center.X + additionalOffsetX, pos[0].Center.Y + additionalOffsetY, imageTimestamp);

                int x = _storedLocation.X + OffsetX;
                int y = _storedLocation.Y + OffsetY;

                Log.Image.Info(String.Format("[{0}] Possible ball location in {1} area: {2}x{3}",
                    MethodBase.GetCurrentMethod().Name, area, x, y));
                Marks.DrawBall(new System.Windows.Point(x, y), Convert.ToInt32(pos[0].Radius));
                
                Transformation transformer = new Transformation();
                System.Drawing.PointF coordinates = transformer.Transform(new System.Drawing.PointF(x, y));
                
                //not to change if it has not changed...
                if (LastBallCoordinates != null && LastBallCoordinates.IsDefined &&
                    LastBallCoordinates.X - _calibrator.Radius > coordinates.X &&
                    LastBallCoordinates.X + _calibrator.Radius < coordinates.X &&
                    LastBallCoordinates.Y - _calibrator.Radius > coordinates.Y &&
                    LastBallCoordinates.Y + _calibrator.Radius < coordinates.Y)
                {
                    LastBallCoordinates = new BallCoordinates(LastBallCoordinates.X, LastBallCoordinates.Y, imageTimestamp);
                }
                else
                {
                    this.LastBallCoordinates = new BallCoordinates(Convert.ToInt32(coordinates.X), Convert.ToInt32(coordinates.Y), imageTimestamp);
                }

                IsBallLocationFound = true;
                Statistics.UpdateBallCoordinates(
                    String.Format("Ball coordinates: {0}x{1}", LastBallCoordinates.X, LastBallCoordinates.Y));
                
                return true;
            }
            Log.Image.Debug(String.Format("[{0}] Ball not found in {1} area", MethodBase.GetCurrentMethod().Name, area));
            IsBallLocationFound = false;
            return false;
        }
    }
}
