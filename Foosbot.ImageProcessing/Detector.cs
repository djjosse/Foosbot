using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foosbot.ImageProcessing
{
    public abstract class Detector
    {
        protected Helpers.UpdateMarkupDelegate UpdateMarkup;
        protected Helpers.UpdateStatisticsDelegate UpdateStatistics;

        public Detector(Helpers.UpdateMarkupDelegate onUpdateMarkup, Helpers.UpdateStatisticsDelegate onUpdateStatistics)
        {
            UpdateMarkup = onUpdateMarkup;
            UpdateStatistics = onUpdateStatistics;
        }



        public Image<Gray, byte> NoiseRemove(Image<Gray, byte> image)
        {
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(image, pyrDown);
            CvInvoke.PyrUp(pyrDown, image);
            return image;
        }

        public CircleF[] DetectCircles(Image<Gray, byte> image, int radius, double error, double minDistance,
            double cannyThreshold = 180.0, double circleAccumulatorThreshold = 120, double inverseRatio = 2.0)
        {
            int minRadius = (int)Math.Round((double)(radius - error * radius));
            int maxRadius = (int)Math.Round((double)(radius + error * radius));
            CircleF[] circles = CvInvoke.HoughCircles(image, HoughType.Gradient, inverseRatio,
                minDistance, cannyThreshold, circleAccumulatorThreshold, minRadius, maxRadius);
            return circles;
        }

        public Image<Gray, byte> Crop(Image<Gray, byte> image, List<PointF> points)
        {
            int xMax = 0;
            int xMin = image.Width;
            int yMax = 0;
            int yMin = image.Height;

            foreach (PointF point in points)
            {
                if (point.X > xMax)
                    xMax = Convert.ToInt32(point.X);
                if (point.X < xMin)
                    xMin = Convert.ToInt32(point.X);
                if (point.Y > yMax)
                    yMax = Convert.ToInt32(point.Y);
                if (point.Y < yMin)
                    yMin = Convert.ToInt32(point.Y);
            }

            int xSize = Convert.ToInt32(xMax - xMin);
            int ySize = Convert.ToInt32(yMax - yMin);

            Rectangle frame = new Rectangle(new System.Drawing.Point(xMin, yMin), new System.Drawing.Size(xSize, ySize));

            Mat cropped = new Mat(image.Clone().Mat, frame);
            Image<Gray, byte> croppedImage = cropped.ToImage<Gray, byte>();
            return croppedImage;
        }

        public Image<Gray, byte> Crop(Image<Gray, byte> image, List<CircleF> circles)
        {
            List<PointF> pointList = new List<PointF>();
            foreach (CircleF circle in circles)
                pointList.Add(circle.Center);
            return Crop(image, pointList);
        }
    }
}
