using Foosbot.Common.Protocols;
using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation
{
    public class VectorUtils
    {
        public VectorUtils()
        {
            if (!_isInitilized)
                Initialize();
        }

        
        private void Initialize()
        {
            x0 = 0;
            y0 = 0;
            xMax = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
            yMax = Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
            _isInitilized = true;
        }

        private static double x0;
        private static double y0;
        private static double xMax;
        private static double yMax;
        private static bool _isInitilized = false;

        public BallCoordinates Ricochet(BallCoordinates ballCoordinates)
        {
            Coordinates2D intersectionPoint = FindNearestIntersectionPoint(ballCoordinates);
            


            if (intersectionPoint != null)
            {
                Log.Common.Info("Point " + intersectionPoint.X + " " + intersectionPoint.Y);

                System.Drawing.PointF point = new System.Drawing.PointF(Convert.ToSingle(intersectionPoint.X), Convert.ToSingle(intersectionPoint.Y));

                Transformation transformer = new Transformation();
                System.Drawing.PointF guiPoint = transformer.InvertTransform(point);

                //Marks.DrawBall(new System.Windows.Point(guiPoint.X, guiPoint.Y), 10);
                //Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BL, new System.Windows.Point(guiPoint.X, guiPoint.Y), 10);
                Marks.DrawRicochetMark(Convert.ToInt32(guiPoint.X), Convert.ToInt32(guiPoint.Y));
            }
            return null;
        }

        /// <summary>
        /// Find nearest intersection point with table borders based on
        /// - given ball coordinates and vector
        /// </summary>
        /// <param name="ballCoordinates">Defined coordinates with defined vector</param>
        /// <returns>Coordinates of intersection with border</returns>
        public Coordinates2D FindNearestIntersectionPoint(BallCoordinates ballCoordinates)
        {
            Dictionary<Vector2D, Coordinates2D> borderIntersection = new Dictionary<Vector2D, Coordinates2D>();
            List<Vector2D> vectors = new List<Vector2D>();

            //find all possible points if both vector X and Y components are bigger than 0 => 4 points
            if (ballCoordinates.Vector.X != 0 || ballCoordinates.Vector.Y != 0)
            {
                double m = (ballCoordinates.Vector.X != 0) ? 0 : 1;
                if (ballCoordinates.Vector.X != 0 && ballCoordinates.Vector.Y != 0 ) 
                    m = ballCoordinates.Vector.Y / ballCoordinates.Vector.X;

                if (ballCoordinates.Vector.X != 0)
                {
                    double yb = m * (0 - ballCoordinates.X) + ballCoordinates.Y;
                    double yc = m * (xMax - ballCoordinates.X) + ballCoordinates.Y;

                    Coordinates2D B = new Coordinates2D(0, yb);
                    Vector2D vB = new Vector2D(B.X - ballCoordinates.X, B.Y - ballCoordinates.Y);
                    vectors.Add(vB);
                    borderIntersection.Add(vB, B); //B

                    Coordinates2D C = new Coordinates2D(xMax, yc);
                    Vector2D vC = new Vector2D(C.X - ballCoordinates.X, C.Y - ballCoordinates.Y);
                    vectors.Add(vC);
                    borderIntersection.Add(vC, C); //C
                }

                if (ballCoordinates.Vector.Y != 0)
                {
                    double xa = (0 - ballCoordinates.Y) / m + ballCoordinates.X;
                    double xd = (yMax - ballCoordinates.Y) / m + ballCoordinates.X;

                    Coordinates2D A = new Coordinates2D(xa, 0);
                    Vector2D vA = new Vector2D(A.X - ballCoordinates.X, A.Y - ballCoordinates.Y);
                    vectors.Add(vA);
                    borderIntersection.Add(vA, A); //A

                    Coordinates2D D = new Coordinates2D(xd, yMax);
                    Vector2D vD = new Vector2D(D.X - ballCoordinates.X, D.Y - ballCoordinates.Y);
                    vectors.Add(vD);
                    borderIntersection.Add(vD, D); //D
                }
            }
            //no possible points if both vector X and Y components are 0
            else // (ballCoordinates.Vector.X == 0 && ballCoordinates.Vector.Y == 0)
                return null;

            //remove points from wrong direction
            foreach (Vector2D vector in vectors)
            {
                double direction = vector.ScalarProduct(ballCoordinates.Vector)
                    / (vector.Velocity() * ballCoordinates.Vector.Velocity());
                if (Math.Round(direction, 0) != 1)
                    borderIntersection.Remove(vector);
            }

            //if no points found => Error, consider adding error rate threshold
            if (borderIntersection.Count < 1)
                Log.Common.Error(String.Format("[{0}] No vectors found!", MethodBase.GetCurrentMethod().Name));

            //get nearest point
            Coordinates2D intersectionPoint = null;
            double minDistance = xMax * yMax;
            foreach (Coordinates2D intersection in borderIntersection.Values)
            {
                double dist = intersection.Distance(ballCoordinates);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    intersectionPoint = intersection;
                }
            }

            return intersectionPoint;
        }

        public static int ScalarProduct(DefinableCartesianCoordinate<int> coordA, DefinableCartesianCoordinate<int> coordB)
        {
            return coordA.X * coordB.X + coordA.Y * coordB.Y;
        }
    }
}
