// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using Foosbot.Common.Enums;

namespace Foosbot.CommunicationLayer.Contracts
{
    public interface IRodConverter : IInitializable
    {
        /// <summary>
        /// Current Rod Type
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
        /// <param name="flipAxe">Flips the end to start if true [Default is True]</param>
        /// <returns>Coordinate in ticks</returns>
        int MmToTicks(int mmCoord, bool flipAxe = true);

        /// <summary>
        /// Convert ticks value to 6 bit corresponding value 
        /// between 1 (0x00000001) and 62 (0x00111110)
        /// </summary>
        /// <param name="dcInTicks">Coordinate in Ticks</param>
        /// <returns>Coordinate as integer</returns>
        int TicksToBits(int dcInTicks);
    }
}
