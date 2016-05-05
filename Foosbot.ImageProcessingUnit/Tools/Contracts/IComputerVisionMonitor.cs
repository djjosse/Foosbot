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
using Foosbot.ImageProcessingUnit.Streamer.Contracts;

namespace Foosbot.ImageProcessingUnit.Tools.Contracts
{
    /// <summary>
    /// Interface for tool used to display computer vision frames on the screen
    /// </summary>
    public interface IComputerVisionMonitor
    {
        /// <summary>
        /// Show Frame Method - show image
        /// </summary>
        /// <param name="image">Image as EMGU-CV image</param>
        void ShowFrame(Image<Gray, byte> image);

        /// <summary>
        /// Show Frame Method - show image from given frame
        /// </summary>
        /// <param name="frame">Frame with image inside</param>
        void ShowFrame(IFrame frame);
    }
}
