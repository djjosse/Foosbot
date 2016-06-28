// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.Common.Enums
{
    /// <summary>
    /// Marks used in GUI to draw current system states
    /// </summary>
    public enum eMarks : int
    {
        /// <summary>
        /// Mark undefined
        /// </summary>
        NA = 0,

        /// <summary>
        /// Ball mark
        /// </summary>
        BallMark = 01,

        /// <summary>
        /// Ball vector mark
        /// </summary>
        BallVectorArrow = 11, //12,13 - reserved for arrows

        /// <summary>
        /// Ball Ricochet point mark
        /// </summary>
        RicochetMark = 14,

        #region Calibration Marks

        /// <summary>
        /// Buttom Left calibration mark
        /// </summary>
        ButtomLeftMark = 101,

        /// <summary>
        /// Buttom Right calibration mark
        /// </summary>
        ButtomRightMark = 103,

        /// <summary>
        /// Top Left calibration mark
        /// </summary>
        TopLeftMark = 105,

        /// <summary>
        /// Top Right calibration mark
        /// </summary>
        TopRightMark = 107,

        /// <summary>
        /// Buttom Left calibration text (position)
        /// </summary>
        ButtomLeftText = 102,

        /// <summary>
        /// Buttom Right calibration text (position)
        /// </summary>
        ButtomRightText = 104,

        /// <summary>
        /// Top Left calibration text (position)
        /// </summary>
        TopLeftText = 106,

        /// <summary>
        /// Top Right calibration text (position)
        /// </summary>
        TopRightText = 108,

        #endregion Calibration Marks
        
        #region Table Border Marks

        /// <summary>
        /// Left field border line
        /// </summary>
        LeftBorder = 111,

        /// <summary>
        /// Right field border line
        /// </summary>
        RightBorder = 112,

        /// <summary>
        /// Top field border line
        /// </summary>
        TopBorder = 113,

        /// <summary>
        /// Buttom field border line
        /// </summary>
        BottomBorder = 114,

        #endregion Table Border Marks

        #region Rod Line Marks

        /// <summary>
        /// Goal keaper rod line
        /// </summary>
        GoalKeeper = 200,

        /// <summary>
        /// Defence rod line
        /// </summary>
        Defence = 201,

        /// <summary>
        /// Midfield rod line
        /// </summary>
        Midfield = 202,

        /// <summary>
        /// Attack rod line
        /// </summary>
        Attack = 203,

        #endregion Rod Line Marks

        #region Player Marks

        /// <summary>
        /// Goal keeper player mark
        /// </summary>
        GoalKeeperPlayer = 210,

        /// <summary>
        /// Goal keeper player rectangle mark
        /// </summary>
        GoalKeeperPlayerRect = 215,

        /// <summary>
        /// First Defence player mark
        /// </summary>
        DefencePlayer1 = 220,

        /// <summary>
        /// Second Defence player mark
        /// </summary>
        DefencePlayer2 = 221,

        /// <summary>
        /// First Defence player rectangle mark
        /// </summary>
        DefencePlayer1Rect = 225,

        /// <summary>
        /// Second Defence rectangle mark
        /// </summary>
        DefencePlayer2Rect = 226,

        /// <summary>
        /// First Midfield player mark
        /// </summary>
        MidfieldPlayer1 = 230,

        /// <summary>
        /// Second Midfield player mark
        /// </summary>
        MidfieldPlayer2 = 231,

        /// <summary>
        /// Third Midfield player mark
        /// </summary>
        MidfieldPlayer3 = 232,

        /// <summary>
        /// Fourth Midfield player mark
        /// </summary>
        MidfieldPlayer4 = 233,

        /// <summary>
        /// Fifth Midfield player mark
        /// </summary>
        MidfieldPlayer5 = 234,

        /// <summary>
        /// First Midfield player rectangle mark
        /// </summary>
        MidfieldPlayer1Rect = 235,

        /// <summary>
        /// Second Midfield player rectangle mark
        /// </summary>
        MidfieldPlayer2Rect = 236,

        /// <summary>
        /// Third Midfield player rectangle mark
        /// </summary>
        MidfieldPlayer3Rect = 237,

        /// <summary>
        /// Fourth Midfield player rectangle mark
        /// </summary>
        MidfieldPlayer4Rect = 238,

        /// <summary>
        /// Fifth Midfield player rectangle mark
        /// </summary>
        MidfieldPlayer5Rect = 239,

        /// <summary>
        /// First Attack player mark
        /// </summary>
        AttackPlayer1 = 240,

        /// <summary>
        /// Second Attack player mark
        /// </summary>
        AttackPlayer2 = 241,

        /// <summary>
        /// Third Attack player mark
        /// </summary>
        AttackPlayer3 = 242,

        /// <summary>
        /// First Attack player rectangle mark
        /// </summary>
        AttackPlayer1Rect = 245,

        /// <summary>
        /// Second Attack player rectangle mark
        /// </summary>
        AttackPlayer2Rect = 246,

        /// <summary>
        /// Third Attack player rectangle mark
        /// </summary>
        AttackPlayer3Rect = 247,

        #endregion Player Marks

        #region Sector Border Line Marks

        /// <summary>
        /// Goal Keeper sector line 1
        /// </summary>
        GoalKeeperSector1 = 2000,

        /// <summary>
        /// Goal Keeper sector line 2
        /// </summary>
        GoalKeeperSector2 = 2001,

        /// <summary>
        /// Defence sector line 1
        /// </summary>
        DefenceSector1 = 2010,

        /// <summary>
        /// Defence sector line 2
        /// </summary>
        DefenceSector2 = 2011,

        /// <summary>
        /// Midfield sector line 1
        /// </summary>
        MidfieldSector1 = 2020,

        /// <summary>
        /// Midfield sector line 2
        /// </summary>
        MidfieldSector2 = 2021,

        /// <summary>
        /// Attack sector line 1
        /// </summary>
        AttackSector1 = 2030,

        /// <summary>
        /// Attack sector line 2
        /// </summary>
        AttackSector2 = 2031

        #endregion Sector Border Line Marks
    }
}
