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
using Foosbot.ImageProcessingUnit.Process.Contracts;
using System.Collections.Generic;

namespace Foosbot.ImageProcessingUnit.Process.Core
{
    /// <summary>
    /// Common Image Data Class
    /// </summary>
    public class ImageData : IImageData
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ImageData()
        {
            CalibrationMarks = new Dictionary<eCallibrationMark, CircleF>();
        }

        /// <summary>
        /// Ball Coordinates Property
        /// </summary>
        public BallCoordinates BallCoords { get; set; }

        /// <summary>
        /// Ball Radius Property
        /// </summary>
        public int BallRadius { get; set; }

        /// <summary>
        /// Ball Radius Error Factor property
        /// </summary>
        public double BallRadiusError { get; set; }

        /// <summary>
        /// Ball Last Known Location (Pixels in Frame)
        /// </summary>
        public ITimedLocation LastKnownBallLocation { get; set; }

        /// <summary>
        /// Sorted Calibration Marks Coordinates on original image
        /// </summary>
        public Dictionary<eCallibrationMark, CircleF> CalibrationMarks { get; set; }
    }
}
