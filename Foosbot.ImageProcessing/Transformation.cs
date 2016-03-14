using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Transformation Performer Class
    /// </summary>
    public static class Transformation
    {
        private static Matrix<double> _matrix;
        private static Matrix<double> _invertMatrix;

        private static Matrix<double> Matrix
        {
            get
            {
                if (_isInitialized)
                    return _matrix;
                else
                    throw new Exception("Transformation static class was not initialized and can not be used!");
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
                    throw new Exception("Transformation static class was not initialized and can not be used!");
            }
            set
            {
                _invertMatrix = value;
            }
        }

        private static bool _isInitialized = false;

        public static void Initialize(Matrix<double> matrix, Matrix<double> invertMatrix)
        {
            Log.Image.Info(String.Format("[{0}] Initializing transformation helper.", MethodBase.GetCurrentMethod().Name));
            _matrix = matrix;
            _invertMatrix = matrix;
            _isInitialized = true;
        }

        public static PointF Transform(PointF point)
        {
            return ApplyTransfromation(Matrix, point);
        }

        public static PointF InvertTransform(PointF point)
        {
            return ApplyTransfromation(InvertMatrix, point);
        }

        public static PointF ApplyTransfromation(Matrix<double> T, PointF point)
        {
            //Convert given point to vector X 
            double[,] vector = {   {point.X},
                                    {point.Y},
                                    {1.0} };
            Matrix<double> X = new Matrix<double>(vector);

            //Multiply matrixes
            Matrix<double> Y = T * X;

            //Return calculated location as point
            return new PointF((float)Y.Data[0, 0] / (float)Y.Data[2, 0], (float)Y.Data[1, 0] / (float)Y.Data[2, 0]);
        }
    }
}
