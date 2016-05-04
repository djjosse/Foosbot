// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Emgu.CV.Structure;
using Foosbot.Common.Protocols;
using System.Collections.Generic;

namespace Foosbot.ImageProcessingUnit.Process.Contracts
{
    /// <summary>
    /// Common Image Data Interface
    /// </summary>
    public interface IImageData
    {
        /// <summary>
        /// Ball Last Known Location (Pixels in Frame)
        /// </summary>
        ITimedLocation LastKnownBallLocation { get; set; }

        /// <summary>
        /// Ball Coordinates (Foosbot World)
        /// </summary>
        BallCoordinates BallCoords { get; set; }

        /// <summary>
        /// Ball Radius
        /// </summary>
        int BallRadius { get; set; }

        /// <summary>
        /// Ball Radius Error Factor
        /// </summary>
        double BallRadiusError { get; set; }

        /// <summary>
        /// Sorted Calibration Marks Coordinates on original image
        /// </summary>
        Dictionary<eCallibrationMark, CircleF> CalibrationMarks { get; set; }
    }
}
