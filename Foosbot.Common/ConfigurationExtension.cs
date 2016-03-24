using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot
{
    /// <summary>
    /// Extensions for Configuration class
    /// </summary>
    public static class ConfigurationExtension
    {
        /// <summary>
        /// Get players count of specific rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Current Rod Type</param>
        /// <returns>Number of players (mm)</returns>
        public static int GetPlayersCountPerRod(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<int>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_COUNT));
        }

        /// <summary>
        /// Get Distance (Y) of first player to border of a table for current rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Current Rod Type</param>
        /// <returns>Offset in (mm)</returns>
        public static int GetPlayersOffsetYPerRod(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<int>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_OFFSET_Y));
        }

        /// <summary>
        /// Get Distance between players for current rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Current Rod Type</param>
        /// <returns>Distance between players</returns>
        public static int GetPlayersDistancePerRod(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<int>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_DISTANCE));
        }
    }
}
