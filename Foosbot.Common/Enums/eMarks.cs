using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.Common.Enums
{
    public enum eMarks : int
    {
        NA = 0,
        BallMark = 1,

        BallVectorArrow = 11, //12,13 - reserved
        RicochetMark = 14,

        ButtomLeftMark = 101,
        ButtomRightMark = 103,
        TopLeftMark = 105,
        TopRightMark = 107,
        ButtomLeftText = 102,
        ButtomRightText = 104,
        TopLeftText = 106,
        TopRightText = 108,

        GoalKeeper = 200,
        Defence = 201,
        Midfield = 202,
        Attack = 203
    }
}
