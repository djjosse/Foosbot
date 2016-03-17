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
    /// <summary>
    /// Abstract Circle Detector Class
    /// </summary>
    public abstract class Detector
    {
        /// <summary>
        /// Update Markup Delegate
        /// </summary>
        protected Helpers.UpdateMarkupCircleDelegate UpdateMarkup;

        /// <summary>
        /// Update Statistics Delegate
        /// </summary>
        protected Helpers.UpdateStatisticsDelegate UpdateStatistics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onUpdateMarkup">On update markup method delegate</param>
        /// <param name="onUpdateStatistics">On update statistics method delegate</param>
        public Detector(Helpers.UpdateMarkupCircleDelegate onUpdateMarkup, Helpers.UpdateStatisticsDelegate onUpdateStatistics)
        {
            UpdateMarkup = onUpdateMarkup;
            UpdateStatistics = onUpdateStatistics;
        }

        /// <summary>
        /// Noise remove from image method
        /// </summary>
        /// <param name="image">Image to remove noise</param>
        /// <returns>Image without noise</returns>
        public Image<Gray, byte> NoiseRemove(Image<Gray, byte> image)
        {
            image._SmoothGaussian(9);
            //UMat pyrDown = new UMat();
            //CvInvoke.PyrDown(image.Clone(), pyrDown);
            //CvInvoke.PyrUp(pyrDown, image);
            return image;
        }

        /// <summary>
        /// Detect circles method
        /// </summary>
        /// <param name="image">Image to detect circles on</param>
        /// <param name="radius">Expected circle radius</param>
        /// <param name="error">Expected circle error rate</param>
        /// <param name="minDistance">Minimal distance beetween possible circle coordinates</param>
        /// <param name="cannyThreshold">Canny Threshold</param>
        /// <param name="circleAccumulatorThreshold">Circle Accumulator Threashold</param>
        /// <param name="inverseRatio">Inverse Ratio</param>
        /// <returns>Found circles in circle array</returns>
        public CircleF[] DetectCircles(Image<Gray, byte> image, int radius, double error, double minDistance,
            double cannyThreshold = 180.0, double circleAccumulatorThreshold = 120, double inverseRatio = 2.0)
        {
            int minRadius = (int)Math.Round((double)(radius - error * radius))-1;
            if (minRadius < 5) minRadius = 5;
            int maxRadius = (int)Math.Round((double)(radius + error * radius))+1;
            CircleF[] circles = CvInvoke.HoughCircles(image, HoughType.Gradient, inverseRatio,
                minDistance, cannyThreshold, circleAccumulatorThreshold, minRadius, maxRadius);
            return circles;
        }

        /// <summary>
        /// Offset on axe X callculated and set in case of CropAndStoreOffset method called
        /// </summary>
        public int OffsetX { get; private set; }

        /// <summary>
        /// Offset on axe Y callculated and set in case of CropAndStoreOffset method called
        /// </summary>
        public int OffsetY { get; private set; }

        /// <summary>
        /// Crops image by given point, callculates and stores offset
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
        public Image<Gray, byte> CropAndStoreOffset(Image<Gray, byte> image, List<PointF> points)
        {
            int xMax = 0;
            OffsetX = image.Width;
            int yMax = 0;
            OffsetY = image.Height;

            foreach (PointF point in points)
            {
                if (point.X > xMax)
                    xMax = Convert.ToInt32(point.X);
                if (point.X < OffsetX)
                    OffsetX = Convert.ToInt32(point.X);
                if (point.Y > yMax)
                    yMax = Convert.ToInt32(point.Y);
                if (point.Y < OffsetY)
                    OffsetY = Convert.ToInt32(point.Y);
            }

            int xSize = Convert.ToInt32(xMax - OffsetX);
            int ySize = Convert.ToInt32(yMax - OffsetY);

            Rectangle frame = new Rectangle(new System.Drawing.Point(OffsetX, OffsetY), new System.Drawing.Size(xSize, ySize));

            Mat cropped = new Mat(image.Clone().Mat, frame);
            Image<Gray, byte> croppedImage = cropped.ToImage<Gray, byte>();
            return croppedImage;
        }

        /// <summary>
        /// Crop image without storing/changing any offset data
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
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

        /// <summary>
        /// Crop and store offset based on given circle center points
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="circles">Circles to crop image based on those circle centers</param>
        /// <returns>Cropped image</returns>
        public Image<Gray, byte> CropAndStoreOffset(Image<Gray, byte> image, List<CircleF> circles)
        {
            List<PointF> pointList = new List<PointF>();
            foreach (CircleF circle in circles)
                pointList.Add(circle.Center);
            return CropAndStoreOffset(image, pointList);
        }
    }
}
