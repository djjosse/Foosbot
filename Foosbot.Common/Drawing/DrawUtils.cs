// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Data;
using Foosbot.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Drawing Marks utilities
    /// </summary>
    internal class DrawUtils
    {
        /// <summary>
        /// Convert rod type to first player on rod mark
        /// </summary>
        /// <param name="rodType">Rod Type</param>
        /// <returns>First player on rod mark</returns>
        public eMarks RodTypeToFirstPlayerMark(eRod rodType)
        {
            switch (rodType)
            {
                case eRod.GoalKeeper:
                    return eMarks.GoalKeeper;
                case eRod.Defence:
                    return eMarks.DefencePlayer1;
                case eRod.Midfield:
                    return eMarks.MidfieldPlayer1;
                case eRod.Attack:
                    return eMarks.AttackPlayer1;
                default:
                    return eMarks.NA;
            }
        }

        public bool IsMarkOfFirstPlayerInRod(eMarks mark)
        {
            switch (mark)
            {
                case eMarks.GoalKeeper:
                case eMarks.DefencePlayer1:
                case eMarks.MidfieldPlayer1:
                case eMarks.AttackPlayer1:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Convert coordinate points to location points
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public void ConvertToLocation(ref int x, ref int y)
        {
            double outX, outY;
            TransformAgent.Data.InvertTransform(x, y, out outX, out outY);
            x = Convert.ToInt32(outX);
            y = Convert.ToInt32(outY);
        }
    }
}
