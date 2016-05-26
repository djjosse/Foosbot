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
using Foosbot.VectorCalculation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation
{
    /// <summary>
    /// Ball Coordinates Updater Class
    /// </summary>
    class BallCoordinatesUpdater : ILastBallCoordinatesUpdater
    {
        /// <summary>
        /// Last Ball Coordinates
        /// </summary>
        public BallCoordinates LastBallCoordinates { get; set; }
    }
}
