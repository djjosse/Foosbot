using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Foosbot.Common.Protocols;

namespace ArduinoTestCommunication
{
    public class ArduinoCom
    {
        private const string KEY_INIT = "init";
        private const int RATE = 9600;

        /// <summary>
        /// Port List where eRod key represents rod type port number
        /// </summary>
        private Dictionary<eRod, SerialPort> portList;
        
        public ArduinoCom()
        {
            portList = new Dictionary<eRod, SerialPort>();
        }

        public void Init()
        {
            string[] portsList = System.IO.Ports.SerialPort.GetPortNames();

            //TODO: need to support several Arduino, identify by Arduino id
            portList.Add(eRod.GoalKeeper, OpenPort(portsList[0]));

            //callibrate all rods
            foreach(eRod rod in portList.Keys)
            {
                InitRod(rod);
            }
        }

        private void InitRod(eRod rod)
        {
            portList[rod].WriteLine(KEY_INIT);
        }

        private SerialPort OpenPort(String port)
        {
            SerialPort currentPort = new SerialPort();
            currentPort.PortName = port;
            currentPort.BaudRate = RATE;
            try
            {
                Console.WriteLine("Open port " + port);
                currentPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Data);
            }
            Console.WriteLine(port + " is open");
            return currentPort;
        }

        public void Move(eRod rod, int dc = -1, int servo = 0)
        {
            String action = String.Format("{0}&{1}", dc, servo);
            portList[rod].Write(action);
        }
    }

}