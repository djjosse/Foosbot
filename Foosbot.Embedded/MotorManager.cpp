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
* Motor manager class
* Class is responsible for setting DC/Servo states and receive/send data
*/

#include "MotorManager.h"

//Constructor
MotorManager::MotorManager(DcMotor dcMotor, ServoWrapper servoMotor, IOHandler ioHandler)
: dc(dcMotor), servo(servoMotor), handler(ioHandler)
{
	isInitialized = false;
}

//Main management method
//Reads input, performs operations, sends responses to Computer System
void MotorManager::manage()
{
	bool available = handler.receive();
	if (available)
	{
		Action request = handler.getLatestRequest();
		if (request.initRequested())
		{
			handler.send(TxCode::TX_INIT_REQUESTED);
			initialize();
		}
		else if (isInitialized)
		{
			TxCode result = dc.setPosition(request.getDc());
			handler.send(result);
			result = servo.setState(request.getServo());
			handler.send(result);
		}
	}
	if (!isInitialized)
	{
		handler.send(TxCode::TX_INIT_REQUERED);
	}
}

//Motor Position Verification Mehod
//Verifies Motor Positions if system was calibrated
void MotorManager::verify()
{
	if (isInitialized)
		dc.verifyPosition();
}

//Calibration method
void MotorManager::initialize()
{
	handler.send(TxCode::TX_INIT_STARTED);

	//Calibrate Servo
	servo.setCalibrated(false);
	TxCode result = servo.calibrate();
	handler.send(result);

	//Calibrate DC
	dc.setCalibrated(false);
	dc.calibrate();
	handler.send(TxCode::TX_DC_CALIBRATED);

	isInitialized = true;

	Serial.write(TxCode::TX_INIT_FINISHED);
}