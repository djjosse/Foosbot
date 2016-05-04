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
using System.Collections.Generic;
using System.Drawing;

namespace Foosbot.ImageProcessingUnit.Detection.Contracts
{
    /// <summary>
    /// Interface to be implemented by class that can crop elements and store offset from originals
    /// </summary>
    public interface ICroppable
    {
        /// <summary>
        /// Offset on Axe X calculated and set in case of CropAndStoreOffset method called
        /// </summary>
        int OffsetX { get; }

        /// <summary>
        /// Offset on Axe Y calculated and set in case of CropAndStoreOffset method called
        /// </summary>
        int OffsetY { get; }

        /// <summary>
        /// Crop image by provided points. NOT storing/changing any offset data of Offset properties
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
        Image<Gray, byte> Crop(Image<Gray, byte> image, List<PointF> points);

        /// <summary>
        /// Crop image by provided points. NOT storing/changing any offset data of Offset properties
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
        Image<Gray, byte> Crop(Image<Gray, byte> image, List<PointF> points, out int xOffset, out int yOffset);

        /// <summary>
        /// Crops image by given points and stores calculated offsets.
        /// NOTE: THIS FUNCTION CHANGES OffsetY and OffsetX
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="points">Points to crop image by</param>
        /// <returns>Cropped image</returns>
        Image<Gray, byte> CropAndUpdate(Image<Gray, byte> image, List<PointF> points);


        /// <summary>
        /// Crops image by given circle center points and stores calculated offsets.
        /// NOTE: THIS FUNCTION CHANGES OffsetY and OffsetX
        /// </summary>
        /// <param name="image">Image to crop</param>
        /// <param name="circles">Circles to crop image based on those circle centers</param>
        /// <returns>Cropped image</returns>
        Image<Gray, byte> CropAndUpdate(Image<Gray, byte> image, List<CircleF> circles);
    }
}
