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

namespace Foosbot.VectorCalculation
{
    public class VectorCallculationUnit : Observer<BallCoordinates>
    {
        /// <summary>
        /// Ball Location Publisher
        /// This inner object is a publisher for vector calculation unit 
        /// </summary>
        public BallLocationPublisher LastBallLocationPublisher { get; protected set; }

        public ILastBallCoordinatesUpdater _coordinatesUpdater;

        public VectorCallculationUnit(Publisher<BallCoordinates> coordinatesPublisher) :
            base(coordinatesPublisher)
        {
            _coordinatesUpdater = new BallCoordinatesUpdater();
            LastBallLocationPublisher = new BallLocationPublisher(_coordinatesUpdater);

            _storedBallCoordinates = new BallCoordinates(DateTime.Now);
            _storedBallCoordinates.Vector = new Vector2D();

            D_ERR = Configuration.Attributes.GetValue<double>(Configuration.Names.VECTOR_CALC_DISTANCE_ERROR);
            ALPHA_ERR = Configuration.Attributes.GetValue<double>(Configuration.Names.VECTOR_CALC_ANGLE_ERROR);
        }

        public override void Job()
        {
            _publisher.Dettach(this);
            BallCoordinates ballCoordinates = _publisher.Data;

            ballCoordinates.Vector = VectorCalculationAlgorithm(ballCoordinates);

            //TODO: implement vector calculation here
            //...
            //Marks.DrawBall(new Point(100, 100), 20, true);
            //Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BL, new System.Windows.Point(200, 200), 50);
                 
            if (ballCoordinates.IsDefined && ballCoordinates.Vector.IsDefined)
            {
                try
                {
                    (_coordinatesUpdater as BallCoordinatesUpdater).LastBallCoordinates  = ballCoordinates;
                    LastBallLocationPublisher.UpdateAndNotify();

                    Marks.DrawBallVector(new Point(ballCoordinates.X, ballCoordinates.Y), 
                        new Point(Convert.ToInt32(ballCoordinates.Vector.X), Convert.ToInt32(ballCoordinates.Vector.Y)), true);
                }
                catch(Exception e)
                {
                    Log.Common.Error(String.Format("[{0}] {1} [{2}]", MethodBase.GetCurrentMethod().Name, e.Message, ballCoordinates.ToString()));
                }
            }
            _publisher.Attach(this);
        }

        private readonly double D_ERR;
        private readonly double ALPHA_ERR;

        private BallCoordinates _storedBallCoordinates;

        private Vector2D VectorCalculationAlgorithm(BallCoordinates ballCoordinates)
        {
            if (ballCoordinates.IsDefined)
            {
                if (_storedBallCoordinates.IsDefined)
                {

                    Vector2D vector = CalculateVector(ballCoordinates);
                    _storedBallCoordinates = ballCoordinates;
                    return vector;
                }
                else
                {
                    _storedBallCoordinates = ballCoordinates;
                    //ToDo: think of handling better undefined coordinates
                    return new Vector2D();
                }
            }
            else //ball coordinates undefined
            {
                //ToDo: think of handling better undefined coordinates
                return new Vector2D();
            }
        }

        static int counter = 0;
        private Vector2D CalculateVector(BallCoordinates ballCoordinates, double maxAngleError = 1.0)
        {
            if (maxAngleError == 1.0) counter = 0;
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
                counter++;
                //if (counter>3)
                //{
                    Log.Common.Debug("counter:" + counter);
                //}
            //    Log.Common.Debug(
            //        String.Format("[{0}] Current angle is {1}",MethodBase.GetCurrentMethod().Name, 
            //                                                   Math.Acos(cosAlpha).ToDegrees(2)));
                  VectorUtils utils = new VectorUtils();
                  utils.Initialize();
                  BallCoordinates intersection = utils.Ricochet(ballCoordinates);
                  _storedBallCoordinates = ballCoordinates;
                  //ballCoordinates = intersection;
                  return CalculateVector(intersection, maxAngleError * 1.2);
            //      if (intersection != null && intersection.Vector != null)
            //    {
            //        return intersection.Vector;
            //    }
            //    else return new Vector2D();
            }
            else
            {
                _storedBallCoordinates.Vector = ballCoordinates.Vector;
                return ballCoordinates.Vector;
            }
        }
    }
}
