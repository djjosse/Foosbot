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
        Attack = 203,

        GoalKeeperPlayer = 210,
        GoalKeeperPlayerRect = 215,

        DefencePlayer1 = 220,
        DefencePlayer2 = 221,
        DefencePlayer1Rect = 225,
        DefencePlayer2Rect = 226,

        MidfieldPlayer1 = 230,
        MidfieldPlayer2 = 231,
        MidfieldPlayer3 = 232,
        MidfieldPlayer4 = 233,
        MidfieldPlayer5 = 234,
        MidfieldPlayer1Rect = 235,
        MidfieldPlayer2Rect = 236,
        MidfieldPlayer3Rect = 237,
        MidfieldPlayer4Rect = 238,
        MidfieldPlayer5Rect = 239,

        AttackPlayer1 = 240,
        AttackPlayer2 = 241,
        AttackPlayer3 = 242,
        AttackPlayer1Rect = 245,
        AttackPlayer2Rect = 246,
        AttackPlayer3Rect = 247
    }
}
