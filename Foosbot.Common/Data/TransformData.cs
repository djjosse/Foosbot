// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;
using Foosbot.Common.Exceptions;

namespace Foosbot.Common.Data
{
    /// <summary>
    /// Transformation Data Unit
    /// </summary>
    public class TransformData : ITransformation
    {
        /// <summary>
        /// Transformation Matrix
        /// </summary>
        private Matrix<double> _matrix;

        /// <summary>
        /// Invert Matrix of Transformation matrix
        /// </summary>
        private Matrix<double> _invertMatrix;

        /// <summary>
        /// Initialization Flag
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// Transformation Matrix
        /// </summary>
        public Matrix<double> Matrix
        {
            get
            {
                if (IsInitialized)
                    return _matrix;
                else
                    throw new InitializationException(
                        "Transformation matrix was not initialized and can not be used!");
            }
        }

        /// <summary>
        /// Invert Matrix of Transformation Matrix 
        /// </summary>
        public Matrix<double> InvertMatrix
        {
            get
            {
                if (IsInitialized)
                    return _invertMatrix;
                else
                    throw new InitializationException(
                        "Transformation matrix was not initialized and can not be used!");
            }
        }

        /// <summary>
        /// Perform transformation on given point
        /// </summary>
        /// <param name="point">Input Point to transform</param>
        /// <returns>Output transformed point</returns>
        public PointF Transform(PointF point)
        {
            double x, y;
            Transform(point.X, point.Y, out x, out y);
            return new PointF(Convert.ToSingle(x), Convert.ToSingle(y));
        }

        /// <summary>
        /// Perform transformation on given point
        /// </summary>
        /// <param name="point">Input Point to transform</param>
        /// <returns>Output transformed point</returns>
        public System.Windows.Point Transform(System.Windows.Point point)
        {
            double x, y;
            Transform(point.X, point.Y, out x, out y);
            return new System.Windows.Point(x, y);
        }

        /// <summary>
        /// Perform transformation on given point
        /// </summary>
        /// <param name="inX">Input Point X coordinate to transform</param>
        /// <param name="inY">Input Point Y coordinate to transform</param>
        /// <param name="outX">Output Point X after transformation</param>
        /// <param name="outY">Output Point Y after transformation</param>
        public void Transform(double inX, double inY, out double outX, out double outY)
        {
            double[,] vector = { { inX }, { inY }, { 1.0 } };
            Matrix<double> X = new Matrix<double>(vector);
            Matrix<double> Y = Matrix * X;
            outX = Y.Data[0, 0] / Y.Data[2, 0];
            outY = Y.Data[1, 0] / Y.Data[2, 0];
        }

        /// <summary>
        /// Perform invert transformation on given point
        /// </summary>
        /// <param name="point">Input Point to perform invert transform</param>
        /// <returns>Output invert-transformed point</returns>
        public PointF InvertTransform(PointF point)
        {
            double x, y;
            InvertTransform(point.X, point.Y, out x, out y);
            return new PointF(Convert.ToSingle(x), Convert.ToSingle(y));
        }

        /// <summary>
        /// Perform invert transformation on given point
        /// </summary>
        /// <param name="point">Input Point to perform invert transform</param>
        /// <returns>Output invert-transformed point</returns>
        public System.Windows.Point InvertTransform(System.Windows.Point point)
        {
            double x, y;
            InvertTransform(point.X, point.Y, out x, out y);
            return new System.Windows.Point(x, y);
        }

        /// <summary>
        /// Perform invert transformation on given point
        /// </summary>
        /// <param name="inX">Input Point X coordinate to perform invert transform</param>
        /// <param name="inY">Input Point Y coordinate to perform invert transform</param>
        /// <param name="outX">Output Point X after performed invert transform</param>
        /// <param name="outY">Output Point Y after performed invert transform</param>
        public void InvertTransform(double inX, double inY, out double outX, out double outY)
        {
            double[,] vector = { { inX }, { inY }, { 1.0 } };
            Matrix<double> X = new Matrix<double>(vector);
            Matrix<double> Y = InvertMatrix * X;
            outX = Y.Data[0, 0] / Y.Data[2, 0];
            outY = Y.Data[1, 0] / Y.Data[2, 0];
        }

        /// <summary>
        /// Is Initialized property
        /// </summary>
        public bool IsInitialized
        {
            get 
            {
                return _isInitialized;
            }
        }

        /// <summary>
        /// Initialize - sets initialization flag to true
        /// </summary>
        public void Initialize()
        {
            if (!_isInitialized)
                _isInitialized = true;
        }

        /// <summary>
        /// Initialize Transformations calculates transformation and invert matrices based
        /// on provided transformed and original points
        /// </summary>
        /// <param name="transformedPoints">Transformaed points</param>
        /// <param name="originalPoints">Original points</param>
        /// <exception cref="InitializationException">Thrown in case input array lenghts are not equal or less than 3</exception>
        public void Initialize(PointF[] transformedPoints, PointF[] originalPoints)
        {
            if (transformedPoints.Length < 3)
                throw new InitializationException("Unable to initialize Transformation Data! "
                    +"Transformation and original points arrays must have at least 3 elements in order to find homography matrix!");

            if (transformedPoints.Length != originalPoints.Length)
                throw new InitializationException("Unable to initialize Transformation Data! " 
                    +"Transformation and original points arrays must have same number of elements in order to find homography matrix!");

            _matrix = FindHomographyMatrix(transformedPoints, originalPoints);
            _invertMatrix = FindInvertMatrix(_matrix);
            Initialize();
        }

        /// <summary>
        /// Calculate Homography Matrix
        /// </summary>
        /// <param name="transformedPoints">Transfomed points array</param>
        /// <param name="originalPoints">Original points array</param>
        /// <returns>Homography matrix</returns>
        private Matrix<double> FindHomographyMatrix(PointF[] transformedPoints, PointF[] originalPoints)
        {
            Matrix<double> homographyMatrix = new Matrix<double>(3, 3);
            _isInitialized = true;
            CvInvoke.FindHomography(transformedPoints, originalPoints, homographyMatrix, HomographyMethod.Default);
            return homographyMatrix;
        }

        /// <summary>
        /// Find Invert Matrix for given matrix
        /// </summary>
        /// <param name="matrix">Original Matrix</param>
        /// <returns>Invert Matrix</returns>
        private Matrix<double> FindInvertMatrix(Matrix<double> matrix)
        {
            Matrix<double> invertMatrix = new Matrix<double>(3, 3);
            CvInvoke.Invert(matrix, invertMatrix, DecompMethod.LU);
            return invertMatrix;
        }
    }
}
