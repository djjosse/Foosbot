using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer.Contracts
{
    public enum eResponseCode
    {
        NO_DATA = 0,

        INIT_REQUERED = 100,
        INIT_REQUESTED = 101,
        INIT_STARTED = 102,
        INIT_FINISHED = 103,

        SERVO_STATE_KICK = 110,
        SERVO_STATE_DEFENCE = 111,
        SERVO_STATE_RISE = 112,

        DC_RANGE_INVALID = 120,
        DC_RECEIVED_OK = 121,
        DC_CALIBRATED = 122
    }
}
