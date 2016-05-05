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
using Foosbot.Common;
using Foosbot.Common.Exceptions;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Foosbot.ImageProcessingUnit.Tools.Core
{
    /// <summary>
    /// Calibration Helper contains some utilities for calibration flow
    /// </summary>
    public class CalibrationHelper : ICalibrationHelper
    {
        #region Prepare Image Constants 

        /// <summary>
        /// Gaussian Kernel Size for Calibration Image Pre Processing
        /// </summary>
        public int GAUSSIAN_KERNEL_SIZE = 9;

        /// <summary>
        /// Canny Method First Threshold for Calibration Image Pre Processing
        /// </summary>
        public int CANNY_FIST_THRESHOLD = 100;

        /// <summary>
        /// Canny Method Second Threshold for Calibration Image Pre Processing
        /// </summary>
        public int CANNY_SECOND_THRESHOLD = 60;

        #endregion Prepare Image Constants

        /// <summary>
        /// Calibration mark locations on table by names in dictionary in Foosbot World
        /// Metrics here is in points (PTS) not in Pixels
        /// </summary>
        private Dictionary<eCallibrationMark, System.Drawing.PointF> _markCoordinates;

        /// <summary>
        /// Prepare Image:
        /// 1. Apply Canny Filter to detect edges
        /// 2. Smooth Image
        /// </summary>
        /// <param name="image">Image to prepare</param>
        /// <returns>Prepared Image</returns>
        public Image<Gray, byte> PrepareFrame(Image<Gray, byte> image)
        {
            CvInvoke.Canny(image, image, CANNY_FIST_THRESHOLD, CANNY_SECOND_THRESHOLD);
            image._SmoothGaussian(GAUSSIAN_KERNEL_SIZE);
            return image;
        }

        /// <summary>
        /// Calculate ball radius and possible error 
        /// </summary>
        /// <param name="origRadius">Original Ball Radius in mm</param>
        /// <param name="ballRadius">Calculated Ball Radius as Out Parameter (pixels)</param>
        /// <param name="ballError">Calculated Ball Error as Out Parameter (pixels)</param>
        public void CalculateBallRadiusAndError(float origRadius, out int ballRadius, out double ballError)
        {
            double minRadius = origRadius * 100;
            double maxRadius = 0;
            foreach (System.Drawing.PointF start in _markCoordinates.Values)
            {
                System.Drawing.PointF end = new System.Drawing.PointF(start.X + origRadius, start.Y);

                double check = Utils.Distance(start, end);

                Transformation transformer = new Transformation();
                System.Drawing.PointF transformedStart = transformer.InvertTransform(start);
                System.Drawing.PointF transformedEnd = transformer.InvertTransform(end);

                double radius = Utils.Distance(transformedStart, transformedEnd);

                if (radius > maxRadius)
                    maxRadius = radius;
                if (radius < minRadius)
                    minRadius = radius;
            }

            ballRadius = Convert.ToInt32((minRadius + maxRadius) / 2);
            double possibleError = ((maxRadius - minRadius) / 2) + 1;
            ballError = Math.Round(possibleError / ballRadius, 3);
        }

        /// <summary>
        /// Find Diagonal pairs of detected calibration marks.
        /// Based on assumption those pairs have largest distance.
        /// </summary>
        /// <param name="unsortedMarks">Unsorted marks list</param>
        /// <returns>Dictionary of 2 diagonal pairs of marks</returns>
        public Dictionary<CircleF, CircleF> FindDiagonalMarkPairs(List<CircleF> unsortedMarks)
        {
            //Find All Pairs based on distance
            Dictionary<CircleF, CircleF> pairs = new Dictionary<CircleF, CircleF>();

            //Go over all unsorted marks 
            for (int i = 0; i < 4; i++)
            {
                //Add mark to dictionary as key and add as value mark with largest distance
                pairs.Add(unsortedMarks[i], default(CircleF));
                double largestDist = 0;
                for (int j = 0; j < 4; j++)
                {
                    double distance = Utils.Distance(unsortedMarks[i].Center, unsortedMarks[j].Center);
                    if (i != j && distance > largestDist)
                    {
                        pairs[unsortedMarks[i]] = unsortedMarks[j];
                        largestDist = distance;
                    }
                }
            }

            //Verify found 4 pairs while two of those are identical
            for (int i = 0; i < 4; i++)
            {
                CircleF expectedMark = unsortedMarks[i];
                CircleF keyMark = pairs[unsortedMarks[i]];
                CircleF valueMark = pairs[keyMark];
                if (!valueMark.Equals(expectedMark))
                    throw new CalibrationException("Pairs Calculated Wrong!");
            }

            //Find and Remove duplicates
            List<CircleF> keysToRemove = new List<CircleF>();
            foreach (CircleF mark in pairs.Keys)
                if (!keysToRemove.Contains(pairs[mark]))
                    keysToRemove.Add(mark);

            foreach (CircleF key in keysToRemove)
                pairs.Remove(key);

            return pairs;
        }

        /// <summary>
        /// Verifies exactly 4 calibration marks found
        /// </summary>
        /// <param name="circles">Calibration marks circles</param>
        /// <exception cref="CalibrationException">Thrown in case not exactly 4 calibration marks found</exception>
        public void VerifyMarksFound(List<CircleF> circles)
        {
            if (circles.Count != 4)
            {
                throw new CalibrationException(String.Format(
                    "Number of marks found in calibration is [{0}], while expected [4] marks.", circles.Count));
            }
        }

        /// <summary>
        /// Update Calibration Marks in order to get better coverage
        /// </summary>
        /// <param name="calibrationMarks">CalibrationMarks to update</param>
        /// <returns>Updated marks</returns>
        public Dictionary<eCallibrationMark, CircleF> UpdateCoverage(Dictionary<eCallibrationMark, CircleF> calibrationMarks)
        {
            //Create the updated marks based on marks radius
            Dictionary<eCallibrationMark, CircleF> updatedMaks = new Dictionary<eCallibrationMark, CircleF>();
            foreach (var mark in calibrationMarks)
            {
                switch (mark.Key)
                {
                    case eCallibrationMark.BL:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X - mark.Value.Radius,
                            mark.Value.Center.Y + mark.Value.Radius), mark.Value.Radius));
                        break;
                    case eCallibrationMark.BR:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X + mark.Value.Radius,
                            mark.Value.Center.Y + mark.Value.Radius), mark.Value.Radius));
                        break;
                    case eCallibrationMark.TL:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X - mark.Value.Radius,
                            mark.Value.Center.Y - mark.Value.Radius), mark.Value.Radius));
                        break;
                    case eCallibrationMark.TR:
                        updatedMaks.Add(mark.Key, new CircleF(new System.Drawing.PointF(mark.Value.Center.X + mark.Value.Radius,
                            mark.Value.Center.Y - mark.Value.Radius), mark.Value.Radius));
                        break;
                }
            }

            //set updated marks
            foreach (var mark in updatedMaks)
                calibrationMarks[mark.Key] = mark.Value;

            return calibrationMarks;
        }

        /// <summary>
        /// Set transformation matrices in Transformer to be used in all further calculations
        /// Both input parameters are length between calibration marks
        /// </summary>
        /// <param name="axeXlength">Table X Axe length in Points (PTS)</param>
        /// <param name="axeYlength">Table Y Axe length in Points (PTS)</param>
        /// <param name="calibrationMarks">Calibration marks detected on image</param>
        public void SetTransformationMatrix(int axeXlength, int axeYlength, Dictionary<eCallibrationMark, CircleF> calibrationMarks)
        {
            _markCoordinates = new Dictionary<eCallibrationMark, System.Drawing.PointF>();
            _markCoordinates.Add(eCallibrationMark.BL, new System.Drawing.PointF(0, axeYlength));             //ButtomLeft
            _markCoordinates.Add(eCallibrationMark.TL, new System.Drawing.PointF(0, 0));                         //TopLeft
            _markCoordinates.Add(eCallibrationMark.TR, new System.Drawing.PointF(axeXlength, 0));               //TopRight
            _markCoordinates.Add(eCallibrationMark.BR, new System.Drawing.PointF(axeXlength, axeYlength));   //ButtomRight

            System.Drawing.PointF[] orriginalPointArray = _markCoordinates.Values.ToArray();
            System.Drawing.PointF[] arrangedPoints = new System.Drawing.PointF[4];

            arrangedPoints[0] = calibrationMarks[eCallibrationMark.BL].Center;
            arrangedPoints[1] = calibrationMarks[eCallibrationMark.TL].Center;
            arrangedPoints[2] = calibrationMarks[eCallibrationMark.TR].Center;
            arrangedPoints[3] = calibrationMarks[eCallibrationMark.BR].Center;

            //Used to perform transformations and store transformation matrices for further calculations
            Transformation transformer = new Transformation();
            transformer.FindHomographyMatrix(arrangedPoints, orriginalPointArray);
        }

        /// <summary>
        /// Show calibration mark on screen - circle and coordinates
        /// </summary>
        /// <param name="mark">Calibration mark as CircleF</param>
        /// <param name="key">Calibration mark type</param>
        public void ShowCalibrationMark(CircleF mark, eCallibrationMark key)
        {
            switch (key)
            {
                case eCallibrationMark.BL:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BL,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.TL:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.TL,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.TR:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.TR,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.BR:
                    Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BR,
                        new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
            }
        }
    }
}
