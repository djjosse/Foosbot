// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.ImageProcessingUnit.Process.Contracts
{
    /// <summary>
    /// Pausable and Resumable Flow
    /// </summary>
    public interface IPauseResume
    {
        /// <summary>
        /// Is Currently flow paused
        /// </summary>
        bool IsPaused { get; }
        
        /// <summary>
        /// Pause the flow
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume the Flow
        /// </summary>
        void Resume();
    }
}
