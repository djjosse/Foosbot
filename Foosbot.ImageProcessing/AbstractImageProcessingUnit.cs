// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Multithreading;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Abstract Image Processing Unit Class
    /// This class represent Image Processing Unit Base Class for 
    /// real Image Processing unit or development demo Image Processing unit
    /// </summary>
    public abstract class AbstractImageProcessingUnit : Observer<Frame>
    {
        /// <summary>
        /// Ball Location Publisher
        /// This inner object is a publisher for vector calculation unit 
        /// </summary>
        public BallLocationPublisher BallLocationPublisher { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="streamer">Streamer to get video stream from</param>
        public AbstractImageProcessingUnit(Publisher<Frame> streamer) :
            base(streamer)
        {
            _publisher = streamer;
        }

        /// <summary>
        /// Abstract function Job - must be overriden by derived class
        /// The actual job to be performed by Image Processing Unit in loop in different thread.
        /// </summary>
        public override abstract void Job();
    }
}
