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
using Foosbot.ImageProcessingUnit.Detection.Enums;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using System;

namespace Foosbot.ImageProcessingUnit.Detection.Contracts
{
    /// <summary>
    /// Interface to implement by tracking class
    /// </summary>
    public interface ITracker: ICroppable
    {
        /// <summary>
        /// Image Pre Processor Instance
        /// </summary>
        IImagePreparation PreProcessor { get; }

        /// <summary>
        /// Motion Detector Instance
        /// </summary>
        IMotionDetector MotionInspector { get; }

        /// <summary>
        /// Common Imaging Data to be used in all IP Unit
        /// </summary>
        IImageData ImagingData { get; }

        /// <summary>
        /// Detect main method
        /// </summary>
        /// <param name="frame">Frame to detect object on</param>
        /// <returns>[True] if detected, [False] otherwise</returns>
        bool Detect(IFrame frame);

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
        bool FindBallLocationInFrame(Image<Gray, byte> image, DateTime timeStamp, eDetectionArea detectionArea = eDetectionArea.Full);

    }
}
