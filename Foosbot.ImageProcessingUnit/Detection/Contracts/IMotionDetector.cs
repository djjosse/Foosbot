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
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using System.Drawing;

namespace Foosbot.ImageProcessingUnit.Detection.Contracts
{
    /// <summary>
    /// Interface to be implemented by Motion Detector
    /// </summary>
    public interface IMotionDetector : IImagePreparation, IComputerVisionMonitorCollection
    {
        /// <summary>
        /// Threshold to define a motion area, reduce the value to detect smaller motion
        /// </summary>
        double MinMotionAreaThreshold { get; set; }

        /// <summary>
        /// Factor to reject the area that contains too few motion
        /// </summary>
        double MinMotionPixelFactor { get; set; }

        /// <summary>
        /// Detected Motion Location on provided to Detect method image
        /// </summary>
        Point DetectedLocation { get; }

        /// <summary>
        /// Common Imaging Data used in Image Processing Unit
        /// </summary>
        IImageData ImagingData { get; }

        /// <summary>
        /// Detect ball on last image provided to BeginInvokeUpdateMotionHisitory method using motion detection
        /// This will update DetectedLocation property
        /// </summary>
        /// <returns>[True] if found, [False] otherwise (also if image is null)</returns>
        bool WaitForDetectionResult();

        /// <summary>
        /// Update Motion History is used to update history without motion detection
        /// This will call another background thread to update.
        /// </summary>
        /// <param name="image">Image to update motion history with</param>
        void BeginInvokeUpdateMotionHisitory(Image<Gray, byte> image);
    }
}
