// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Foosbot.VectorCalculation
{
    public class VectorUtils
    {
        //public static VectorUtils()
        //{
        //    RICOCHET_FACTOR = Configuration.Attributes.GetValue<double>(Configuration.Names.KEY_RICOCHET_FACTOR);
        //}

        public VectorUtils()
        {
            if (!_isInitilized)
            {
                Initialize();
            }
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
        private static readonly double RICOCHET_FACTOR
            = Configuration.Attributes.GetValue<double>(Configuration.Names.KEY_RICOCHET_FACTOR);
        private static bool _isInitilized = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ballCoordinates"></param>
        /// <returns></returns>
        public BallCoordinates Ricochet(BallCoordinates ballCoordinates)
        {
            try
            {
                Coordinates2D intersectionPoint = FindNearestIntersectionPoint(ballCoordinates);
                if (intersectionPoint == null) return null;
                Marks.DrawRicochetMark(Convert.ToInt32(intersectionPoint.X), Convert.ToInt32(intersectionPoint.Y), true);
                
                DateTime ricocheTime = FindRicochetTime(ballCoordinates, intersectionPoint);
                Vector2D vector = FindIntersectionVector(ballCoordinates.Vector, intersectionPoint);
                BallCoordinates coordinates = new BallCoordinates(
                        Convert.ToInt32(intersectionPoint.X),
                        Convert.ToInt32(intersectionPoint.Y), ricocheTime);
                coordinates.Vector = vector;

                Marks.DrawBallVector(new System.Windows.Point(intersectionPoint.X, intersectionPoint.Y),
                                     new System.Windows.Point(vector.X, vector.Y), true);

               // return null;
                return coordinates;
            }
            catch (Exception e)
            {
                Log.Common.Error(String.Format("[{0}] {1} {2}", MethodBase.GetCurrentMethod().Name, e.GetType().Name, e.Message));
                return null;
            }

            //if (intersectionPoint != null)
            //{
            //    double distance = ballCoordinates.Distance(intersectionPoint);
            //    double velocity = ballCoordinates.Vector.Velocity();
            //    double deltaT = distance / velocity;
            //    DateTime ricocheTime = new DateTime();
            //    try
            //    {
            //        ricocheTime = ballCoordinates.Timestamp + TimeSpan.FromSeconds(deltaT);
            //    }
            //    catch(Exception e)
            //    {
            //        Log.Common.Error(String.Format("[{0}] Error in addition time. Stamp: {1} Delta T: {2}",
            //            MethodBase.GetCurrentMethod().Name, ballCoordinates.Timestamp.ToString("HH:mm:ss.fff"), deltaT));
            //        return null;
            //    }

            //    Log.Common.Debug(String.Format("[{0}] Dist: {1} From: {2} To: {3}",
            //        MethodBase.GetCurrentMethod().Name, Math.Round(distance,2), ballCoordinates.Timestamp.ToString("HH:mm:ss.fff"), ricocheTime.ToString("HH:mm:ss.fff")));

            //    double xVector = ballCoordinates.Vector.X * RICOCHET_FACTOR;
            //    double yVector = ballCoordinates.Vector.Y * RICOCHET_FACTOR;
            //    if (intersectionPoint.Y == y0 || intersectionPoint.Y == yMax)
            //        yVector *= -1;
            //    if (intersectionPoint.X == x0 || intersectionPoint.X == xMax)
            //        xVector *= -1;


            //    Log.Common.Info("Point " + intersectionPoint.X + " " + intersectionPoint.Y);

            //    System.Drawing.PointF point = new System.Drawing.PointF(Convert.ToSingle(intersectionPoint.X), Convert.ToSingle(intersectionPoint.Y));

            //    Transformation transformer = new Transformation();
            //    System.Drawing.PointF guiPoint = transformer.InvertTransform(point);

            //    //Marks.DrawBall(new System.Windows.Point(guiPoint.X, guiPoint.Y), 10);
            //    //Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BL, new System.Windows.Point(guiPoint.X, guiPoint.Y), 10);
            //    Marks.DrawRicochetMark(Convert.ToInt32(guiPoint.X), Convert.ToInt32(guiPoint.Y));

            //    BallCoordinates coordinates =
            //        new BallCoordinates(
            //        Convert.ToInt32(intersectionPoint.X),
            //        Convert.ToInt32(intersectionPoint.Y), ricocheTime);
            //    coordinates.Vector = new Vector2D(xVector, yVector);

            //    return coordinates;
            //}
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

        /// <summary>
        /// Find intersection time based on ball coordinates, timestamp, vector and intersection point.
        /// </summary>
        /// <param name="ballCoordinates">Ball coordinates before intersection with border</param>
        /// <param name="intersection">Intersection with border point</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown in case calculated intersection time is too big</exception>
        /// <exception cref="NotSupportedException">Thrown in case intersection coordinates undefined.</exception>
        /// <returns>Intersection timestamp</returns>
        public DateTime FindRicochetTime(BallCoordinates ballCoordinates, Coordinates2D intersection)
        {
            if (intersection == null || !intersection.IsDefined)
                throw new NotSupportedException("Intersection coordinates undefined!");

            double distance = ballCoordinates.Distance(intersection);
            double velocity = ballCoordinates.Vector.Velocity();
            double deltaT = distance / velocity;
            return ballCoordinates.Timestamp + TimeSpan.FromSeconds(deltaT);
        }

        /// <summary>
        /// Calculate intersection vector from intersection point
        /// </summary>
        /// <param name="vector">Last known vector before intersection</param>
        /// <param name="intersection">Intersection point with border</param>
        /// <exception cref="NotSupportedException">Thrown in case passed intersection point is not on the border</exception>
        /// <returns>Ball Vector after intersection</returns>
        public Vector2D FindIntersectionVector(Vector2D vector, Coordinates2D intersection)
        {
            bool isDirectionChanged = false;
            double x = vector.X * RICOCHET_FACTOR;
            double y = vector.Y * RICOCHET_FACTOR;

            if (intersection.Y == y0 || intersection.Y == yMax)
            {
                y *= (-1);
                isDirectionChanged = true;
            }
            if (intersection.X == x0 || intersection.X == xMax)
            {
                x *= (-1);
                isDirectionChanged = true;
            }
            if (!isDirectionChanged)
                throw new NotSupportedException(
                    String.Format("Intersection point must be on border! Current point is {0}x{1}",
                        intersection.X, intersection.Y));

            return new Vector2D(x, y);
        }

        public static int ScalarProduct(DefinableCartesianCoordinate<int> coordA, DefinableCartesianCoordinate<int> coordB)
        {
            return coordA.X * coordB.X + coordA.Y * coordB.Y;
        }
    }
}
