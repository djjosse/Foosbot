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
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using System;

namespace Foosbot.ImageProcessingUnit.Tools.Core
{
    /// <summary>
    /// Circle Detection Tool
    /// </summary>
    public class CircleDetector : ICircleDetector
    {
        #region constants

        /// <summary>
        /// Default Canny Threshold for Circle Detection
        /// </summary>
        public double DEFAULT_BASE_CANNY_THRESHOLD = 180.0;

        /// <summary>
        /// Default Circle Accumulation Threshold for Circle Detection
        /// </summary>
        public double DEFAULT_BASE_CIRCLE_ACCUMULATOR_THRESHOLD = 120.0;

        /// <summary>
        /// Default Inverse Ratio for Circle Detection
        /// </summary>
        public double DEFAULT_BASE_INVERSE_RATION = 2.0;

        #endregion constants

        #region properties 

        /// <summary>
        /// Canny threshold For Circle Detection
        /// </summary>
        public double CannyThreshold { get; set; }

        /// <summary>
        /// Circle Accumulator threshold For Circle Detection
        /// </summary>
        public double CircleAccumulator { get; set; }

        /// <summary>
        /// Inverse Ratio For Circle Detection
        /// </summary>
        public double InverseRatio { get; set; }

        #endregion properties

        /// <summary>
        /// Default Constructor for Circle Detector
        /// </summary>
        public CircleDetector()
        {
            CannyThreshold = DEFAULT_BASE_CANNY_THRESHOLD;
            CircleAccumulator = DEFAULT_BASE_CIRCLE_ACCUMULATOR_THRESHOLD;
            InverseRatio = DEFAULT_BASE_INVERSE_RATION;
        }

        /// <summary>
        /// Detect circles method
        /// </summary>
        /// <param name="image">Image to detect circles on</param>
        /// <param name="radius">Expected circle radius</param>
        /// <param name="error">Expected circle error rate</param>
        /// <param name="minDistance">Minimal distance between possible circle coordinates</param>
        /// <param name="cannyThreshold">Canny Threshold</param>
        /// <param name="circleAccumulatorThreshold">Circle Accumulator Threshold</param>
        /// <param name="inverseRatio">Inverse Ratio</param>
        /// <returns>Found circles in circle array</returns>
        public CircleF[] DetectCircles(Image<Gray, byte> image, int radius, double error, double minDistance)
        {
            int minRadius = (int)Math.Round((double)(radius - error * radius)) - 1;
            if (minRadius < 5) minRadius = 5;
            int maxRadius = (int)Math.Round((double)(radius + error * radius)) + 1;
            CircleF[] circles = CvInvoke.HoughCircles(image, HoughType.Gradient, InverseRatio,
                minDistance, CannyThreshold, CircleAccumulator, minRadius, maxRadius);
            return circles;
        }
    }
}
