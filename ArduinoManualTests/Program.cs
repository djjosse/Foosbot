using Foosbot.Common.Protocols;
using Foosbot.CommunicationLayer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoManualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] portsList = SerialPort.GetPortNames();
            if (portsList.Length < 1)
            {
                Console.WriteLine("No Arduino connected!");
            }
            else
            {
                ArduinoCom arduino = new ArduinoCom(portsList[0]);
                try
                {
                    arduino.OpenArduinoComPort();
                    Console.WriteLine("Arduino port {0} is open!", portsList[0]);
                    arduino.Initialize();
                    Console.WriteLine("Arduino port is initialized!");
                    string input = "";
                    while (true)
                    {
                        Console.Write("Waiting for input: ");
                        input = Console.ReadLine();

                        string[] chars = input.Split('&');
                        if (chars.Length == 0)
                        {
                            break;
                        }
                        int dc = Convert.ToInt32(chars[0]);
                        int servo = 0;
                        if (chars.Length > 1)
                        {
                            servo = Convert.ToInt32(chars[1]);
                        }

                        Console.WriteLine("Performing: dc: {0} servo: {1}", dc, servo);
                        arduino.Move(dc, (eRotationalMove)servo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured: {0}", ex.Message);
                }
            }
        }
    }
}
