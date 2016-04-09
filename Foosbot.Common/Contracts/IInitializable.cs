using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
