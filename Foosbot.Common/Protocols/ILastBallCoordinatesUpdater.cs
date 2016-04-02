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

namespace Foosbot.Common.Protocols
{
    /// <summary>
    /// Last Ball Coordinates Interface for implementing interface
    /// </summary>
    public interface ILastBallCoordinatesUpdater
    {
        /// <summary>
        /// Last stored ball coordinates
        /// </summary>
        BallCoordinates LastBallCoordinates { get; }
    }
}
