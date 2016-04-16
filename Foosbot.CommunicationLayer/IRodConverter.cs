using Foosbot.Common.Contracts;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer
{
    public interface IRodConverter : IInitializable
    {
        /// <summary>
        /// Currrent Rod Type
        /// </summary>
        eRod RodType { get; }

        /// <summary>
        /// Rod Length in ticks
        /// </summary>
        int TicksPerRod { get; }

        /// <summary>
        /// Rod Maximal Start Stopper Position in mm
        /// </summary>
        int RodMaximalCoordinate { get; }

        /// <summary>
        /// Rod Minimal Start Stopper Position in mm
        /// </summary>
        int RodMinimalCoordinate { get; }

        /// <summary>
        /// Initialize with parameters Method
        /// </summary>
        /// <param name="ticksPerRod">Ticks for current rod</param>
        /// <param name="minStopperCoordinate">Rod Minimal Start Stopper Position in mm</param>
        /// <param name="tableYLength">Table height in mm (Y Axe)</param>
        /// <param name="distanceBetweenStoppers">Distance between rod stoppers in mm</param>
        void Initialize(int ticksPerRod, int minStopperCoordinate, int tableYLength, int distanceBetweenStoppers);

        /// <summary>
        /// Convert coordinate from mm to ticks
        /// </summary>
        /// <param name="mmCoord">Coordinate in mm</param>
        /// <returns>Coordinate in ticks</returns>
        int MmToTicks(int mmCoord);
    }
}
