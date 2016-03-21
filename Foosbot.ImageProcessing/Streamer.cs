using Foosbot.Common.Multithreading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Video Streamer Abstract class
    /// </summary>
    public abstract class Streamer : Publisher<Frame>
    {
        /// <summary>
        /// Frame Rate 
        /// </summary>
        public int FrameRate { get; protected set; }

        /// <summary>
        /// Frame Width
        /// </summary>
        public int FrameWidth { get; protected set; }

        /// <summary>
        /// Frame Height
        /// </summary>
        public int FrameHeight { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Streamer() { }

        /// <summary>
        /// Start streaming
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Frame Process Function called on Image Grabbed Event
        /// </summary>
        protected abstract void ProcessFrame(object sender, EventArgs e);

        /// <summary>
        /// Update Diagnostic Info abstract function
        /// </summary>
        protected abstract void UpdateDiagnosticInfo();
    }
}
