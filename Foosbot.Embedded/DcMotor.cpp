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
* Class represents instance of DCMotor with encoder and stopper buttons
* See comments for functions in "DcMotor.h"
*/

#include "DcMotor.h"
#include "Arduino.h"

//constructor
DcMotor::DcMotor(volatile int * currentPosition, int rodLength, bool isReversed, 
	int calibraionSpeed, double kp, double ki, double kd, int buffer)
	: ROD_LENGTH(rodLength), ROD_REVERSED(isReversed), INIT_SPEED(calibraionSpeed),
		KP(kp), KI(ki), KD(kd), BUFFER(buffer)
{
	//initialize current position and desired position for PID
	_curPosPtr = currentPosition;
	_setpoint = 0;

	//create pid instance and configure it
	_pid = new PID(&_input, &_output, &_setpoint, KP, KI, KD, DIRECT);
	_pid->SetMode(AUTOMATIC);
	_pid->SetSampleTime(10);
	_pid->SetOutputLimits(-200, 200);

	//set calibration flag to false
	_isCalibrated = false;
}

//set dc direction to forward
void DcMotor::setForward()
{
	if (ROD_REVERSED)
		digitalWrite(DC_DIRECTION, LOW);
	else
		digitalWrite(DC_DIRECTION, HIGH);
}

//set dc direction to 
void DcMotor::setBackward()
{
	if (ROD_REVERSED)
		digitalWrite(DC_DIRECTION, HIGH);
	else
		digitalWrite(DC_DIRECTION, LOW);
}

//set dc speed (255 max, 0 stop)
void DcMotor::setSpeed(double speed)
{
	analogWrite(DC_POWER, speed);
}

//set dc position - moves back or forward according to PID and current position,
//must be called on new input
TxCode DcMotor::setPosition(int newPosition)
{
	TxCode result = TxCode::TX_NA;
	if (newPosition >= 0 && newPosition <= BUFFER) 
		newPosition = BUFFER;
	if (newPosition >= ROD_LENGTH - BUFFER && newPosition <= ROD_LENGTH)
		newPosition = ROD_LENGTH - BUFFER;
	if (newPosition <= ROD_LENGTH - BUFFER && newPosition >= BUFFER)
	{
		if (digitalRead(END_BUTTON) == LOW) (*_curPosPtr) = ROD_LENGTH;
		if (digitalRead(START_BUTTON) == LOW) (*_curPosPtr) = 0;

		result = TxCode::TX_DC_RECEIVED_OK;

		_lastReceivedPosition = newPosition;
		_setpoint = newPosition;
		_input = *_curPosPtr;

		_pid->Compute();

		if (_setpoint > (*_curPosPtr) + PID_ERROR)
		{
			if (digitalRead(END_BUTTON) != LOW)
			{
				setForward();
				setSpeed(_output);
			}
			else
			{
				setSpeed(0);
			}
		}
		else if (_setpoint < (*_curPosPtr) - PID_ERROR)
		{
			if (digitalRead(START_BUTTON) != LOW)
			{
				setBackward();
				setSpeed(_output*(-1));
			}
			else
			{
				setSpeed(0);
			}
		}
		//Position reached
		else
		{
			setSpeed(0);
		}
	}
	else
	{
		result = TxCode::TX_DC_RANGE_INVALID;
	}
	return result;
}

//verifies current position is as desired - uses setPosition() to fix,
//must be called every loop
TxCode DcMotor::verifyPosition()
{
	TxCode result = TxCode::TX_NA;
	result = setPosition(_lastReceivedPosition);
	return result;
}

//calibrattion detects 0 position of a rod, must be called once at the begging of a programm
//sets calibration flag as true, second call will be ignored
void DcMotor::calibrate()
{
	if (!_isCalibrated)
	{
		//move to start pin (0)
		while (digitalRead(START_BUTTON) != LOW)
		{
			setBackward();
			setSpeed(INIT_SPEED);
		}
		setSpeed(0);
		*_curPosPtr = 0;
		_setpoint = *_curPosPtr;
		_isCalibrated = true;
	}
}