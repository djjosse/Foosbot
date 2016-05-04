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

namespace Foosbot.ImageProcessingUnit.Tools.Contracts
{
    /// <summary>
    /// Interface for Circle Detection Tool
    /// </summary>
    public interface ICircleDetector
    {
        /// <summary>
        /// Canny threshold For Circle Detection
        /// </summary>
        double CannyThreshold { get; set; }

        /// <summary>
        /// Circle Accumulator threshold For Circle Detection
        /// </summary>
        double CircleAccumulator { get; set; }

        /// <summary>
        /// Inverse Ratio For Circle Detection
        /// </summary>
        double InverseRatio { get; set; }

        /// <summary>
        /// Detect circles method
        /// </summary>
        /// <param name="image">Image to detect circles on</param>
        /// <param name="radius">Expected circle radius</param>
        /// <param name="error">Expected circle error rate</param>
        /// <param name="minDistance">Minimal distance between possible circle coordinates</param>
        /// <returns>Found circles in circle array</returns>
        CircleF[] DetectCircles(Image<Gray, byte> image, int radius, double error, double minDistance);
    }
}
