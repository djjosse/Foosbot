// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Protocols;
using Foosbot.DecisionUnit.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.DecisionUnit.Core
{
    public class RodState : IRodState
    {
        /// <summary>
        /// Minimal Position of DC in mm
        /// </summary>
        private int DC_LOW_LIMIT;

        /// <summary>
        /// Maximal Position of DC in mm
        /// </summary>
        private int DC_HIGH_LIMIT;

        /// <summary>
        /// Assumed for last known DC position (in mm)
        /// </summary>
        private int _dcPosition;

        /// <summary>
        /// Assumed for last known Servo position
        /// </summary>
        private eRotationalMove _servoPosition;

        /// <summary>
        /// Constructor for rod state
        /// </summary>
        /// <param name="dcLowLimit">Minimal Position of DC in mm</param>
        /// <param name="dcHighLimit">Maximal Position of DC in mm</param>
        /// <param name="initialDcPos">Initial DC position [default is 0]</param>
        /// <param name="initialServoPos">Initial Servo position [default is DEFENCE]</param>
        public RodState(int dcLowLimit, int dcHighLimit, int initialDcPos = 0, eRotationalMove initialServoPos =  eRotationalMove.DEFENCE)
        {
            DC_LOW_LIMIT = dcLowLimit;
            DC_HIGH_LIMIT = dcHighLimit;
            _dcPosition = (initialDcPos < dcLowLimit) ? dcLowLimit : initialDcPos;
            _servoPosition = initialServoPos;
        }

        /// <summary>
        /// Assumed for last known DC position (in mm)
        /// </summary>
        public int DcPosition
        { 
            get
            {
                return _dcPosition;
            }
            set
            {
                if (value >= DC_LOW_LIMIT && value <= DC_HIGH_LIMIT)
                {
                    _dcPosition = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(String.Format(
                        "[{0}] Provided argument for current DC position is [{1}]. Expected must be in range [{2} - {3}]",
                            MethodBase.GetCurrentMethod().Name, value, DC_LOW_LIMIT, DC_HIGH_LIMIT));
                }
            }
        }

        /// <summary>
        /// Assumed for last known Servo position
        /// </summary>
        public eRotationalMove ServoPosition
        {
            get
            {
                return _servoPosition;
            }
            set
            {
                switch(value)
                {
                    case eRotationalMove.DEFENCE:
                    case eRotationalMove.RISE:
                    case eRotationalMove.KICK:
                        _servoPosition = value;
                        break;
                    case eRotationalMove.NA:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(String.Format(
                            "[{0}] Provided argument for current SERVO position is [{1}]. Expected must be DEFENCE, RISE or KICK",
                                MethodBase.GetCurrentMethod().Name, value));
                }
            }
        }
    }
}
