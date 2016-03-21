using Foosbot.Common.Multithreading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
