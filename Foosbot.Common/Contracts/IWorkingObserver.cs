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
    /// Observer with Job to perform Interface for pipeline publisher-observer pattern
    /// </summary>
    /// <typeparam name="T">Data Type to be observed</typeparam>
    public interface IWorkingObserver<T>
    {
        /// <summary>
        /// Update to be called by publisher
        /// </summary>
        void Update();

        /// <summary>
        /// Tasks to perfrom before go to sleep
        /// Update will be received while sleeping and sleep will be 
        /// enterrupted - Job() will be triggered again
        /// If _currentData is null Job will not be performed and thread will go to sleep
        /// </summary>
        void Job();
    }
}
