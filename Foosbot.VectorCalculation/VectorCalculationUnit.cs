// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Reflection;
using System.Windows;
using Foosbot.Common;
using System.Threading;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.VectorCalculation.Contracts;
using Foosbot.Common.Data;

namespace Foosbot.VectorCalculation
{
    public class VectorCalculationUnit : Observer<BallCoordinates>
    {
        #region Constants

        private readonly double D_ERR;
        private readonly double ALPHA_ERR;

        #endregion Constants

        /// <summary>
        /// Ball Location Publisher
        /// This inner object is a publisher for vector calculation unit 
        /// </summary>
        public BallLocationPublisher LastBallLocationPublisher { get; protected set; }

        /// <summary>
        /// Coordinates Publisher for Next Foosbot Pipeline element (Decision Unit)
        /// </summary>
        public ILastBallCoordinatesUpdater _coordinatesUpdater;

        /// <summary>
        /// Coordinates Stabilizer (Shake removing) Utility
        /// </summary>
        public CoordinatesStabilizer _stabilizer;

        /// <summary>
        /// Ricochet Calculation Utility
        /// </summary>
        private RicochetCalc vectorUtils;

        /// <summary>
        /// Image Data from Image Processing Unit
        /// </summary>
        private IImageData _imagingData;

        /// <summary>
        /// Last known ball coordinates from previous iteration
        /// </summary>
        private BallCoordinates _storedBallCoordinates;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinatesPublisher">Coordinates Publisher (Image processing Unit)</param>
        /// <param name="imagingData">Image Data from Image Processing Unit</param>
        public VectorCalculationUnit(Publisher<BallCoordinates> coordinatesPublisher, IImageData imagingData) :
            base(coordinatesPublisher)
        {
            vectorUtils = new RicochetCalc();
            vectorUtils.Initialize();

            _imagingData = imagingData;

            _coordinatesUpdater = new BallCoordinatesUpdater();
            LastBallLocationPublisher = new BallLocationPublisher(_coordinatesUpdater);

            _storedBallCoordinates = new BallCoordinates(DateTime.Now);
            _storedBallCoordinates.Vector = new Vector2D();

            D_ERR = Configuration.Attributes.GetValue<double>(Configuration.Names.VECTOR_CALC_DISTANCE_ERROR);
            ALPHA_ERR = Configuration.Attributes.GetValue<double>(Configuration.Names.VECTOR_CALC_ANGLE_ERROR);
        }

        /// <summary>
        /// Main Method of Vector Calculation Unit
        /// </summary>
        public override void Job()
        {
            try
            {
                _publisher.Detach(this);

                if (_stabilizer == null)
                {
                    int ballRadius = (_imagingData.BallRadius < 0) ? 5 : _imagingData.BallRadius;
                    _stabilizer = new CoordinatesStabilizer(ballRadius);
                }

                //Get data and remove noise
                BallCoordinates newCoordinates = _publisher.Data;
                BallCoordinates ballCoordinates = _stabilizer.Stabilize(newCoordinates, _storedBallCoordinates);

                //Draw coordinates from stabilizer
                if (ballCoordinates.IsDefined)
                {
                    double x, y;
                    TransformAgent.Data.InvertTransform(ballCoordinates.X, ballCoordinates.Y, out x, out y);
                }
                else
                {
                    //Delete old locations
                    Marks.DrawBall(new System.Windows.Point(0, 0), 0);
                }
                

                ballCoordinates.Vector = VectorCalculationAlgorithm(ballCoordinates);

                if (ballCoordinates.IsDefined && ballCoordinates.Vector.IsDefined)
                {
                    try
                    {
                        (_coordinatesUpdater as BallCoordinatesUpdater).LastBallCoordinates = ballCoordinates;
                        LastBallLocationPublisher.UpdateAndNotify();

                        Marks.DrawBallVector(new Point(ballCoordinates.X, ballCoordinates.Y),
                            new Point(Convert.ToInt32(ballCoordinates.Vector.X), Convert.ToInt32(ballCoordinates.Vector.Y)));
                    }
                    catch (Exception e)
                    {
                        Log.Common.Error(String.Format("[{0}] {1} [{2}]", MethodBase.GetCurrentMethod().Name, e.Message, ballCoordinates.ToString()));
                    }
                }
                else
                {
                   Marks.DrawBallVector(new Point(0,0), new Point(0, 0), false);
                }
            }
            catch (ThreadInterruptedException)
            {
                /* new data received */
            }
            catch (Exception e)
            {
                Log.Common.Error(String.Format("[{0}] Error in vector calculation. Reason: {1}", MethodBase.GetCurrentMethod().Name, e.Message));
            }
            finally
            {
                _publisher.Attach(this);
            }
        }

