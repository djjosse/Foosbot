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
namespace Foosbot.DecisionUnit.Contracts
{
    public interface IPredictor
    {
        /// <summary>
        /// Calculate Ball Future Coordinates in actual time system can responce
        /// </summary>
        /// <param name="currentCoordinates"><Current ball coordinates/param>
        /// <param name="actionTime">Actual system responce time</param>
        /// <returns>Ball Future coordinates</returns>
        BallCoordinates FindBallFutureCoordinates(BallCoordinates currentCoordinates, DateTime actionTime);
    }
}
