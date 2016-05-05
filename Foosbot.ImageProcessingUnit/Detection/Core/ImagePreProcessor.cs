using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.ImageProcessingUnit.Detection.Contracts;
// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.ImageProcessingUnit.Detection.Core
{
    /// <summary>
    /// Image PreProcessor - used to prepare image for processing
    /// </summary>
    public class ImagePreProcessor : IImagePreparation
    {
        /// <summary>
        /// Default Gray Threshold for detection
        /// </summary>
        public const int DEFAULT_GRAY_THRESHOLD = 180;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImagePreProcessor()
        {
            ImagePreProcessingGrayThreshold = DEFAULT_GRAY_THRESHOLD;
        }

        /// <summary>
        /// Image Preparation Gray Threshold Property
        /// </summary>
        public int ImagePreProcessingGrayThreshold { get; set; }

        /// <summary>
        /// Prepare Image Method
        /// </summary>
        /// <param name="image">Image to prepare</param>
        /// <returns>Prepared Image</returns>
        public Image<Gray, byte> Prepare(Image<Gray, byte> image)
        {
            image = image.ThresholdToZero(new Gray(ImagePreProcessingGrayThreshold));
            image._EqualizeHist();
            return image;
        }
    }
}
