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
using Foosbot.ImageProcessingUnit.Detection.Contracts;
using Foosbot.ImageProcessingUnit.Detection.Enums;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Core;
using Foosbot.ImageProcessingUnit.Tools.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Foosbot.ImageProcessingUnit.Detection.Core
{
    /// <summary>
    /// Object Tracker abstract class
    /// </summary>
    public abstract class Tracker : CircleDetector, ITracker, IComputerVisionMonitorCollection
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imagingData">Common Use Imaging Data</param>
        /// <param name="preprocessor">Image Preprocessor Unit [Default is nu will be created]</param>
        /// <param name="motionInspector">Motion Detection Unit [Default is nu will be created]</param>
        public Tracker(IImageData imagingData, IImagePreparation preprocessor = null, IMotionDetector motionInspector = null)
        {
            ComputerVisionMonitors = new Dictionary<eComputerVisionMonitor, IComputerVisionMonitor>();
            ImagingData = imagingData;
            MotionInspector = motionInspector ?? new MotionDetector(ImagingData);
            PreProcessor = preprocessor ?? new ImagePreProcessor();
        }

        /// <summary>
        /// Image Pre Processor Unit
        /// </summary>
        public IImagePreparation PreProcessor { get; private set; }

        /// <summary>
        /// Motion Detection Unit
        /// </summary>
        public IMotionDetector MotionInspector { get; private set; }

        /// <summary>
        /// Common Imaging Data used in Image Processing Unit
        /// </summary>
        public IImageData ImagingData { get; private set; }

        /// <summary>
        /// Offset on Axe X calculated and set in case of CropAndStoreOffset method called
        /// </summary>
        public int OffsetX { get; private set; }

        /// <summary>
        /// Offset on Axe Y calculated and set in case of CropAndStoreOffset method called
        /// </summary>
        public int OffsetY { get; private set; }

        /// <summary>
        /// Computer Vision Monitor Dictionary used to show processed frames on the screen
        /// </summary>
        public Dictionary<eComputerVisionMonitor, IComputerVisionMonitor> ComputerVisionMonitors { get; private set; }

        /// <summary>
        /// Abstract Main Detection Method
        /// </summary>
        /// <param name="frame">Frame to detect object on</param>
        /// <returns>[True] if object detected, [False] otherwise</returns>
        public abstract bool Detect(IFrame frame);

        /// <summary>
        /// Find ball location in frame in defined area
        /// </summary>
        /// <param name="image">Image to detect ball</param>
        /// <param name="timeStamp">Time Stamp of an image</param>
        /// <param name="detectionArea">Area to search ball in. If Selected:
        /// area will be defined based on last stored location and maximum possible speed.
        /// [Default is Full to search in all frame]
        /// </param>
        /// <returns>[True] if ball location found, [False] otherwise</returns>
        public abstract bool FindBallLocationInFrame(Image<Gray, byte> image, DateTime timeStamp, eDetectionArea detectionArea = eDetectionArea.Full);

        /// <summary>
        /// Crops image by given points and stores calculated offsets.
        /// NOTE: THIS FUNCTION CHANGES OffsetY and OffsetX
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
        public Image<Gray, byte> CropAndUpdate(Image<Gray, byte> image, List<PointF> points)
        {
            int xOffset, yOffset;
            Image<Gray, byte> croppedImage = Crop(image, points, out xOffset, out yOffset);
            OffsetX = xOffset;
            OffsetY = yOffset;
            return croppedImage;
        }

        /// <summary>
        /// Crop image by provided points. NOT storing/changing any offset data of Offset properties
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
        public Image<Gray, byte> Crop(Image<Gray, byte> image, List<PointF> points)
        {
            int x, y;
            return Crop(image, points, out x, out y);
        }

        /// <summary>
        /// Crop image by provided points. NOT storing/changing any offset data of Offset properties
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
        public Image<Gray, byte> Crop(Image<Gray, byte> image, List<PointF> points, out int xOffset, out int yOffset)
        {
            int xMax = 0;
            xOffset = image.Width;
            int yMax = 0;
            yOffset = image.Height;

            foreach (PointF point in points)
            {
                if (point.X > xMax)
                    xMax = Convert.ToInt32(point.X);
                if (point.X < xOffset)
                    xOffset = Convert.ToInt32(point.X);
                if (point.Y > yMax)
                    yMax = Convert.ToInt32(point.Y);
                if (point.Y < yOffset)
                    yOffset = Convert.ToInt32(point.Y);
            }

            int xSize = Convert.ToInt32(xMax - xOffset);
            int ySize = Convert.ToInt32(yMax - yOffset);


            Rectangle frame = new Rectangle(new System.Drawing.Point(xOffset, yOffset), new System.Drawing.Size(xSize, ySize));
            Mat cropped = null;
            try
            {
                cropped = new Mat(image.Clone().Mat, frame);
            }
            catch (Exception e)
            {
                Log.Image.Error(String.Format("[{0}] Error occurred during image crop. Reason: {1}",
                    MethodBase.GetCurrentMethod().Name, e.Message));
            }
            Image<Gray, byte> croppedImage = cropped.ToImage<Gray, byte>();
            return croppedImage;
        }

        /// <summary>
        /// Crops image by given circle center points and stores calculated offsets.
        /// NOTE: THIS FUNCTION CHANGES OffsetY and OffsetX
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="circles">Circles to crop image based on those circle centers</param>
        /// <returns>Cropped image</returns>
        public Image<Gray, byte> CropAndUpdate(Image<Gray, byte> image, List<CircleF> circles)
        {
            List<PointF> pointList = new List<PointF>();
            foreach (CircleF circle in circles)
                pointList.Add(circle.Center);
            return CropAndUpdate(image, pointList);
        }
    }
}
