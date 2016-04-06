using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Foosbot.CommunicationLayer
{
    public class CommunicationUnit : Observer<RodAction>
    {
        private ArduinoCom _arduino;
        private eRod _rodType;
        private double _rodLength; //mm
        private int _ticksPerRod = 3100;
        private const int TICKS_BUFFER = 100;

        public CommunicationUnit(Publisher<RodAction> publisher, eRod rodType, string comPort)
            : base(publisher)
        {
            _publisher.Dettach(this);

            _rodType = rodType;

            _arduino = new ArduinoCom(comPort);
            _arduino.OpenArduinoComPort();
            _arduino.InitializeArduino();

            _rodLength = Configuration.Attributes.GetRodDistanceBetweenStoppers(_rodType);

            _publisher.Attach(this);
        }

        public override void Job()
        {
            try
            {
                _publisher.Dettach(this);

                RodAction action = _publisher.Data;

                Log.Common.Debug(String.Format("[{0}] New action received for {1} Rotational: {2} mm Linear: {3}: {4} mm",
                   MethodBase.GetCurrentMethod().Name, action.Type.ToString(), action.Rotation.ToString(),
                    action.Linear.ToString(), action.LinearMovement));

                int proportinalMove = ConvertLengthToTicks(action.LinearMovement);
                _arduino.Move(proportinalMove, action.Rotation);

            }
            catch (ThreadInterruptedException)
            {
                /* Got new data */
            }
            catch (Exception ex)
            {
                Log.Common.Error(String.Format("[{0}] Error occurred! Reason: [{1}]",
                    MethodBase.GetCurrentMethod().Name, ex.Message));
            }
            finally
            {
                _publisher.Attach(this);
            }
        }

        private int ConvertLengthToTicks(double movement)
        {
            int ticks = Convert.ToInt32((movement / _rodLength) * _ticksPerRod);
            if (ticks >= _ticksPerRod - TICKS_BUFFER)
                ticks = _ticksPerRod - TICKS_BUFFER;
            if (ticks <= TICKS_BUFFER)
                ticks = TICKS_BUFFER;
            ticks = _ticksPerRod - ticks;
            return ticks;
        }
    }
}
