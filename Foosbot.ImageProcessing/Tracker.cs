using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.Common;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    public class Tracker : Detector, ILastBallCoordinatesUpdater
    {
        public const double MAX_BALL_SPEED = 2000; //pixel per sec

        private CalibrationUnit _calibrator;
        private Publisher<Frame> _streamer;

        public BallCoordinates LastBallCoordinates { get; private set; }

        public Tracker(CalibrationUnit callibrator, Publisher<Frame> streamer, Helpers.UpdateMarkupDelegate onUpdateMarkup, Helpers.UpdateStatisticsDelegate onUpdateStatistics)
            : base(onUpdateMarkup, onUpdateStatistics)
        {
            _calibrator = callibrator;
            _streamer = streamer;
        }

        private Location _storedLocation;

        private Image<Gray, byte> PrepareImage(Image<Gray, byte> image)
        {
            //Crop current image
            image = CropAndStoreOffset(image, _calibrator.CallibrationMarks.Values.ToList());

            //remove noise
            image = NoiseRemove(image);
            //image.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + ".png");
            //remove background
            //_calibrator.Background.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "back.png");
            // image.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "_1.png");
            //_calibrator.Background.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "_2.png");
            //image = image.AbsDiff(_calibrator.Background);
            //res.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "res.png");

            //CvInvoke.Canny(image, image, 100, 60);
            //res.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "rescanny.png");

            image._EqualizeHist();
            return image;
        }

        public void InvokeTracking(Image<Gray, byte> image, DateTime imageTimestamp)
        {
            try
            {
                //Crop, noise remove, equalize histogram, etc.
                image = PrepareImage(image);

                //Has stored location
                if (_storedLocation!=null && _storedLocation.IsDefined) 
                {
                    bool isDetected = FindBallLocationInDefinedArea(image.Clone(), imageTimestamp);
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
                            FindBallLocationInFullArea(image, imageTimestamp);
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
                    FindBallLocationInFullArea(image, imageTimestamp);
                    //Log.Image.Info(String.Format(
                    //    "Searching for a ball on image from: {0}", imageTimestamp.ToString("HH:mm:ss.ffff")));
                    //LastBallCoordinates = new BallCoordinates(imageTimestamp);
                }
            }catch(Exception e)
            {
                Log.Image.Error(String.Format("[{0}] Unable to find the ball in current image. Reason: {1}", MethodBase.GetCurrentMethod().Name, e.Message));
            }
        }

        private bool FindBallLocationInFullArea(Image<Gray, byte> image, DateTime imageTimestamp)
        {
            CircleF[] pos = base.DetectCircles(image, _calibrator.Radius, _calibrator.ErrorRate * 2, _calibrator.Radius * 5, 300, 40, 1.3);
            if (pos.Length > 0)
            {
                _storedLocation = new Location(pos[0].Center.X, pos[0].Center.Y, imageTimestamp);

                int x = _storedLocation.X + OffsetX;
                int y = _storedLocation.Y + OffsetY;
                Log.Image.Info(String.Format("[{0}] Possible ball location in FULL area: {1}x{2}",
                    MethodBase.GetCurrentMethod().Name, x, y));
                UpdateMarkup(Helpers.eMarkupKey.BALL_CIRCLE_MARK, new System.Windows.Point(x, y), Convert.ToInt32(pos[0].Radius));
                return true;
            }
            Log.Image.Debug(String.Format("[{0}] Ball not found in FULL area", MethodBase.GetCurrentMethod().Name));
            return false;
        }

        private bool FindBallLocationInDefinedArea(Image<Gray, byte> image, DateTime imageTimestamp)
        {
            TimeSpan deltaT =  imageTimestamp - _storedLocation.Timestamp;
            double searchRadius = MAX_BALL_SPEED * deltaT.TotalSeconds;

            int maxX = (Convert.ToInt32(_storedLocation.X + searchRadius) > image.Width) ? image.Width : Convert.ToInt32(_storedLocation.X + searchRadius);
            int maxY = (Convert.ToInt32(_storedLocation.Y + searchRadius) > image.Height) ? image.Height : Convert.ToInt32(_storedLocation.Y + searchRadius);
            int additionalOffsetX = (Convert.ToInt32(_storedLocation.X - searchRadius) < 0) ? 0 : Convert.ToInt32(_storedLocation.X - searchRadius);
            int additionalOffsetY = (Convert.ToInt32(_storedLocation.Y - searchRadius) < 0) ? 0 : Convert.ToInt32(_storedLocation.Y - searchRadius);

            List<System.Drawing.PointF> croppingPoints = new List<System.Drawing.PointF>()
            {
                new System.Drawing.PointF(maxX, maxY),
                new System.Drawing.PointF(maxX, additionalOffsetY),
                new System.Drawing.PointF(additionalOffsetX, maxY),
                new System.Drawing.PointF(additionalOffsetX, additionalOffsetY)
            };

            Image<Gray, byte> croppedImage = base.Crop(image.Clone(), croppingPoints);
            croppedImage.Save("test\\" + imageTimestamp.ToString("mm_ss_fff") + ".png");

            CircleF[] pos = base.DetectCircles(croppedImage, _calibrator.Radius, _calibrator.ErrorRate * 2, _calibrator.Radius * 5, 300, 40, 1.3);
            if (pos.Length > 0)
            {
                _storedLocation = new Location(pos[0].Center.X + additionalOffsetX, pos[0].Center.Y + additionalOffsetY, imageTimestamp);

                int x = _storedLocation.X + OffsetX;
                int y = _storedLocation.Y + OffsetY;
                Log.Image.Info(String.Format("[{0}] Possible ball location in SELECTED area: {1}x{2}",
                    MethodBase.GetCurrentMethod().Name, x, y));
                UpdateMarkup(Helpers.eMarkupKey.BALL_CIRCLE_MARK, new System.Windows.Point(x, y), Convert.ToInt32(pos[0].Radius));
                return true;
            }
            Log.Image.Debug(String.Format("[{0}] Ball not found in SELECTED area", MethodBase.GetCurrentMethod().Name));
            return false;
        }
    }
}
