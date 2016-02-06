using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Frame with time stamp
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Frame Image
        /// </summary>
        public Image<Gray, byte> image { get; set; }

        /// <summary>
        /// Frame Timestamp
        /// </summary>
        public DateTime timestamp { get; set; }
    }
}
