// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.ImageProcessingUnit.Tools.Enums;
using System.Collections.Generic;

namespace Foosbot.ImageProcessingUnit.Tools.Contracts
{
    /// <summary>
    /// Interface with collection of Computer Vision Monitors to be used by other tools
    /// </summary>
    public interface IComputerVisionMonitorCollection
    {
        /// <summary>
        /// Dictionary of Computer Vision Monitors to display image prcoessing data on the screen
        /// Where key is an enum of Computer Vision Monitor type and value is its Instance
        /// </summary>
        Dictionary<eComputerVisionMonitor, IComputerVisionMonitor> ComputerVisionMonitors { get; }
    }
}
