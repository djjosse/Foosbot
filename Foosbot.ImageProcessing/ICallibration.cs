using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
