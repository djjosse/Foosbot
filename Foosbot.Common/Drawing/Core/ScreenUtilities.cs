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
using System.Linq;
using System.Reflection;

namespace Foosbot.Common.Drawing
{
    /// <summary>
    /// Class with utility methods for Screen
    /// </summary>
    internal abstract class ScreenUtilities
    {
        /// <summary>
        /// Verification if provided coordinates type is supported by draw method
        /// </summary>
        /// <param name="method">Draw method</param>
        /// <param name="provided">Provided coordinates type</param>
        /// <param name="supported">Supported coordinates array</param>
        /// <exception cref="NotSupportedException">Thrown in case provided coordinates type is not supported</exception>
        protected void VerifyCoordinatesTypeSupported(MethodBase method, eCoordinatesType provided, params eCoordinatesType[] supported)
        {
            if (supported != null && !supported.Contains(provided))
                throw new NotSupportedException(String.Format("Coordinates of type {0} are not supported by [{1}]!", provided, method.Name));
        }

        /// <summary>
        /// Convert provided calibration mark to out parameters: mark of circle and text
        /// </summary>
        /// <param name="mark">Calibration mark</param>
        /// <param name="circle">[OUT] circle mark</param>
        /// <param name="text">[OUT] text mark</param>
        protected void TryParseToCalibrationCircleAndTextMark(eCallibrationMark mark, out eMarks circle, out eMarks text)
        {
            circle = eMarks.NA;
            text = eMarks.NA;
            switch (mark)
            {
                case eCallibrationMark.BL:
                    circle = eMarks.ButtomLeftMark;
                    text = eMarks.ButtomLeftText;
                    break;
                case eCallibrationMark.BR:
                    circle = eMarks.ButtomRightMark;
                    text = eMarks.ButtomRightText;
                    break;
                case eCallibrationMark.TL:
                    circle = eMarks.TopLeftMark;
                    text = eMarks.TopLeftText;
                    break;
                case eCallibrationMark.TR:
                    circle = eMarks.TopRightMark;
                    text = eMarks.TopRightText;
                    break;
            }
        }

        /// <summary>
        /// Parsing sector line marks by selected rod type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        protected void TryParseToSectorLineMarks(eRod type, out eMarks lineA, out eMarks lineB)
        {
            lineA = eMarks.NA;
            lineB = eMarks.NA;
            switch (type)
            {
                case eRod.GoalKeeper:
                    lineA = eMarks.GoalKeeperSector1;
                    lineB = eMarks.GoalKeeperSector2;
                    break;
                case eRod.Defence:
                    lineA = eMarks.DefenceSector1;
                    lineB = eMarks.DefenceSector2;
                    break;
                case eRod.Midfield:
                    lineA = eMarks.MidfieldSector1;
                    lineB = eMarks.MidfieldSector2;
                    break;
                case eRod.Attack:
                    lineA = eMarks.AttackSector1;
                    lineB = eMarks.AttackSector2;
                    break;
            }
        }
    }
}
