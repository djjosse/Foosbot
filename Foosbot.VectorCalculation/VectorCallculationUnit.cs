using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation
{
    public class VectorCallculationUnit : Observer<BallCoordinates>
    {
        Helpers.UpdateMarkupLineDelegate UpdateMarkupLineRealWorld;
        Helpers.UpdateMarkupCircleDelegate UpdateMarkupCircle;

        public VectorCallculationUnit(Publisher<BallCoordinates> coordinatesPublisher, Helpers.UpdateMarkupLineDelegate onUpdateMarkupLine, Helpers.UpdateMarkupCircleDelegate onUpdateMarkup) :
            base(coordinatesPublisher)
        {
            UpdateMarkupLineRealWorld = onUpdateMarkupLine;
            UpdateMarkupCircle = onUpdateMarkup;
        }

        public override void Job()
        {
            _publisher.Dettach(this);
            BallCoordinates ballCoordinates = _publisher.Data;

            //TODO: implement vector calculation here
            //...

            //example of updating UI
            UpdateLineInUI(ballCoordinates.X, ballCoordinates.Y, 
                    ballCoordinates.X + 50, ballCoordinates.Y + 50);

            _publisher.Attach(this);
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

            System.Drawing.PointF guiStartPoint = Transformation.InvertTransform(startPoint);
            System.Drawing.PointF guiEndPoint = Transformation.InvertTransform(endPoint);

            UpdateMarkupLineRealWorld(Helpers.eMarkupKey.BALL_VECTOR,
                new System.Windows.Point(guiStartPoint.X, guiStartPoint.Y),
                new System.Windows.Point(guiEndPoint.X, guiEndPoint.Y));
        }
    }
}
