using Foosbot.Common.Exceptions;
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
    /// <summary>
    /// Communication Unit to observe the publisher of RodAction and sent commands to Arduino Rod
    /// </summary>
    public class CommunicationUnit : Observer<RodAction>
    {
        #region private members

        private ArduinoCom _arduino;
        private eRod _rodType;
        private double _rodLength; //mm
        private int _ticksPerRod;
        private const int TICKS_BUFFER = 100;
        private bool _isInitialized = false;

        #endregion private members

        /// <summary>
        /// Communication Layer Constructor
        /// </summary>
        /// <param name="publisher">Rod Action publisher to observe</param>
        /// <param name="rodType">Current rod type</param>
        /// <param name="comPort">Com port of current arduino rod</param>
        public CommunicationUnit(Publisher<RodAction> publisher, eRod rodType, string comPort)
            : base(publisher)
        {
            //we don't wan't to receive anything before initialization finished
            _publisher.Dettach(this);

            //set rod type for current rod
            _rodType = rodType;

            //Create arduino com object
            _arduino = new ArduinoCom(comPort);
        }

        /// <summary>
        /// Initialization method. 
        /// Open arduino and init it. Read configuration file.
        /// </summary>
        public void InitializeRod()
        {
            if (!_isInitialized)
            {
                //Create and Initialize Arduino to work with
                _arduino.OpenArduinoComPort();
                _arduino.Initialize();

                //get rod length in mm and ticks from configuration file
                _rodLength = Configuration.Attributes.GetRodDistanceBetweenStoppers(_rodType);
                _ticksPerRod = Configuration.Attributes.GetTicksPerRod(_rodType);

                _arduino.MaxTicks = _ticksPerRod;

                _isInitialized = true;

                //subscribe to new commands
                _publisher.Attach(this);
            }
        }

        /// <summary>
        /// Main Flow for communication unit
        /// </summary>
        public override void Job()
        {
            try
            {
                if (!_isInitialized)
                    throw new InitializationException(String.Format(
                        "[{0}] Instance was not initialized. Must call initialization method before used!",
                            this.GetType().DeclaringType));

                //we don't wan't to receive new data while not finished with old one
                _publisher.Dettach(this);

                //received command
                RodAction action = _publisher.Data;

                //Log.Common.Debug(String.Format("[{0}] New action received for {1} Rotational: {2} Linear: {3}: Coordinate: {4} mm",
                //   MethodBase.GetCurrentMethod().Name, action.RodType.ToString(), action.Rotation.ToString(),
                //    action.Linear.ToString(), action.DcCoordinate));

                //Convert mm to ticks
                int proportinalMove = ConvertLengthToTicks(action.DcCoordinate);

                //Invoke the arduino
                _arduino.Move(proportinalMove, action.Rotation);

            }
            catch (ThreadInterruptedException)
            {
                /* Got new data */
            }
            catch (InitializationException)
            {
                throw;
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

        /// <summary>
        /// Convert Length from mm to ticks
        /// </summary>
        /// <param name="movement">Position to go to from start of the rod in mm</param>
        /// <returns>Position to go to from start of the rod in ticks</returns>
        private int ConvertLengthToTicks(double movement)
        {
            int ticks = Convert.ToInt32((movement / _rodLength) * _ticksPerRod);
            if (ticks >= _ticksPerRod - TICKS_BUFFER)
                ticks = _ticksPerRod - TICKS_BUFFER;
            if (ticks <= TICKS_BUFFER)
                ticks = TICKS_BUFFER;
            ticks = _ticksPerRod; // -ticks;
            return ticks;
        }
    }
}
