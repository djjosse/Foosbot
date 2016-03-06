using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Foosbot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Foosbot.ImageProcessing
{
    public class CalibrationUnit : Detector, ICallibration
    {
        #region Callibration Constants

        /// <summary>
        /// Approx. Inner Radius of Callibration Circle
        /// </summary>
        public const int INNER_RADIUS = 30;//60;

        /// <summary>
        /// Approx. Outer Radius of Callibration Circle
        /// </summary>
        public const int OUTER_RADIUS = 50;//80;

        /// <summary>
        /// Possible error for inner and outer radiuses
        /// </summary>
        public const double RADIUS_THRESHOLD = 1;

        /// <summary>
        /// Possible Error for distance between inner and outer mark radius
        /// </summary>
        public const double DISTANCE_ERROR = 0.1;

        public const int FRAMES_TO_SKIP = 10;

        #endregion Callibration Constants

        #region private members

        private Dictionary<eCallibrationMark, System.Drawing.PointF> _markCoordinates;

        private Image<Gray, byte> _phaseOneFrame;

        private int _skipFrames = 0;

        #endregion private members

        #region ICallibration

        /// <summary>
        /// Current Callibration State
        /// </summary>
        public eCallibrationState CallibrationState { get; private set; }

        /// <summary>
        /// Sorted Callibration Marks Coordinates on original image
        /// </summary>
        public Dictionary<eCallibrationMark, CircleF> CallibrationMarks { get; set; }

        /// <summary>
        /// Transformation Matrix calcullated on callibration
        /// </summary>
        public Matrix<double> TransformationMatrix { get; set; }

        /// <summary>
        /// Invert Matrix for Transformation Matrix calcullated on callibration
        /// </summary>
        public Matrix<double> InvertMatrix { get; set; }

        /// <summary>
        /// Background Image Found in callibration
        /// </summary>
        public Image<Gray, Byte> Background { get; set; }

        /// <summary>
        /// Ball Radius
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        /// Ball Radius Error Rate
        /// </summary>
        public double ErrorRate { get; set; }

        #endregion ICallibration

        /// <summary>
        /// Callibration Unit Constructor
        /// </summary>
        /// <param name="onUpdateMarkup">Delegate for updating markups</param>
        /// <param name="onUpdateStatistics">Delegate for updating statistics</param>
        public CalibrationUnit(Helpers.UpdateMarkupDelegate onUpdateMarkup, Helpers.UpdateStatisticsDelegate onUpdateStatistics)
            : base(onUpdateMarkup, onUpdateStatistics) { }

        /// <summary>
        /// Start callibration process
        /// </summary>
        /// <param name="source">Image to perform callibration on it</param>
        /// <returns>[True] if callibration finished, [False] otherwise</returns>
        public bool InvokeCallibration(Image<Gray, byte> source)
        {
            switch (CallibrationState)
            {
                case eCallibrationState.NotStarted:
                    CallibrationPhaseI(source);
                    break;
                case eCallibrationState.FinishedPhaseI:
                    CallibrationPhaseII(source);
                    break;
                case eCallibrationState.Finished:
                    Log.Image.Debug("Callibration already finished.");
                    break;
            }
            return CallibrationState.Equals(eCallibrationState.Finished);
        }

        /// <summary>
        /// First Phase of callibration
        /// * Skip unstable frames
        /// * Detect Callibration Circles and sort those
        /// * Callculate Transformation Homography and Invert matrix
        /// * Calcullate Ball Radius and possible Error
        /// </summary>
        /// <param name="source">Image from camera to perform callibration on</param>
        private void CallibrationPhaseI(Image<Gray, byte> source)
        {
            //ignore first frames
            if (_skipFrames < FRAMES_TO_SKIP)
            {
                _skipFrames++;
            }
            else
            {
                _skipFrames = 0;

                Log.Image.Debug("Starting callibration Phase I...");
                _phaseOneFrame = source.Clone();

                Image<Gray, byte> image = _phaseOneFrame.Clone();

                //Remove Noise from picture
                CvInvoke.Canny(image, image, 100, 60);
                image = NoiseRemove(image);

                //Find Callibration Marks
                List<CircleF> circles = FindCallibrationMarks(image);

                if (circles.Count == 4)
                {
                    //Sort Callibration Marks and set value to property
                    SortCallibrationMarks(circles);

                    ShowAllCallibrationMarks();
                    StringBuilder str = new StringBuilder("4 callibration Marks found and sorted: \n\t\t\t\t");
                    foreach (var mark in CallibrationMarks)
                        str.Append(String.Format("{0}:[{1}x{2}] ", mark.Key, mark.Value.Center.X, mark.Value.Center.Y));
                    Log.Image.Info(str.ToString());

                    //Calculate Homography Matrix
                    int axeXLength = Configuration.Attributes.GetValue<int>("axeX");
                    int axeYLength = Configuration.Attributes.GetValue<int>("axeY");
                    TransformationMatrix = FindTransformationMatrix(axeXLength, axeYLength);
                    InvertMatrix = new Matrix<double>(3, 3);
                    CvInvoke.Invert(TransformationMatrix, InvertMatrix, DecompMethod.LU);
                    Log.Image.Info("Homography matrix calculated.");

                    //Calculate Ball Radius and Error
                    double originalBallRadius = Configuration.Attributes.GetValue<double>("BallDiameter") / 2;
                    CalculateBallRadiusAndError((float)originalBallRadius);
                    Log.Image.Info(String.Format("Expected ball radius is {0} +/- {1}", Radius, ErrorRate));

                    //Check Calculated Radius
                    //ToDo:
                    //UpdateMarkup(Helpers.eMarkupKey.BALL_CIRCLE_MARK, new Point(300, 300), Radius);

                    CallibrationState = eCallibrationState.FinishedPhaseI;
                }
                else
                {
                    Log.Image.Warning("Unable to find 4 corresponding circles in phase I");
                }
            }
        }

        /// <summary>
        /// Second Phase of callibration
        /// * Skip unstable frames
        /// * Find Callibration Circles
        /// * Extract Background Image
        /// * Crop background Image based o callibration marks
        /// </summary>
        /// <param name="source">Image from camera to perform callibration on</param>
        private void CallibrationPhaseII(Image<Gray, byte> source)
        {
            if (_skipFrames < FRAMES_TO_SKIP)
            {
                _skipFrames++;
            }
            else
            {
                _skipFrames = 0;

                Log.Image.Debug("Starting callibration Phase II...");
                Image<Gray, byte> image = source.Clone();

                Image<Gray, byte> tempImage = image.Clone();

                //Remove Noise from picture
                CvInvoke.Canny(tempImage, tempImage, 100, 60);
                tempImage = NoiseRemove(tempImage);

                //Find Callibration Marks
                List<CircleF> circles = FindCallibrationMarks(tempImage);

                if (circles.Count == 4)
                {
                    Log.Image.Debug("Found callibration marks!");

                    //Extract Background
                    Background = image.Clone();//.Sub(_phaseOneFrame);
                   // image.Save("test//" + DateTime.Now.ToString("HH_mm_ss_fff") + "image.png");
                    //_phaseOneFrame.Save("test//" + DateTime.Now.ToString("HH_mm_ss_fff") + "_phaseOneFrame.png");
                    //Background.Save("test//" + DateTime.Now.ToString("HH_mm_ss_fff") + "back.png");
                    Log.Image.Debug("Background image extracted");

                    //Crop background based on callibration marks
                    Background = CropAndStoreOffset(Background, CallibrationMarks.Values.ToList());

                    Log.Image.Debug("Background image cropped");

                    Log.Image.Info("Callibration finished!");

                    CallibrationState = eCallibrationState.Finished;
                }
                else
                {
                    Log.Image.Warning("Unable to find 4 callibration circles in phase II.");
                }
            }
        }

        /// <summary>
        /// Find Callibration Marks on image
        /// </summary>
        /// <param name="image">Source image to find circles on</param>
        /// <returns>List of callibration marks as circles</returns>
        private List<CircleF> FindCallibrationMarks(Image<Gray, byte> image)
        {
            List<CircleF> circles = new List<CircleF>();

            //Find Callibration Big Circles
            CircleF[] possibleInnerCircles = DetectCircles(image, INNER_RADIUS, RADIUS_THRESHOLD, (double)OUTER_RADIUS * 5
                , 180.0, 120.0, 2.0);

            //Find Callibration Small Circles
            CircleF[] possibleOuterCircles = DetectCircles(image, OUTER_RADIUS, RADIUS_THRESHOLD, (double)OUTER_RADIUS * 5
                , 180.0, 120.0, 2.0);

            //Find corresponding circles
            foreach (CircleF innerCircle in possibleInnerCircles)
            {
                foreach (CircleF outerCircle in possibleOuterCircles)
                {
                    if (Utils.Distance(innerCircle.Center, outerCircle.Center) < DISTANCE_ERROR * OUTER_RADIUS)
                    {
                        circles.Add(innerCircle);
                    }
                }
            }

            return circles;
        }

        /// <summary>
        /// Sort marks and fill the property
        /// CallibrationMarks
        /// </summary>
        /// <param name="unsortedMarks">Unsorted callibration marks list</param>
        private void SortCallibrationMarks(List<CircleF> unsortedMarks)
        {
            if (unsortedMarks.Count != 4)
                throw new NotSupportedException("To find diagonal mark pairs exactly 4 marks must be detected!");

            CallibrationMarks = new Dictionary<eCallibrationMark, CircleF>();

            Dictionary<CircleF, CircleF> diagonalPairs = FindDiagonalMarkPairs(unsortedMarks);

            //In each pair find left point, right, buttom and top and set corresponding marks
            foreach (CircleF key in diagonalPairs.Keys)
            {
                CircleF buttom = (key.Center.Y > diagonalPairs[key].Center.Y) ? key : diagonalPairs[key];
                CircleF top = (key.Center.Y < diagonalPairs[key].Center.Y) ? key : diagonalPairs[key];
                CircleF left = (key.Center.X < diagonalPairs[key].Center.X) ? key : diagonalPairs[key];
                CircleF right = (key.Center.X > diagonalPairs[key].Center.X) ? key : diagonalPairs[key];

                if (buttom.Equals(left))
                    CallibrationMarks.Add(eCallibrationMark.ButtomLeft, left);
                if (top.Equals(left))
                    CallibrationMarks.Add(eCallibrationMark.TopLeft, left);
                if (top.Equals(right))
                    CallibrationMarks.Add(eCallibrationMark.TopRight, right);
                if (buttom.Equals(right))
                    CallibrationMarks.Add(eCallibrationMark.ButtomRight, right);
            }
        }

        /// <summary>
        /// Find Diagonal pairs of detected callibration marks.
        /// Based on assumption those pairs have largest distance.
        /// </summary>
        /// <param name="unsortedMarks">Unsorted marks list</param>
        /// <returns>Dictionary of 2 diagonal pairs of marks</returns>
        private Dictionary<CircleF, CircleF> FindDiagonalMarkPairs(List<CircleF> unsortedMarks)
        {
            //Find All Pairs based on distance
            Dictionary<CircleF, CircleF> pairs = new Dictionary<CircleF, CircleF>();

            //Go over all unsorted marks 
            for (int i = 0; i < 4; i++)
            {
                //Add mark to dictionary as key and add as value mark with largest distance
                pairs.Add(unsortedMarks[i], default(CircleF));
                double largestDist = 0;
                for (int j = 0; j < 4; j++)
                {
                    double distance = Utils.Distance(unsortedMarks[i].Center, unsortedMarks[j].Center);
                    if (i != j && distance > largestDist)
                    {
                        pairs[unsortedMarks[i]] = unsortedMarks[j];
                        largestDist = distance;
                    }
                }
            }

            //Verify found 4 pairs while two of those are identical
            for (int i = 0; i < 4; i++)
            {
                CircleF expectedMark = unsortedMarks[i];
                CircleF keyMark = pairs[unsortedMarks[i]];
                CircleF valueMark = pairs[keyMark];
                if (!valueMark.Equals(expectedMark))
                    throw new Exception("Pairs Calculated Wrong!");
            }

            //Find and Remove duplicates
            List<CircleF> keysToRemove = new List<CircleF>();
            foreach (CircleF mark in pairs.Keys)
                if (!keysToRemove.Contains(pairs[mark]))
                    keysToRemove.Add(mark);

            foreach (CircleF key in keysToRemove)
                pairs.Remove(key);

            return pairs;
        }

        /// <summary>
        /// Find Homography matrix based on callibration marks
        /// </summary>
        /// <param name="axeXlength">Length of X axe in world of Foosbot</param>
        /// <param name="axeYlength">Length of Y axe in world of Foosbot</param>
        /// <returns>Homograpgy Matrix</returns>
        private Matrix<double> FindTransformationMatrix(int axeXlength, int axeYlength)
        {
            _markCoordinates = new Dictionary<eCallibrationMark, System.Drawing.PointF>();
            _markCoordinates.Add(eCallibrationMark.ButtomLeft, new System.Drawing.PointF(0, axeYlength));             //ButtomLeft
            _markCoordinates.Add(eCallibrationMark.TopLeft, new System.Drawing.PointF(0, 0));                         //TopLeft
            _markCoordinates.Add(eCallibrationMark.TopRight, new System.Drawing.PointF(axeXlength, 0));               //TopRight
            _markCoordinates.Add(eCallibrationMark.ButtomRight, new System.Drawing.PointF(axeXlength, axeYlength));   //ButtomRight

            System.Drawing.PointF[] orriginalPointArray = _markCoordinates.Values.ToArray();
            System.Drawing.PointF[] arrangedPoints = new System.Drawing.PointF[4];

            arrangedPoints[0] = CallibrationMarks[eCallibrationMark.ButtomLeft].Center;
            arrangedPoints[1] = CallibrationMarks[eCallibrationMark.TopLeft].Center;
            arrangedPoints[2] = CallibrationMarks[eCallibrationMark.TopRight].Center;
            arrangedPoints[3] = CallibrationMarks[eCallibrationMark.ButtomRight].Center;

            Matrix<double> matrix = new Matrix<double>(3, 3);
            CvInvoke.FindHomography(arrangedPoints, orriginalPointArray, matrix, HomographyMethod.Default);
            return matrix;
        }

        /// <summary>
        /// Show all callibration marks on screen
        /// </summary>
        private void ShowAllCallibrationMarks()
        {
            foreach (KeyValuePair<eCallibrationMark, CircleF> mark in CallibrationMarks)
            {
                ShowCallibrationMark(mark.Value, mark.Key);
            }
        }

        /// <summary>
        /// Show callibration mark on screen - circle and coordinates
        /// </summary>
        /// <param name="mark">Callibration mark as CircleF</param>
        /// <param name="key">Caliibration mark type</param>
        private void ShowCallibrationMark(CircleF mark, eCallibrationMark key)
        {
            switch (key)
            {
                case eCallibrationMark.ButtomLeft:
                    UpdateMarkup(Helpers.eMarkupKey.BUTTOM_LEFT_CALLIBRATION_MARK,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    UpdateMarkup(Helpers.eMarkupKey.BUTTOM_LEFT_CALLIBRATION_TEXT,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.TopLeft:
                    UpdateMarkup(Helpers.eMarkupKey.TOP_LEFT_CALLIBRATION_MARK,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    UpdateMarkup(Helpers.eMarkupKey.TOP_LEFT_CALLIBRATION_TEXT,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.TopRight:
                    UpdateMarkup(Helpers.eMarkupKey.TOP_RIGHT_CALLIBRATION_MARK,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    UpdateMarkup(Helpers.eMarkupKey.TOP_RIGHT_CALLIBRATION_TEXT,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
                case eCallibrationMark.ButtomRight:
                    UpdateMarkup(Helpers.eMarkupKey.BUTTOM_RIGHT_CALLIBRATION_MARK,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    UpdateMarkup(Helpers.eMarkupKey.BUTTOM_RIGHT_CALLIBRATION_TEXT,
                            new Point(mark.Center.X, mark.Center.Y), Convert.ToInt32(mark.Radius));
                    break;
            }

        }

        /// <summary>
        /// Callculate ball radius and possible error 
        /// </summary>
        /// <param name="origRadius">Original Ball Radius in mm</param>
        private void CalculateBallRadiusAndError(float origRadius)
        {
            double minRadius = origRadius * 100;
            double maxRadius = 0;
            foreach (System.Drawing.PointF start in _markCoordinates.Values)
            {
                System.Drawing.PointF end = new System.Drawing.PointF(start.X + origRadius, start.Y);

                double check = Utils.Distance(start, end);

                System.Drawing.PointF transformedStart = ImageProcessingHelpers.ApplyTransfromation(InvertMatrix, start);
                System.Drawing.PointF transformedEnd = ImageProcessingHelpers.ApplyTransfromation(InvertMatrix, end);

                double radius = Utils.Distance(transformedStart, transformedEnd);

                if (radius > maxRadius)
                    maxRadius = radius;
                if (radius < minRadius)
                    minRadius = radius;
            }

            Radius = Convert.ToInt32((minRadius + maxRadius) / 2);
            double possibleError = ((maxRadius - minRadius) / 2) + 1;
            ErrorRate = possibleError / Radius;
        }
    }
}
