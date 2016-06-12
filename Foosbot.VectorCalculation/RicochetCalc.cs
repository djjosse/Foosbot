// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using EasyLog;
using Foosbot.Common.Enums;
using Foosbot.Common.Exceptions;
using Foosbot.Common.Logs;
using Foosbot.Common.Protocols;
using Foosbot.VectorCalculation.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Foosbot.VectorCalculation
{
    /// <summary>
    /// Ricochet Calculation Class
    /// </summary>
    public class RicochetCalc : IInitializableRicochet
    {
        #region Private Members

        protected double _xMinBorder;
        protected double _yMinBorder;
        protected double _xMaxBorder;
        protected double _yMaxBorder;
        protected double _ricocheFactor;

        #endregion Private Members

        #region Constructor

        /// <summary>
        /// Ricochet Calculator Constructor
        /// </summary>
        /// <param name="doInit">Perform initialization flag [default is false]</param>
        /// <param name="units">Desired units to work in [default is Points]</param>
        public RicochetCalc(bool doInit = false, eUnits units = eUnits.Pts)
        {
            IsInitialized = false;
            if (doInit)
            {
                InitializeRicochetCalc(units);
            }
        }

        #endregion Constructor

        #region IInitializable Implementation

        /// <summary>
        /// Is Initialized property
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initialization Method
        /// </summary>
        public void Initialize()
        {
            InitializeRicochetCalc();
        }

        /// <summary>
        /// Initialization Method - reads all constants from Configuration file
        /// </summary>
        /// <param name="units">Desired units to work in [default is Points]</param>
        /// <param name="xMaxBorder">X Max Limit for Ricochet Calculations</param>
        /// <param name="yMaxBorder">Y Max Limit for Ricochet Calculations</param>
        /// <param name="ricochetFactor">Ricochet Factor to use in calculations</param>
        public void InitializeRicochetCalc(eUnits units = eUnits.Pts, double xMaxBorder = -1, double yMaxBorder = -1, double ricochetFactor = -1)
        {
            if (!IsInitialized)
            {
                _xMinBorder = 0;
                _yMinBorder = 0;
                _ricocheFactor = (ricochetFactor > 0) ? ricochetFactor
                            : Configuration.Attributes.GetValue<double>(Configuration.Names.KEY_RICOCHET_FACTOR);
                switch(units)
                {
                    case eUnits.Mm:
                        _xMaxBorder = (xMaxBorder > 0) ? xMaxBorder 
                            : Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_WIDTH);
                        _yMaxBorder = (yMaxBorder > 0) ? yMaxBorder
                            : Configuration.Attributes.GetValue<double>(Configuration.Names.TABLE_HEIGHT);
                        break;
                    case eUnits.Pts:
                        _xMaxBorder = (xMaxBorder > 0) ? xMaxBorder
                            : Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_X_SIZE);
                        _yMaxBorder = (yMaxBorder > 0) ? yMaxBorder
                            : Configuration.Attributes.GetValue<double>(Configuration.Names.FOOSBOT_AXE_Y_SIZE);
                        break;
                    default:
                        throw new NotSupportedException(String.Format(
                            "[{0}] Units [{1}] are not supported by RicochetCalc.",
                                MethodBase.GetCurrentMethod().Name, units.ToString()));

                }
                IsInitialized = true;
            }
        }

        #endregion IInitializable Implementation

        #region IRicochet Properties Implementation

        /// <summary>
        /// Minimum border in x
        /// </summary>
        public double MinBorderX
        {
            get
            {
                if (IsInitialized)
                    return _xMinBorder;
                else
                    throw new InitializationException(String.Format(
                        "[{0}] Class must be initialized before used", GetType().Name));
            }
        }

        /// <summary>
        /// Minimum border in y
        /// </summary>
        public double MinBorderY
        {
            get
            {
                if (IsInitialized)
                    return _yMinBorder;
                else
                    throw new InitializationException(String.Format(
                        "[{0}] Class must be initialized before used", GetType().Name));
            }
        }

        /// <summary>
        /// Maxmimum border in x
        /// </summary>
        public double MaxBorderX
        {
            get
            {
                if (IsInitialized)
                    return _xMaxBorder;
                else
                    throw new InitializationException(String.Format(
                        "[{0}] Class must be initialized before used", GetType().Name));
            }
        }

        /// <summary>
        /// Maximum border in x
        /// </summary>
        public double MaxBorderY
        {
            get
            {
                if (IsInitialized)
                    return _yMaxBorder;
                else
                    throw new InitializationException(String.Format(
                        "[{0}] Class must be initialized before used", GetType().Name));
            }
        }

        /// <summary>
        /// Ricochet factor
        /// </summary>
        public double RicochetFactor
        {
            get
            {
                if (IsInitialized)
                    return _ricocheFactor;
                else
                    throw new InitializationException(String.Format(
                        "[{0}] Class must be initialized before used", GetType().Name));
            }
        }

        #endregion IRicochet Properties Implementation

        #region Method Implementation

        /// <summary>
        /// Ricochet coordinate calculation
        /// </summary>
        /// <param name="ballCoordinates">Ball Coordinates</param>
        /// <returns>Ricochet Ball Coordinates</returns>
        public BallCoordinates Ricochet(BallCoordinates ballCoordinates)
        {
            try
            {
                Coordinates2D intersectionPoint = FindNearestIntersectionPoint(ballCoordinates);

                //Marks.DrawRicochetMark(Convert.ToInt32(intersectionPoint.X), Convert.ToInt32(intersectionPoint.Y), true);
                
                DateTime ricocheTime = FindRicochetTime(ballCoordinates, intersectionPoint);
                
                Vector2D vector = FindIntersectionVector(ballCoordinates.Vector, intersectionPoint);
                
                BallCoordinates coordinates = new BallCoordinates(
                        Convert.ToInt32(intersectionPoint.X),
                        Convert.ToInt32(intersectionPoint.Y), ricocheTime);
                coordinates.Vector = vector;

                return coordinates;
            }
            catch (Exception e)
            {
                throw new NotSupportedException(String.Format(
                    "[{0}] Failed to find ricochet coordinates. Reason: {1}",
                        MethodBase.GetCurrentMethod().Name, e.Message));
            }
        }

        /// <summary>
        /// Find nearest intersection point with table borders based on
        /// - given ball coordinates and vector
        /// </summary>
        /// <param name="ballCoordinates">Defined coordinates with defined vector</param>
        /// <returns>Coordinates of intersection with border</returns>
        public Coordinates2D FindNearestIntersectionPoint(BallCoordinates ballCoordinates)
        {
            //verify we can proceed to calculate intersection
            VerifyBallCoordinatesAndVectorInput(ballCoordinates);

            Dictionary<Vector2D, Coordinates2D> borderIntersection = new Dictionary<Vector2D, Coordinates2D>();
            List<Vector2D> vectors = new List<Vector2D>();

            //get line slope
            double m = CalculateLineSlope(ballCoordinates.Vector);

            if (ballCoordinates.Vector.X != 0)
            {
                double yb = CalculateY2OnLine(m, ballCoordinates.X, ballCoordinates.Y, MinBorderX);
                double yc = CalculateY2OnLine(m, ballCoordinates.X, ballCoordinates.Y, MaxBorderX);

                Coordinates2D B = new Coordinates2D(MinBorderX, yb);
                Vector2D vB = new Vector2D(B.X - ballCoordinates.X, B.Y - ballCoordinates.Y);
                vectors.Add(vB);
                borderIntersection.Add(vB, B); //B

                Coordinates2D C = new Coordinates2D(MaxBorderX, yc);
                Vector2D vC = new Vector2D(C.X - ballCoordinates.X, C.Y - ballCoordinates.Y);
                vectors.Add(vC);
                borderIntersection.Add(vC, C); //C
            }

            if (ballCoordinates.Vector.Y != 0)
            {
                double xa = CalculateX2OnLine(m, ballCoordinates.X, ballCoordinates.Y, MinBorderY);
                double xd = CalculateX2OnLine(m, ballCoordinates.X, ballCoordinates.Y, MaxBorderY);

                Coordinates2D A = new Coordinates2D(xa, MinBorderY);
                Vector2D vA = new Vector2D(A.X - ballCoordinates.X, A.Y - ballCoordinates.Y);
                vectors.Add(vA);
                borderIntersection.Add(vA, A); //A

                Coordinates2D D = new Coordinates2D(xd, MaxBorderY);
                Vector2D vD = new Vector2D(D.X - ballCoordinates.X, D.Y - ballCoordinates.Y);
                vectors.Add(vD);
                borderIntersection.Add(vD, D); //D
            }


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
                Log.Print("No vectors found!", eCategory.Error, LogTag.VECTOR);

            //get nearest point
            Coordinates2D intersectionPoint = null;
            double minDistance = MaxBorderX * MaxBorderY;
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
            VerifyBallCoordinatesAndVectorInput(ballCoordinates);

            if (intersection == null || !intersection.IsDefined)
                throw new NotSupportedException(String.Format("[{0}] Intersection coordinates undefined!", MethodBase.GetCurrentMethod().Name));

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
            if (vector == null)
                throw new NotSupportedException(String.Format(
                    "[{0}] Vector from Intersection point can not be found because last known vector is NULL",
                        MethodBase.GetCurrentMethod().Name));

            if (!vector.IsDefined)
                throw new NotSupportedException(String.Format(
                    "[{0}] Vector from Intersection point can not be found because last known vector is undefined",
                        MethodBase.GetCurrentMethod().Name));

            if (!intersection.IsDefined)
                throw new NotSupportedException(String.Format(
                    "[{0}] Vector from Intersection point can not be found because intersection point is undefined",
                        MethodBase.GetCurrentMethod().Name));

            bool isDirectionChanged = false;
            double x = vector.X * RicochetFactor;
            double y = vector.Y * RicochetFactor;

            if (intersection.Y == MinBorderY || intersection.Y == MaxBorderY)
            {
                y *= (-1);
                isDirectionChanged = true;
            }
            if (intersection.X == MinBorderX || intersection.X == MaxBorderX)
            {
                x *= (-1);
                isDirectionChanged = true;
            }
            if (!isDirectionChanged)
                throw new NotSupportedException(
                    String.Format("[{0}] Intersection point must be on border! Current point is {1}x{2}",
                        MethodBase.GetCurrentMethod().Name, intersection.X, intersection.Y));

            return new Vector2D(x, y);
        }

        #endregion Method Implementation

        #region Private Member Functions

        /// <summary>
        /// Calculate X2 from: Y2-Y1= m(X2-X1)
        /// </summary>
        /// <param name="m">Line Slope</param>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="y2">Y2</param>
        /// <returns>X2</returns>
        private double CalculateX2OnLine(double m, double x1, double y1, double y2)
        {
            if (m == 0) return x1;
            return (y2 - y1) / m + x1;
        }

        /// <summary>
        /// Calculate Y2 from: Y2-Y1= m(X2-X1)
        /// </summary>
        /// <param name="m">Line Slope</param>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <returns>Y2</returns>
        private double CalculateY2OnLine(double m, double x1, double y1, double x2)
        {
            return m * (x2 - x1) + y1;
        }

        /// <summary>
        /// Calculate line slope
        /// </summary>
        /// <param name="vector">Vector to calculate line slope from</param>
        /// <returns>Line Slope</returns>
        private double CalculateLineSlope(Vector2D vector)
        {
            if (vector.X == 0 || vector.Y == 0)
                return 0;

            return vector.Y / vector.X;
        }

        /// <summary>
        /// Verify coordinates and vector are defined, not null and not both vector x and y are 0
        /// </summary>
        /// <param name="ballCoordinates">Ball Coordianates with vector</param>
        /// <exception cref="NotSupportedException">Thrown in case vector or coordinates are not defined or are NULL, or both vector x and y are 0</exception>
        private void VerifyBallCoordinatesAndVectorInput(BallCoordinates ballCoordinates)
        {
            if (ballCoordinates == null)
                throw new NotSupportedException(String.Format("[{0}] Intersection point can not be found because ball coordinates are NULL", MethodBase.GetCurrentMethod().Name));

            if (!ballCoordinates.IsDefined)
                throw new NotSupportedException(String.Format("[{0}] Intersection point can not be found because ball coordinates are not defined", MethodBase.GetCurrentMethod().Name));

            if (ballCoordinates.Vector == null)
                throw new NotSupportedException(String.Format("[{0}] Intersection point can not be found because vector is NULL", MethodBase.GetCurrentMethod().Name));

            if (!ballCoordinates.Vector.IsDefined)
                throw new NotSupportedException(String.Format("[{0}] Intersection point can not be found because vector is undefined", MethodBase.GetCurrentMethod().Name));

            if (ballCoordinates.Vector.X == 0 && ballCoordinates.Vector.Y == 0)
                throw new NotSupportedException(String.Format("[{0}] Intersection point can not be found because vector is 0x0", MethodBase.GetCurrentMethod().Name));
        }

        #endregion Private Member Functions
    } 
}
