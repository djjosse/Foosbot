using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer
{
    public class ArduinoConverter : IRodConverter
    {
        /// <summary>
        /// Current Rod Type
        /// </summary>
        public eRod RodType { get; private set; }

        /// <summary>
        /// Rod Length in ticks
        /// </summary>
        public int TicksPerRod { get; private set; }

        /// <summary>
        /// Rod Maximal Start Stopper Position in mm
        /// </summary>
        public int RodMaximalCoordinate { get; private set; }

        /// <summary>
        /// Rod Minimal Start Stopper Position in mm
        /// </summary>
        public int RodMinimalCoordinate { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rodType">Current rod type</param>
        public ArduinoConverter(eRod rodType)
        {
            RodType = rodType;
        }

        /// <summary>
        /// Is Converter Initialized property
        /// </summary>
        public bool IsInitialized  { get; private set; }

        /// <summary>
        /// Initialize Method
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                TicksPerRod = Configuration.Attributes.GetTicksPerRod(RodType);
                RodMinimalCoordinate = Configuration.Attributes.GetValue<int>(Configuration.Names.KEY_ROD_START_Y);
                int height = Configuration.Attributes.GetValue<int>(Configuration.Names.TABLE_HEIGHT);
                int rodLength = Configuration.Attributes.GetRodDistanceBetweenStoppers(RodType);
                RodMaximalCoordinate = height - rodLength - RodMinimalCoordinate;
                    
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Initialize with parameters Method
        /// </summary>
        /// <param name="ticksPerRod">Ticks for current rod</param>
        /// <param name="minStopperCoordinate">Rod Minimal Start Stopper Position in mm</param>
        /// <param name="tableYLength">Table height in mm (Y Axe)</param>
        /// <param name="distanceBetweenStoppers">Distance between rod stoppers in mm</param>
        public void Initialize(int ticksPerRod, int minStopperCoordinate, int tableYLength, int distanceBetweenStoppers)
        {
            if (!IsInitialized)
            {
                TicksPerRod = ticksPerRod;
                RodMinimalCoordinate = minStopperCoordinate;
                RodMaximalCoordinate = tableYLength - distanceBetweenStoppers - RodMinimalCoordinate;
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Convert and FLIP coordinate from mm to ticks
        /// </summary>
        /// <param name="mmCoord">Coordinate in mm</param>
        /// <param name="flipAxe">Flips the end to start if true [Default is True]</param>
        /// <returns>Coordinate in ticks</returns>
        public int MmToTicks(int mmCoord, bool flipAxe = true)
        {
            if (!IsInitialized) Initialize();

            //m = (y2 - y1)/(x2 - x1)
            double slope = (double)(TicksPerRod) / (double)(RodMaximalCoordinate - RodMinimalCoordinate);
            
            //y = m(x-x1) + y1
            double result = slope * (mmCoord - RodMinimalCoordinate);

            //Flip the Axe if required
            if (flipAxe)
                result = TicksPerRod - result;

            //return value according to safety buffer
            if (result > TicksPerRod - Communication.SAFETY_TICKS_BUFFER)
                result = TicksPerRod - Communication.SAFETY_TICKS_BUFFER;
            if (result < Communication.SAFETY_TICKS_BUFFER)
                result = Communication.SAFETY_TICKS_BUFFER;

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Convert ticks value to 6 bit corresponding value 
        /// between 1 (0x00000001) and 62 (0x00111110)
        /// </summary>
        /// <param name="dcInTicks">Coordinate in Ticks</param>
        /// <returns>Coordinate as integer</returns>
        public int TicksToBits(int dcInTicks)
        {
            if (!IsInitialized) Initialize();
            float m = (float)(Communication.MAX_DC_VALUE_TO_ENCODE - Communication.MIN_DC_VALUE_TO_ENCODE) / (float)TicksPerRod;
            return Convert.ToInt32(m * dcInTicks);
        }
    }
}
