using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot
{
    /// <summary>
    /// Represents a flow that can run in Separate Thread
    /// </summary>
    public interface IFlow
    {
        /// <summary>
        /// Function that will run in Separate Thread
        /// </summary>
        void Flow();

        /// <summary>
        /// Run the flow in Thread
        /// </summary>
        void Start();
    }
}
