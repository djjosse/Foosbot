// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

#ifndef _ACTION_h
#define _ACTION_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

//Action to perform by Arduino
class Action
{
private:
	//Servo Position
	int servo;
	//DC Position
	int dc;
	//True if initialization requested
	bool init;
public:
	//Constructor
	Action()  { };

	//Set action with dc and servo position
	void setAction(int dc, int servo)
	{
		this->servo = servo;
		this->dc = dc;
		init = false;
	}

	//Set action with initialization flag
	void setAction(bool initRequested)	{ init = initRequested;	}

	//True if flag og initialization is True, False otherwise
	bool initRequested() { return init; }
	
	//Dc Position getter
	int getDc() { return dc; }

	//Servo position getter
	int getServo() { return servo; }
};

#endif