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

namespace Foosbot.ImageProcessingUnit.Detection.Contracts
{
    /// <summary>
    /// Detection Helper Interface
    /// </summary>
    public interface IImagePreparation
    {
        /// <summary>
        /// Gray Threshold for image Pre-Processing before motion detection
        /// </summary>
        int GrayThreshold { get; set; }

        /// <summary>
        /// Prepare Image
        /// </summary>
        /// <param name="image">Image to prepare</param>
        /// <returns>Prepared Image</returns>
        Image<Gray, byte> Prepare(Image<Gray, byte> image);
    }
}
