using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer
{
    public class CommunicationFactory
    {
        public static Dictionary<eRod, CommunicationUnit> ConnectedArduinos(Dictionary<eRod, Publisher<RodAction>> publishers)
        {
            Dictionary<eRod, CommunicationUnit> allArduinos = new Dictionary<eRod,CommunicationUnit>();

            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                allArduinos.Add(rodType, null);
            }

            //TODO: Decide how to get all
            string[] portsList = SerialPort.GetPortNames();


            allArduinos[eRod.GoalKeeper] = new CommunicationUnit(publishers[eRod.GoalKeeper], eRod.GoalKeeper, portsList[0]);

            return allArduinos;
        }
    }
}
