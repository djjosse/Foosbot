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
using Foosbot.Common.Multithreading;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Core;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using System;

namespace Foosbot.ImageProcessingUnit.Tools.Core
{
    /// <summary>
    /// Tool used to display computer vision frames on the screen
    /// </summary>
    public class ComputerVisionFramesPublisher : Publisher<IFrame>, IComputerVisionMonitor
    {
        /// <summary>
        /// Show Frame Method - show image
        /// </summary>
        /// <param name="image">Image as EMGU-CV image</param>
        public void ShowFrame(Image<Gray, byte> image)
        {
            IFrame frame = new Frame();
            frame.Timestamp = DateTime.Now;
            frame.Image = image.Clone();
            ShowFrame(frame);
        }

        /// <summary>
        /// Show Frame Method - show image from given frame
        /// </summary>
        /// <param name="frame">Frame with image inside</param>
        public void ShowFrame(IFrame frame)
        {
            if (frame.Image != null)
            {
                Data = frame;
                NotifyAll();
            }
        }
    }
}
