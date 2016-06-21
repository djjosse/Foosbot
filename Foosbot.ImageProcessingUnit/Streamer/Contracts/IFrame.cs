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
using System;
using System.Windows.Media.Imaging;

namespace Foosbot.ImageProcessingUnit.Streamer.Contracts
{
    /// <summary>
    /// Interface for image with Time Stamp
    /// </summary>
    public interface IFrame : IDisposable
    {
        /// <summary>
        /// Actual Image in frame
        /// </summary>
        Image<Gray, byte> Image { get; set; }

        /// <summary>
        /// Image Time-stamp
        /// </summary>
        DateTime Timestamp { get; set; }

        /// <summary>
        /// Convert Image to Bitmap Source Function
        /// </summary>
        /// <returns></returns>
        BitmapSource ToBitmapSource();

        /// <summary>
        /// Create exact copy of current frame
        /// </summary>
        /// <returns>IFrame</returns>
        IFrame Clone();
    }
}
