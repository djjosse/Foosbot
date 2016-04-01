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
using Emgu.CV.CvEnum;
using System;
using System.Drawing;

namespace Foosbot
{
    /// <summary>
    /// Transformation Performer Class
    /// </summary>
    public class Transformation
    {
        private static Matrix<double> _matrix;
        private static Matrix<double> _invertMatrix;
        private static bool _isInitialized = false;

        private static Matrix<double> Matrix
        {
            get
            {
                if (_isInitialized)
                    return _matrix;
                else
                    throw new NotSupportedException(
                        "Transformation matrix was not initialized and can not be used!");
            }
            set
            {
                _matrix = value;
            }
        }

        private static Matrix<double> InvertMatrix
        { 
            get
            {
                if (_isInitialized)
                    return _invertMatrix;
                else
                    throw new NotSupportedException(
                        "Transformation matrix was not initialized and can not be used!");
            }
            set
            {
                _invertMatrix = value;
            }
        }

        public PointF Transform(PointF point)
        {
            double x, y;
            Transform(point.X, point.Y, out x, out y);
            return new PointF(Convert.ToSingle(x), Convert.ToSingle(y));
        }

        public System.Windows.Point Transform(System.Windows.Point point)
        {
            double x, y;
            Transform(point.X, point.Y, out x, out y);
            return new System.Windows.Point(x, y);
        }

        public void Transform(double inX, double inY, out double outX, out double outY)
        {
            double[,] vector = {   {inX}, {inY}, {1.0} };
            Matrix<double> X = new Matrix<double>(vector);
            Matrix<double> Y = Matrix * X;
            outX = Y.Data[0, 0] / Y.Data[2, 0];
            outY = Y.Data[1, 0] / Y.Data[2, 0];
        }

        public PointF InvertTransform(PointF point)
        {
            double x, y;
            InvertTransform(point.X, point.Y, out x, out y);
            return new PointF(Convert.ToSingle(x), Convert.ToSingle(y));
        }

        public System.Windows.Point InvertTransform(System.Windows.Point point)
        {
            double x, y;
            InvertTransform(point.X, point.Y, out x, out y);
            return new System.Windows.Point(x, y);
        }

        public void InvertTransform(double inX, double inY, out double outX, out double outY)
        {
            double[,] vector = { { inX }, { inY }, { 1.0 } };
            Matrix<double> X = new Matrix<double>(vector);
            Matrix<double> Y = InvertMatrix * X;
            outX = Y.Data[0, 0] / Y.Data[2, 0];
            outY = Y.Data[1, 0] / Y.Data[2, 0];
        }

        public void FindHomographyMatrix(PointF[] transformedPoints, PointF[] originalPoints)
        {
            _isInitialized = true;
            Matrix = new Matrix<double>(3, 3);
            CvInvoke.FindHomography(transformedPoints, originalPoints, Matrix, HomographyMethod.Default);
            InvertMatrix = new Matrix<double>(3, 3);
            CvInvoke.Invert(Matrix, InvertMatrix, DecompMethod.LU);
        }
    }
}
