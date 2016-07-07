// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

#ifndef _IOHANDLER_h
#define _IOHANDLER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "TxCode.h"
#include "RxCode.h"
#include "Action.h"


#define INPUT_BUFFER_LENGTH 1
#define REFRESH 1000
#define INPUT_SIZE 10

#define MAX_CODED_DC_POSITION 62
#define MIN_CODED_DC_POSITION 1

class IOHandler
{
private:
	//input buffer to read from serial port
	uint8_t inputBuffer[INPUT_BUFFER_LENGTH];

	//Slope of convertion function to calculate DC Position
	float dcFactor;

	//Latest action to be performed by arduino
	Action latestRequest;

	//Last initialization request time-stamp
	unsigned long _lastRefreshTime = 0;

	//Decode input method, sets latestRequest action
	void decode(int input);

	//Request Calibration Request to Computer System
	void requestCalibration();

public:

	//Constructor
	IOHandler(int rodLength);

	//Sends response code to Computer System
	void send(TxCode code);

	//Receives data from Computer System, sets latestRequest action
	//Returns True if data available and state was changed, False otherwise
	bool receive();

	//Returns the latest request by Computer System
	Action getLatestRequest() { return latestRequest; }
};

#endif
