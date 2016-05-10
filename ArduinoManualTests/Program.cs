using Foosbot.Common.Enums;
using Foosbot.Common.Protocols;
using Foosbot.CommunicationLayer;
using Foosbot.CommunicationLayer.Contracts;
using Foosbot.CommunicationLayer.Core;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Foosbot.ArduinoManualTests
{
    class Program
    {
        static void Main(string [] args)
        {
            Dictionary<int, eRotationalMove> vector = new Dictionary<int, eRotationalMove>();
            vector.Add(300, eRotationalMove.KICK);
            vector.Add(2000, eRotationalMove.DEFENCE);
            vector.Add(800, eRotationalMove.RISE);
            vector.Add(2200, eRotationalMove.DEFENCE);

            string[] portsList = SerialPort.GetPortNames();
            if (portsList.Length < 1)
            {
                Console.WriteLine("No Arduino connected!");
            }
            else
            {
                IRodConverter converter = new ArduinoConverter(eRod.GoalKeeper);
                ArduinoCom arduino = new ArduinoCom(portsList[0], new ActionEncoder(converter));
                try
                {
                    arduino.OpenArduinoComPort();
                    Console.WriteLine("Arduino port {0} is open!", portsList[0]);
                    arduino.Initialize();
                    Console.WriteLine("Arduino port is initialized!");
                    arduino.MaxTicks = 2600;

                    Thread.Sleep(5000);

                    while (true)
                    {
                        foreach (var pair in vector)
                        {
                            Console.WriteLine("Moving: {0}, {1} ", pair.Key, pair.Value.ToString());
                            arduino.Move(pair.Key, pair.Value);
                            Thread.Sleep(1500);

                            //arduino.Initialize();
                            //Console.WriteLine("Arduino port is initialized!");
                            //Thread.Sleep(10000);
                        }
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
