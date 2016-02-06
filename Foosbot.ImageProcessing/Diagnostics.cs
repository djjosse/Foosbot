using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Diagnostics data
    /// </summary>
    public class Diagnostics
    {
        /// <summary>
        /// Frame Rate
        /// </summary>
        public double Fps { get; set; }

        /// <summary>
        /// Frame Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Frame Height
        /// </summary>
        public int Height { get; set; }
    }
}
