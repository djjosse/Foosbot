using Foosbot.Common.Enums;
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
        /// <summary>
        /// Create Communication Layer for each connected Arduino
        /// </summary>
        /// <param name="publishers">Dictionary of RodActionPublishers per each rod</param>
        /// <returns>Communication Layer per each Rod</returns>
        public static Dictionary<eRod, CommunicationUnit> Create(Dictionary<eRod, RodActionPublisher> publishers)
        {
            Dictionary<eRod, CommunicationUnit> allArduinos = new Dictionary<eRod,CommunicationUnit>();

            foreach (eRod rodType in Enum.GetValues(typeof(eRod)))
            {
                allArduinos.Add(rodType, null);
            }

            //TODO: Decide how to get all
            string[] portsList = SerialPort.GetPortNames();
            if (portsList.Length < 1) //change to 4
                throw new NotSupportedException("Verify arduino is connected!");

            allArduinos[eRod.GoalKeeper] = new CommunicationUnit(publishers[eRod.GoalKeeper], eRod.GoalKeeper, portsList[0]);
            allArduinos[eRod.GoalKeeper].InitializeRod();

            return allArduinos;
        }
    }
}
