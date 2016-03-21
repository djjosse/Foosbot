using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foosbot.VectorCalculation
{
    public class VectorCallculationUnit : Observer<BallCoordinates>
    {
        public VectorCallculationUnit(Publisher<BallCoordinates> coordinatesPublisher) :
            base(coordinatesPublisher)
        {
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
           // Marks.DrawBall(new Point(100, 100), 20, true);
           // Marks.DrawCallibrationCircle(Foosbot.Common.Protocols.eCallibrationMark.BL, new System.Windows.Point(200, 200), 50);
                 
            //example of updating UI

            if (ballCoordinates.IsDefined && ballCoordinates.Vector.IsDefined)
            {
                try
                {
                    UpdateLineInUI(ballCoordinates.X, ballCoordinates.Y,
                            ballCoordinates.X + Convert.ToInt32(ballCoordinates.Vector.X),
                            ballCoordinates.Y + Convert.ToInt32(ballCoordinates.Vector.Y));
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

        private Vector2D CalculateVector(BallCoordinates ballCoordinates)
        {

            double deltaT = (ballCoordinates.Timestamp - _storedBallCoordinates.Timestamp).TotalMilliseconds / 100;
            double x = ballCoordinates.X - _storedBallCoordinates.X;
            double y = ballCoordinates.Y - _storedBallCoordinates.Y;
            ballCoordinates.Vector = new Vector2D(x / deltaT, y / deltaT);

            if (_storedBallCoordinates.Vector.IsDefined &&
                ballCoordinates.Vector.Velocity() > 0)
            {
                double cosAlpha = (_storedBallCoordinates.Vector.X * ballCoordinates.Vector.X
                                    + _storedBallCoordinates.Vector.Y * ballCoordinates.Vector.Y)
                                    / (_storedBallCoordinates.Vector.Velocity() * ballCoordinates.Vector.Velocity());
                //if (!((1 - ALPHA_ERR < cosAlpha) && (cosAlpha < 1 + ALPHA_ERR)))
                //{
                //    Log.Common.Warning("NEED RICOSHET");
                //    VectorUtils utils = new VectorUtils();
                //    utils.Ricochet(_storedBallCoordinates);
                //    return new Vector2D();
                //}
            }
            _storedBallCoordinates.Vector = ballCoordinates.Vector;
            return ballCoordinates.Vector;
        }

        /// <summary>
        /// Updates UI after transfroming point
        /// </summary>
        /// <param name="startX">Vector start point X</param>
        /// <param name="startY">Vector start point Y</param>
        /// <param name="endX">Vector end point X</param>
        /// <param name="endY">Vector end point Y</param>
        public void UpdateLineInUI(float startX, float startY, float endX, float endY)
        {
            System.Drawing.PointF startPoint = new System.Drawing.PointF(startX, startY);
            System.Drawing.PointF endPoint = new System.Drawing.PointF(endX, endY);

            Transformation transformer = new Transformation();
            System.Drawing.PointF guiStartPoint = transformer.InvertTransform(startPoint);
            System.Drawing.PointF guiEndPoint = transformer.InvertTransform(endPoint);

            Marks.DrawBallVector(new System.Windows.Point(guiStartPoint.X, guiStartPoint.Y),
                new System.Windows.Point(guiEndPoint.X - guiStartPoint.X, guiEndPoint.Y - guiStartPoint.Y));
           

            //UpdateMarkupLineRealWorld(Helpers.eMarkupKey.BALL_VECTOR,
            //        new System.Windows.Point(guiStartPoint.X, guiStartPoint.Y),
            //          new System.Windows.Point(guiEndPoint.X, guiEndPoint.Y));
        }
    }
}
