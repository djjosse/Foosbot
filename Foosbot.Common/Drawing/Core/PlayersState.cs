// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Enums;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Contains current player states per each rod
    /// </summary>
    internal class PlayersState
    {
        /// <summary>
        /// All rods and their stopper positions
        /// </summary>
        protected Dictionary<eRod, Point> _playerPositions = new Dictionary<eRod, Point>();

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayersState()
        {
            foreach (eRod type in Enum.GetValues(typeof(eRod)))
            {
                _playerPositions.Add(type, default(Point));
            }
        }

        /// <summary>
        /// Get player rod first player position by rod type
        /// </summary>
        /// <param name="type">RodType</param>
        /// <returns>Point - position of first player of rod in Frame dimensions</returns>
        public Point Get(eRod type)
        {
            return _playerPositions[type];
        }

        /// <summary>
        /// Get player rod first player position by rod type
        /// </summary>
        /// <param name="type">Rod Type</param>
        /// <param name="position">Point - position of first player of rod in Frame dimensions</param>
        public void Set(eRod type, Point position)
        {
            _playerPositions[type] = position;
        }

        /// <summary>
        /// Get player rod first player position by rod type
        /// </summary>
        /// <param name="mark">First Player Mark</param>
        /// <param name="position">Point - position of first player of rod in Frame dimensions</param>
        public void Set(eMarks mark, Point position)
        {
            if (IsFirstPlayerMark(mark))
            {
                eRod rodType = MarkTypeToRodType(mark);
                Set(rodType, position);
            }
        }

        /// <summary>
        /// Convert rod type to first player on rod mark
        /// </summary>
        /// <param name="rodType">Rod Type</param>
        /// <returns>First player on rod mark</returns>
        public eRod MarkTypeToRodType(eMarks markType)
        {
            switch (markType)
            {
                case eMarks.GoalKeeperPlayer:
                    return eRod.GoalKeeper;
                case eMarks.DefencePlayer1:
                    return eRod.Defence;
                case eMarks.MidfieldPlayer1:
                    return eRod.Midfield;
                case eMarks.AttackPlayer1:
                    return eRod.Attack;
                default:
                    throw new NotSupportedException(String.Format(
                "Provided mark {0} is not of a first player, and cannot be converted to eRod type.", markType));
            }
        }

        /// <summary>
        /// Convert rod type to first player on rod mark
        /// </summary>
        /// <param name="rodType">Rod Type</param>
        /// <returns>First player on rod mark</returns>
        public eMarks RodTypeToMarkOfFirstPlayerType(eRod rodType)
        {
            switch (rodType)
            {
                case eRod.GoalKeeper:
                    return eMarks.GoalKeeperPlayer;
                case eRod.Defence:
                    return eMarks.DefencePlayer1;
                case eRod.Midfield:
                    return eMarks.MidfieldPlayer1;
                case eRod.Attack:
                    return eMarks.AttackPlayer1;
                default:
                    throw new NotSupportedException(String.Format(
                "Provided rod type {0} is not supported, and cannot be converted to eMark type of first player.", rodType));
            }
        }

        /// <summary>
        /// Verify if selected mark is first player of rod mark
        /// </summary>
        /// <param name="markType">Mark</param>
        /// <returns>[True] if selected mark is first player of rod mark, [False] otherwise</returns>
        public bool IsFirstPlayerMark(eMarks markType)
        {
            switch (markType)
            {
                case eMarks.GoalKeeperPlayer:
                case eMarks.DefencePlayer1:
                case eMarks.MidfieldPlayer1:
                case eMarks.AttackPlayer1:
                    return true;
                default:
                    return false;
            }
        }
    }
}
