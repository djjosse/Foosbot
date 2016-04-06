using Foosbot.Common.Protocols;
using Foosbot.CommunicationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoManualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            ArduinoCom arduino = new ArduinoCom("Com3");
            arduino.OpenArduinoComPort();
            arduino.InitializeArduino();
            string input = "";
            while(true)
            {
                input = Console.ReadLine();

                string [] chars = input.Split('&');
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

                Console.WriteLine("dc: " + dc + " servo: " + servo);
                arduino.Move(dc, (eRotationalMove)servo);
            }
        }
    }
}
