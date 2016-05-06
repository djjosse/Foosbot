// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.Common.Contracts
{
    /// <summary>
    /// Interface for initializable object
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Is Initialized property
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initialization method
        /// </summary>
        void Initialize();
    }
}
