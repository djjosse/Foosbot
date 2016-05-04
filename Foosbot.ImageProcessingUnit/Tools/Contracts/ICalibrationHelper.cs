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
using System.Collections.Generic;

namespace Foosbot.ImageProcessingUnit.Tools.Contracts
{
    /// <summary>
    /// Calibration Helper Interface for some calibration utilities in flow
    /// </summary>
    public interface ICalibrationHelper
    {
        /// <summary>
        /// Prepare Image:
        /// 1. Apply Canny Filter to detect edges
        /// 2. Smooth Image
        /// </summary>
        /// <param name="image">Image to prepare</param>
        /// <returns>Prepared Image</returns>
        Image<Gray, byte> PrepareFrame(Image<Gray, byte> image);

        /// <summary>
        /// Calculate ball radius and possible error 
        /// </summary>
        /// <param name="origRadius">Original Ball Radius in mm</param>
        /// <param name="ballRadius">Calculated Ball Radius as Out Parameter (pixels)</param>
        /// <param name="ballError">Calculated Ball Error as Out Parameter (pixels)</param>
        void CalculateBallRadiusAndError(float origRadius, out int ballRadius, out double ballError);

        /// <summary>
        /// Find Diagonal pairs of detected calibration marks.
        /// Based on assumption those pairs have largest distance.
        /// </summary>
        /// <param name="unsortedMarks">Unsorted marks list</param>
        /// <returns>Dictionary of 2 diagonal pairs of marks</returns>
        Dictionary<CircleF, CircleF> FindDiagonalMarkPairs(List<CircleF> unsortedMarks);

        /// <summary>
        /// Verifies exactly 4 calibration marks found
        /// </summary>
        /// <param name="circles">Calibration marks circles</param>
        /// <exception cref="CalibrationException">Thrown in case not exactly 4 calibration marks found</exception>
        void VerifyMarksFound(List<CircleF> circles);

        /// <summary>
        /// Update Calibration Marks in order to get better coverage
        /// </summary>
        /// <param name="calibrationMarks">CalibrationMarks to update</param>
        /// <returns>Updated marks</returns>
        Dictionary<eCallibrationMark, CircleF> UpdateCoverage(Dictionary<eCallibrationMark, CircleF> calibrationMarks);

        /// <summary>
        /// Set transformation matrices in Transformer to be used in all further calculations
        /// Both input parameters are length between calibration marks
        /// </summary>
        /// <param name="axeXlength">Table X Axe length in Points (PTS)</param>
        /// <param name="axeYlength">Table Y Axe length in Points (PTS)</param>
        /// <param name="calibrationMarks">Calibration marks detected on image</param>
        void SetTransformationMatrix(int axeXlength, int axeYlength, Dictionary<eCallibrationMark, CircleF> calibrationMarks);

        /// <summary>
        /// Show calibration mark on screen - circle and coordinates
        /// </summary>
        /// <param name="mark">Calibration mark as CircleF</param>
        /// <param name="key">Calibration mark type</param>
        void ShowCalibrationMark(CircleF mark, eCallibrationMark key);
    }
}
