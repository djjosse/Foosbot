using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    public static class ImageProcessingHelpers
    {
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
