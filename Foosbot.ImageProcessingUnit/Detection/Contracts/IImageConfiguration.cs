// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.ImageProcessingUnit.Detection.Contracts
{
    /// <summary>
    /// Configurable Image Processing Settings
    /// </summary>
    public interface IImageConfiguration
    {
        /// <summary>
        /// Circle Detection Pre-Processing Gray Threshold
        /// </summary>
        int CircleDetectionGrayThreshold { get; set; }

        /// <summary>
        /// Circle Detection Canny Threshold for Circle Edge Detection
        /// </summary>
        double CircleDetectionCannyThreshold { get; set; }

        /// <summary>
        /// Circle Accumulator Threshold for Circle Detection
        /// </summary>
        double CircleDetectionAccumulatorThreshold { get; set; }

        /// <summary>
        /// Circle Detection Method Inverse Ration
        /// </summary>
        double CircleDetectionInverseRatio { get; set; }

        /// <summary>
        /// Motion Detection Pre-Processing Gray Threshold
        /// </summary>
        int MotionDetectionGrayThreshold { get; set; }

        /// <summary>
        /// Minimal Motion Area Threshold
        /// </summary>
        double MinimalMotionAreaThreshold { get; set; }

        /// <summary>
        /// Factor for Minimal Motion Pixels in Motion Area
        /// </summary>
        double MinimalMotionPixelsFactor { get; set; }

    }
}
