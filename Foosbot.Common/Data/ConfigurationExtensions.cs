﻿// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using System;

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

        /// <summary>
        /// Get X Coordinate of current rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Current Rod Type</param>
        /// <returns>Distance from X min to current rod</returns>
        public static int GetRodXCoordinate(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<int>(rodType.ToString());
        }

        /// <summary>
        /// Get First Player Y coordinate for Best Effort linear move
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Current Rod Type</param>
        /// <returns>First Player Y coordinate for Best Effort</returns>
        public static int GetFirstPlayerBestEffort(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<int>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_BEST_EFFORT));
        }

        /// <summary>
        /// Get distance between stoppers on current rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Current Rod Type</param>
        /// <returns>Distance between stoppers</returns>
        public static int GetRodDistanceBetweenStoppers(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<int>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_STOPPER_DIST));
        }

        /// <summary>
        /// Get Number of ticks per rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Rod Type</param>
        /// <returns>Number of ticks per rod</returns>
        public static int GetTicksPerRod(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<int>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_TICKS));
        }

        /// <summary>
        /// Get Number of ticks per rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Rod Type</param>
        /// <returns>[True] Servo feedback should be taken from arduino, [False] it should be taken from decision</returns>
        public static bool IsServoExistsWithFeedback(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<bool>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_SERVO_FEEDBACK));
        }

        /// <summary>
        /// Get Arduino Serial Port Per Rod
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="rodType">Rod Type</param>
        /// <returns>Arduino Serial Port Name Per Rod as string</returns>
        public static string GetArduinoSerialPortPerRod(this Configuration configuration, eRod rodType)
        {
            return Configuration.Attributes.GetValue<string>(
                String.Format("{0}{1}", rodType.ToString(), Configuration.Names.SUBKEY_COM_PORT));
        }
    }
}
