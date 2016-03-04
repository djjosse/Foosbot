using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.Common;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    public class Tracker : Detector, ILastBallCoordinatesUpdater
    {
        private CalibrationUnit _calibrator;

        public BallCoordinates LastBallCoordinates { get; private set; }

        public Tracker(CalibrationUnit callibrator, Helpers.UpdateMarkupDelegate onUpdateMarkup, Helpers.UpdateStatisticsDelegate onUpdateStatistics)
            : base(onUpdateMarkup, onUpdateStatistics)
        {
            _calibrator = callibrator;
        }

        DateTime _currentImageTime;

        public void InvokeTracking(Image<Gray, byte> image, DateTime imageTimestamp)
        {
            try
            {
                //Store image timestamp
                _currentImageTime = imageTimestamp;
                //Crop current image
                image = Crop(image, _calibrator.CallibrationMarks.Values.ToList());

                //remove noise
                image = NoiseRemove(image);
                //image.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + ".png");
                //remove background
                //Image<Gray, Byte> res = _calibrator.Background.Clone().Sub(image.Clone());
                //res.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "res.png");

                //CvInvoke.Canny(res, res, 100, 60);
                //res.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "rescanny.png");

                double error = Convert.ToDouble(_calibrator.Error) / Convert.ToDouble(_calibrator.Radius);
                CircleF[] pos = base.DetectCircles(image, _calibrator.Radius*3, error * 15, _calibrator.Radius * 5, 180.0, 120.0, 2.0);
                image.Save("test//" + imageTimestamp.ToString("HH_mm_ss_fff") + "res.png");
                if (pos.Length > 0)
                {
                    UpdateMarkup(Helpers.eMarkupKey.BALL_CIRCLE_MARK, new System.Windows.Point(pos[0].Center.X, pos[0].Center.Y), Convert.ToInt32(pos[0].Radius));
                }
                    //Log.Image.Info(String.Format(
                //    "Searching for a ball on image from: {0}", imageTimestamp.ToString("HH:mm:ss.ffff")));
                //LastBallCoordinates = new BallCoordinates(imageTimestamp);
            }catch(Exception e)
            {
                Log.Image.Error(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