        /// <summary>
        /// Vector Calculation Algorithm
        /// </summary>
        /// <param name="ballCoordinates"></param>
        /// <returns></returns>
        private Vector2D VectorCalculationAlgorithm(BallCoordinates ballCoordinates)
        {
            //verify ball coordinates
            if (ballCoordinates == null)
            {
                throw new ArgumentException(String.Format(
                    "[{0}] Ball coordinates are null we are unable to calculate vector",
                        MethodBase.GetCurrentMethod().Name));
            }

            //create undefined vector in case we can't calculate vector
            Vector2D vector = new Vector2D();

            //calculate new vector if possible
            if (ballCoordinates.IsDefined && _storedBallCoordinates.IsDefined)
            {
                vector = CalculateVector(ballCoordinates);
            }

            //update stored ball coordinates
            _storedBallCoordinates = ballCoordinates;

            //return calculated vector
            return vector;
        }

        /// <summary>
        /// Calculate Vector Method
        /// </summary>
        /// <param name="ballCoordinates"></param>
        /// <param name="maxAngleError"></param>
        /// <returns></returns>
        private Vector2D CalculateVector(BallCoordinates ballCoordinates, double maxAngleError = 1.0)
        {
            if (ballCoordinates.Timestamp == _storedBallCoordinates.Timestamp)
            {
                Log.Common.Error(String.Format("[{0}] Current ball coordinates and stored are with same time stamp!", MethodBase.GetCurrentMethod().Name));
                return new Vector2D();
            }
            else
            {
                //find basic vector
                double deltaT = (ballCoordinates.Timestamp - _storedBallCoordinates.Timestamp).TotalSeconds;// / 100;
                double x = ballCoordinates.X - _storedBallCoordinates.X;
                double y = ballCoordinates.Y - _storedBallCoordinates.Y;
                ballCoordinates.Vector = new Vector2D(x / deltaT, y / deltaT);

                //no movement in a new vector OR 
                //stored vector is undefined OR
                //no movement in the old vector
                if (ballCoordinates.Vector.Velocity() == 0 ||
                    !_storedBallCoordinates.Vector.IsDefined ||
                    _storedBallCoordinates.Vector.Velocity() == 0)
                {
                    _storedBallCoordinates.Vector = ballCoordinates.Vector;
                    return ballCoordinates.Vector;
                }

                //Calculate cos of angle of scalar product
                double scalarProductDevider = _storedBallCoordinates.Vector.Velocity() *
                        ballCoordinates.Vector.Velocity();
                double cosAlpha = (_storedBallCoordinates.Vector.X * ballCoordinates.Vector.X +
                                   _storedBallCoordinates.Vector.Y * ballCoordinates.Vector.Y) /
                                   scalarProductDevider;

                double minLimit = (1 - ALPHA_ERR) * 1/maxAngleError;
                double maxLimit = (1 + ALPHA_ERR) * maxAngleError;

                if (!((minLimit < cosAlpha) && (cosAlpha < maxLimit)))
                {
                    BallCoordinates intersection = vectorUtils.Ricochet(ballCoordinates);
                    _storedBallCoordinates = ballCoordinates;
                    return CalculateVector(intersection, maxAngleError * 1.2);
                }
                else
                {
                    _storedBallCoordinates.Vector = ballCoordinates.Vector;
                    return ballCoordinates.Vector;
                }
            }
        }
    }
}
