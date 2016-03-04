using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
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
        /// Transformation Matrix calcullated on callibration
        /// </summary>
        Matrix<double> TransformationMatrix { get; set; }

        /// <summary>
        /// Invert Matrix for Transformation Matrix calcullated on callibration
        /// </summary>
        Matrix<double> InvertMatrix { get; set; }

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
        int Error { get; set; }

    }
}
