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
            Dictionary<int, eRotationalMove> commandsGoalKeeper = new Dictionary<int, eRotationalMove>();
            commandsGoalKeeper.Add(100, eRotationalMove.KICK);
            commandsGoalKeeper.Add(500, eRotationalMove.DEFENCE);
            commandsGoalKeeper.Add(200, eRotationalMove.RISE);
            commandsGoalKeeper.Add(400, eRotationalMove.DEFENCE);

            Dictionary<int, eRotationalMove> commandsDefence = new Dictionary<int, eRotationalMove>();
            commandsDefence.Add(300, eRotationalMove.KICK);
            commandsDefence.Add(6000, eRotationalMove.DEFENCE);
            commandsDefence.Add(3000, eRotationalMove.RISE);
            commandsDefence.Add(2000, eRotationalMove.DEFENCE);

            string[] portsList = SerialPort.GetPortNames();
            if (portsList.Length < 1)
            {
                 Console.WriteLine("No Arduino connected!");
            }
            else
            {
                IRodConverter converterGoalKeeper = new RodActionConverter(eRod.GoalKeeper);
                ISerialController arduinoGoalKeeper = new ArduinoController("COM3", new ActionEncoder(converterGoalKeeper))
                {
                    RodType = eRod.GoalKeeper
                };

                IRodConverter converterDefence = new RodActionConverter(eRod.Defence);
                ISerialController arduinoDefence = new ArduinoController("COM5", new ActionEncoder(converterDefence))
                {
                    RodType = eRod.Defence
                };

                try
                {
                    arduinoGoalKeeper.OpenSerialPort();
                    Console.WriteLine("Arduino port {0} is open!", "COM3");

                    arduinoDefence.OpenSerialPort();
                    Console.WriteLine("Arduino port {0} is open!", "COM5");

                    arduinoGoalKeeper.Initialize();
                    Console.WriteLine("Arduino port is initialized!");

                    arduinoDefence.Initialize();
                    Console.WriteLine("Arduino port is initialized!");
                    
                    
                    arduinoGoalKeeper.MaxTicks = 620;
                    arduinoDefence.MaxTicks = 6056;

                    Thread.Sleep(5000);

                    while (true)
                    {
                        for (int i = 0; i < commandsGoalKeeper.Count; i++ )
                        {
                            Console.WriteLine("Moving GOAL_KEEPER: {0}, {1} ", commandsGoalKeeper.ElementAt(i).Key, commandsGoalKeeper.ElementAt(i).Value.ToString());
                            arduinoGoalKeeper.Move(commandsGoalKeeper.ElementAt(i).Key, commandsGoalKeeper.ElementAt(i).Value);

                            Console.WriteLine("Moving DEFENCE: {0}, {1} ", commandsDefence.ElementAt(i).Key, commandsDefence.ElementAt(i).Value.ToString());
                            arduinoDefence.Move(commandsDefence.ElementAt(i).Key, commandsDefence.ElementAt(i).Value);

                            Thread.Sleep(2500);
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
