// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Contracts
{
    public interface IInitializableRod : IInitializable, IRod
    {
       /// <summary>
        /// Initialization with given parameters
        /// </summary>
        /// <param name="rodXCoordinate">X Coordinate of rod (in MM)</param>
        /// <param name="minSectorWidth">Minimal sector width (in MM)</param>
        /// <param name="sectorFactor">Sector factor to change width accoroding to speed</param>
        /// <param name="playerDistance">Distance beetween 2 player on rod (in MM)</param>
        /// <param name="playerCount">Player count on current rod (in MM)</param>
        /// <param name="offsetY">Distance between stopper and first player on rod (in MM)</param>
        /// <param name="stopperDistance">Distance between start and end stoppers of current rod (in MM)</param>
        /// <param name="bestEffort">Coordinate (in MM) for first player to be on in BEST_EFFORT state</param>
        void Initialize(int rodXCoordinate, int minSectorWidth, double sectorFactor,
            int playerDistance, int playerCount, int offsetY, int stopperDistance, int bestEffort);
    }
}
