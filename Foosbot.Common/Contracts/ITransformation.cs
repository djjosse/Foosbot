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
using System.Drawing;

namespace Foosbot.Common.Contracts
{
    /// <summary>
    /// Interface for class performing and storing transformation data
    /// </summary>
    public interface ITransformation : IInitializable
    {
        /// <summary>
        /// Transformation Matrix
        /// </summary>
        Matrix<double> Matrix { get; }

        /// <summary>
        /// Invert Matrix of Transformation Matrix 
        /// </summary>
        Matrix<double> InvertMatrix { get; }

        /// <summary>
        /// Perform transformation on given point
        /// </summary>
        /// <param name="point">Input Point to transform</param>
        /// <returns>Output transformed point</returns>
        PointF Transform(PointF point);

        /// <summary>
        /// Perform transformation on given point
        /// </summary>
        /// <param name="point">Input Point to transform</param>
        /// <returns>Output transformed point</returns>
        System.Windows.Point Transform(System.Windows.Point point);

        /// <summary>
        /// Perform transformation on given point
        /// </summary>
        /// <param name="inX">Input Point X coordinate to transform</param>
        /// <param name="inY">Input Point Y coordinate to transform</param>
        /// <param name="outX">Output Point X after transformation</param>
        /// <param name="outY">Output Point Y after transformation</param>
        void Transform(double inX, double inY, out double outX, out double outY);

        /// <summary>
        /// Perform invert transformation on given point
        /// </summary>
        /// <param name="point">Input Point to perform invert transform</param>
        /// <returns>Output invert-transformed point</returns>
        PointF InvertTransform(PointF point);

        /// <summary>
        /// Perform invert transformation on given point
        /// </summary>
        /// <param name="point">Input Point to perform invert transform</param>
        /// <returns>Output invert-transformed point</returns>
        System.Windows.Point InvertTransform(System.Windows.Point point);

        /// <summary>
        /// Perform invert transformation on given point
        /// </summary>
        /// <param name="inX">Input Point X coordinate to perform invert transform</param>
        /// <param name="inY">Input Point Y coordinate to perform invert transform</param>
        /// <param name="outX">Output Point X after performed invert transform</param>
        /// <param name="outY">Output Point Y after performed invert transform</param>
        void InvertTransform(double inX, double inY, out double outX, out double outY);

        /// <summary>
        /// Initialize Transformations calculates transformation and invert matrices based
        /// on provided transformed and original points
        /// </summary>
        /// <param name="transformedPoints">Transformaed points</param>
        /// <param name="originalPoints">Original points</param>
        void Initialize(PointF[] transformedPoints, PointF[] originalPoints);
    }
}
