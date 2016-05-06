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
using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInitializableRicochet: IInitializable, IRicochet
    {
        /// <summary>
        /// Initialization Method - reads all constants from Configuration file
        /// </summary>
        /// <param name="units">Desired units to work in [default is Points]</param>
        /// <param name="xMaxBorder">X Max Limit for Ricochet Calculations</param>
        /// <param name="yMaxBorder">Y Max Limit for Ricochet Calculations</param>
        /// <param name="ricochetFactor">Ricochet Factor to use in calculations</param>
        void InitializeRicochetCalc(eUnits units = eUnits.Pts, double xMaxBorder = -1, double yMaxBorder = -1, double ricochetFactor = -1);
    }
}
