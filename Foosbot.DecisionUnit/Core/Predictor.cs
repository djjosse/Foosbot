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
using Foosbot.DecisionUnit.Contracts;
using Foosbot.VectorCalculation;
using Foosbot.VectorCalculation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    public class Predictor : IPredictor
    {
        /// <summary>
        /// Surveyor for calculating and verify table size vs. coordinates
        /// </summary>
        private ISurveyor _surveyor;

        /// <summary>
        /// Ricochet Calc Unit instance for calculating Ricochet
        /// </summary>
        private IInitializableRicochet _ricochetCalc;

        /// <summary>
        /// Predictor class constructor
        /// </summary>
        /// <param name="surveyor">Surveyor for calculating and verify table size vs. coordinates</param>
        /// <param name="ricochetCalc">Ricochet Calc Unit instance for calculating ricochet</param>
        public Predictor(ISurveyor surveyor, IInitializableRicochet ricochetCalc)
        {
            _surveyor = surveyor;
            _ricochetCalc = ricochetCalc;
        }

        /// <summary>
        /// Calculate Ball Future Coordinates in actual time system can responce
        /// </summary>
        /// <param name="currentCoordinates"><Current ball coordinates/param>
        /// <param name="actionTime">Actual system responce time</param>
        /// <returns>Ball Future coordinates</returns>
        public BallCoordinates FindBallFutureCoordinates(BallCoordinates currentCoordinates, DateTime actionTime)
        {
            if (currentCoordinates == null || !currentCoordinates.IsDefined)
                throw new ArgumentException(String.Format(
                    "[{0}] Unable to calculate ball future coordinates while current coordinates are null or undefined",
                        MethodBase.GetCurrentMethod().Name));

            if (actionTime < currentCoordinates.Timestamp)
                throw new ArgumentException(String.Format(
                    "[{0}] Unable to calculate ball future coordinates while action time is earlier than time stamp",
                        MethodBase.GetCurrentMethod().Name));

            BallCoordinates bfc;

            if (currentCoordinates.Vector == null || !currentCoordinates.Vector.IsDefined)
            {
                bfc = new BallCoordinates(currentCoordinates.X, currentCoordinates.Y, actionTime);
                bfc.Vector = currentCoordinates.Vector;
                return bfc;
            }

            bfc = currentCoordinates;
            try
            {
                TimeSpan deltaT = actionTime - currentCoordinates.Timestamp;

                int xfc = Convert.ToInt32(currentCoordinates.Vector.X * deltaT.TotalSeconds + currentCoordinates.X);
                int yfc = Convert.ToInt32(currentCoordinates.Vector.Y * deltaT.TotalSeconds + currentCoordinates.Y);

                if (_surveyor.IsCoordinatesInRange(xfc, yfc))
                {
                    bfc = new BallCoordinates(xfc, yfc, actionTime);
                    bfc.Vector = currentCoordinates.Vector;
                }
                else
                {
                    BallCoordinates ricoshetCoordiantes = _ricochetCalc.Ricochet(currentCoordinates);
                    return FindBallFutureCoordinates(ricoshetCoordiantes, actionTime);
                }
            }
            catch (Exception e)
            {
                Log.Common.Error(String.Format("[{0}] Error: {1}", MethodBase.GetCurrentMethod().Name, e.Message));
            }
            return bfc;
        }
    }
}
