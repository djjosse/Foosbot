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
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Iterface for callibration implementing class
    /// </summary>
    public interface ICallibration
    {
        /// <summary>
        /// Current Callibration State
        /// </summary>
        eCallibrationState CallibrationState { get; }

        /// <summary>
        /// Sorted Callibration Marks Coordinates on original image
        /// </summary>
        Dictionary<eCallibrationMark, CircleF> CallibrationMarks { get; set; }

        /// <summary>
        /// Background Image Found in callibration
        /// </summary>
        Image<Gray, Byte> Background { get; set; }

        /// <summary>
        /// Ball Radius
        /// </summary>
        int Radius { get; set; }

        /// <summary>
        /// Ball Radius Error +/- Radius
        /// </summary>
        double ErrorRate { get; set; }

    }
}
