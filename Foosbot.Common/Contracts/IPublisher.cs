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
    /// Publisher Interface for pipeline publisher-observer pattern
    /// </summary>
    /// <typeparam name="T">Data Type to be published</typeparam>
    public interface IPublisher<T>
    {
        /// <summary>
        /// Stored Data property to take on update
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Attach new observer
        /// </summary>
        /// <param name="observer"></param>
        void Attach(IWorkingObserver<T> observer);

        /// <summary>
        /// Dettach existing observer
        /// </summary>
        /// <param name="observer"></param>
        void Detach(IWorkingObserver<T> observer);
    }
}
