// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

/*
* Wrapper for servo in foosbot context
* See "ServoWrapper.h" for function explanations
*/

#include "ServoWrapper.h"

#define WAIT_SERVO 150

//set servo to desired state
//0 - NA, 1 - Kick, 2 - Defence, 3 - Rise
TxCode ServoWrapper::setState(int state)
{
	if (!_servo.attached()) _servo.attach(SERVO_PIN);
	if (_servoState != state)
	{
		//Wait after last received command and ignore new commands
		//this is made in order to stop burining servos
		if (millis() - _lastActionTime >= WAIT_SERVO)
		{
			_lastActionTime = millis();
			switch (state)
			{
			case KICK:
				_servo.write(KICK_DEGREES);
				_servoState = state;
				break;
			case DEFENCE:
				_servo.write(DEFENCE_DEGREES);
				_servoState = state;
				break;
			case RISE:
				_servo.write(RISE_DEGREES);
				_servoState = state;
				break;
			default:
				break;
			}
		}
	}

	//return current state
	switch (_servoState)
	{
	case KICK:
		return TxCode::TX_NEW_SERVO_STATE_KICK;
	case DEFENCE:
		return TxCode::TX_NEW_SERVO_STATE_DEFENCE;
	case RISE:
		return TxCode::TX_NEW_SERVO_STATE_RISE;
	default:
		return TxCode::TX_NA;
	}
}

//calibration method sets servo to defence mode
TxCode ServoWrapper::calibrate()
{
	TxCode result = TxCode::TX_NA;
	if (!_isCalibrated)
	{
		result = setState(DEFENCE);
		_isCalibrated = true;
	}
	return result;
}