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
* Handler for input/output of Arduino
*/

#include "IOHandler.h"

//Constructor
IOHandler::IOHandler(int rodLength)
{
	this->dcFactor = (float)rodLength / (float)(MAX_CODED_DC_POSITION - MIN_CODED_DC_POSITION);
}

//Decode input method, sets latestRequest action
void IOHandler::decode(int input)
{
	int dcB = 0;

	//get command from serial as 8 bits (assuming a 8 bit int)
	int bits[8];
	for (int i = 0; i < 8; ++i)
	{
		bits[i] = input & (1 << i) ? 1 : 0;
	}
	//get servo command bits 0 and 1
	int servoB = bits[0] + bits[1] * 2;
	//get DC position between MIN_CODED_DC_POSITION (1) and MAX_CODED_DC_POSITION (62)
	for (int i = 7; i >= 2; i--)
	{
		dcB <<= 1;
		dcB += bits[i];
	}

	//calculate requested position in ticks
	int dc = dcFactor*(float)(dcB);

	//set requested position
	latestRequest.setAction(dc, servoB);
}

//Request Calibration Request to Computer System
void IOHandler::requestCalibration()
{
	if (millis() - _lastRefreshTime >= REFRESH)
	{
		_lastRefreshTime += REFRESH;
		Serial.write(TxCode::TX_INIT_REQUERED);
	}
}

//Sends response code to Computer System
void IOHandler::send(TxCode code)
{
	switch (code)
	{
	case TxCode::TX_INIT_REQUERED:
		requestCalibration();
		break;
	default:
		Serial.write((char)code);
		break;
	}
}

//Receives data from Computer System, sets latestRequest action
//Returns True if data available and state was changed, False otherwise
bool IOHandler::receive()
{
	if (Serial.available() > 0)
	{
		Serial.readBytes(inputBuffer, INPUT_BUFFER_LENGTH);
		int input = inputBuffer[0];
		switch (input)
		{
		case RxCode::RX_INPUT_INIT_REQUEST:
			latestRequest.setAction(true);
			return true;
		default:
			decode(input);
			return true;
		}
	}
	return false;
}


