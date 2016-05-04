// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Enums;

namespace Foosbot.ImageProcessingUnit.Tools.Contracts
{
    /// <summary>
    /// Interface for calibration tool
    /// </summary>
    public interface ICalibration : IComputerVisionMonitorCollection
    {
        /// <summary>
        /// Common imaging data to set/update/use
        /// </summary>
        IImageData ImagingData { get; }

        /// <summary>
        /// Circle Detection Tool used to find calibration marks
        /// </summary>
        ICircleDetector CircleDetector { get; }

        /// <summary>
        /// Calibration Helper
        /// </summary>
        ICalibrationHelper CalibrationUtils { get; }

        /// <summary>
        /// Current Calibration State
        /// </summary>
        eCalibrationState CurrentState { get; }

        /// <summary>
        /// Perform Calibration Method
        /// </summary>
        /// <param name="frame">Frame to use for calibration</param>
        void Calibrate(IFrame frame);
    }
}
