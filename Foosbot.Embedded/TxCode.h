// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

#ifndef _TXCODE_H
#define _TXCODE_H

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

//Data Output for Computer System codes
enum TxCode
{
	TX_NA = 63,

	TX_INIT_REQUERED = 'i',				//i(nitialization required)
	TX_INIT_REQUESTED = 'r',			//r(equested initialization)
	TX_INIT_STARTED = 's',				//s(tarted initialization)
	TX_INIT_FINISHED = 'f',				//f(inished initialization)

	TX_NEW_SERVO_STATE_KICK = 'K',		//K(ick)
	TX_NEW_SERVO_STATE_DEFENCE = 'D',	//D(efence)
	TX_NEW_SERVO_STATE_RISE = 'R',		//R(ise)

	TX_DC_RANGE_INVALID = 'E',			//E(rror of DC range)
	TX_DC_RECEIVED_OK = 'O',			//O(k DC received)
	TX_DC_CALIBRATED = 'C'				//C(alibrated DC)
};

#endif