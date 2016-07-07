// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

#ifndef _MOTORMANAGER_h
#define _MOTORMANAGER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "IoHandler.h"
#include "DcMotor.h"
#include "ServoWrapper.h";
#include "TxCode.h"

//Motor manager class
//Class is responsible for setting DC/Servo states and receive/send data
class MotorManager
{
private:
	//Dc Motor Instance
	DcMotor dc;
	//Servo Motor Instance
	ServoWrapper servo;
	//Input/Output handler Instance
	IOHandler handler;
	//Is Initialized flag
	bool isInitialized;

	//Calibration method
	void initialize();
public:
	//Constructor
	MotorManager(DcMotor dcMotor, ServoWrapper servoMotor, IOHandler ioHandler);

	//Main management method
	//Reads input, performs operations, sends responses to Computer System
	void manage();

	//Motor Position Verification Mehod
	//Verifies Motor Positions if system was calibrated
	void verify();
};

#endif