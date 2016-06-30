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
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Responsible for creating marks as shapes
    /// </summary>
    internal static class ShapesFactory
    {
        /// <summary>
        /// Create methods
        /// </summary>
        /// <returns>Dictionary of marks and related shapes</returns>
        public static Dictionary<eMarks, FrameworkElement> Create()
        {
            Dictionary<eMarks, FrameworkElement> _markups = new Dictionary<eMarks, FrameworkElement>();
            foreach (eMarks mark in Enum.GetValues(typeof(eMarks)))
            {
                switch (mark)
                {
                    case eMarks.BallMark:
                    case eMarks.ButtomLeftMark:
                    case eMarks.ButtomRightMark:
                    case eMarks.TopLeftMark:
                    case eMarks.TopRightMark:
                    case eMarks.RicochetMark:
                    case eMarks.GoalKeeperPlayer:
                    case eMarks.DefencePlayer1:
                    case eMarks.DefencePlayer2:
                    case eMarks.MidfieldPlayer1:
                    case eMarks.MidfieldPlayer2:
                    case eMarks.MidfieldPlayer3:
                    case eMarks.MidfieldPlayer4:
                    case eMarks.MidfieldPlayer5:
                    case eMarks.AttackPlayer1:
                    case eMarks.AttackPlayer2:
                    case eMarks.AttackPlayer3:
                        _markups.Add(mark, new Ellipse());
                        break;
                    case eMarks.GoalKeeperPlayerRect:
                    case eMarks.DefencePlayer1Rect:
                    case eMarks.DefencePlayer2Rect:
                    case eMarks.MidfieldPlayer1Rect:
                    case eMarks.MidfieldPlayer2Rect:
                    case eMarks.MidfieldPlayer3Rect:
                    case eMarks.MidfieldPlayer4Rect:
                    case eMarks.MidfieldPlayer5Rect:
                    case eMarks.AttackPlayer1Rect:
                    case eMarks.AttackPlayer2Rect:
                    case eMarks.AttackPlayer3Rect:
                        _markups.Add(mark, new Rectangle());
                        break;
                    case eMarks.ButtomLeftText:
                    case eMarks.ButtomRightText:
                    case eMarks.TopLeftText:
                    case eMarks.TopRightText:
                        _markups.Add(mark, new TextBlock());
                        break;
                    case eMarks.BallVectorArrow:
                        _markups.Add(mark, new Line());
                        _markups.Add(eMarks.BallVectorArrow + 1, new Line());
                        _markups.Add(eMarks.BallVectorArrow + 2, new Line());
                        break;
                    case eMarks.LeftBorder:
                    case eMarks.RightBorder:
                    case eMarks.TopBorder:
                    case eMarks.BottomBorder:
                    case eMarks.GoalKeeper:
                    case eMarks.Defence:
                    case eMarks.Midfield:
                    case eMarks.Attack:
                    case eMarks.GoalKeeperSector1:
                    case eMarks.GoalKeeperSector2:
                    case eMarks.DefenceSector1:
                    case eMarks.DefenceSector2:
                    case eMarks.MidfieldSector1:
                    case eMarks.MidfieldSector2:
                    case eMarks.AttackSector1:
                    case eMarks.AttackSector2:
                        _markups.Add(mark, new Line());
                        break;
                }
            }
            return _markups;
        }
    }
}
