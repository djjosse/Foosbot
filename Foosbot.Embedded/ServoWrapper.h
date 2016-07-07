// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

#ifndef _SERVOWRAPPER_h
#define _SERVOWRAPPER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <Servo.h>
#include "TxCode.h"

#define NA 0
#define KICK 1
#define DEFENCE 2
#define RISE 3


//Wrapper for servo in foosbot context
class ServoWrapper
{
private:
	//Degree of Kick Position
	const int KICK_DEGREES;
	//Degree of Defence Position
	const int DEFENCE_DEGREES;
	//Degree of Rise Position
	const int RISE_DEGREES;

	//servo object instance
	Servo _servo;
	//current servo state (0 - NA)
	int _servoState = 0;
	//is calibration flag
	bool _isCalibrated;

	//last action time-stamp
	unsigned long _lastActionTime = 0;
public:
	//servo arduino connection pin
	const int SERVO_PIN = 9;

	//constructor
	ServoWrapper(int kick, int defence, int rise) : 
		KICK_DEGREES(kick), DEFENCE_DEGREES(defence), RISE_DEGREES(rise) {}
	
	//set servo to desired state
	//0 - NA, 1 - Kick, 2 - Defence, 3 - Rise
	TxCode setState(int state);

	//calibration method sets servo to defence mode
	TxCode calibrate();

	//set current calibration status
	void setCalibrated(bool isCalibrated) { _isCalibrated = isCalibrated; }
};

#endif

