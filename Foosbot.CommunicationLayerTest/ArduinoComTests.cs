using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foosbot.CommunicationLayer;
using NSubstitute;
using Foosbot.Common.Exceptions;
using System.IO;
using Foosbot.Common.Protocols;

namespace Foosbot.CommunicationLayerTest
{
    [TestClass]
    public class ArduinoComTests
    {
        ISerialPort _mockPort;
        ArduinoCom _arduino;
        int MAX_TICKS = 3100;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockPort = Substitute.For<ISerialPort>();
            _arduino = new ArduinoCom(_mockPort);
            _arduino.MaxTicks = MAX_TICKS;
        }

        #region InitializeTest

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InitializeTest_PortNotOpen()
        {
            _mockPort.IsOpen.Returns(false);
            _arduino.Initialize();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InitializeTest_ExceptionOnWriteLine()
        {
            _mockPort.IsOpen.Returns(true);
            _mockPort
                .When(x => x.WriteLine(ArduinoCom.KEY_INIT))
                .Do(x => { throw new TimeoutException(); });

            _arduino.Initialize();
        }

        [TestMethod]
        public void InitializeTest_Positive()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            Assert.IsTrue(_arduino.IsInitialized);
        }

        #endregion InitializeTest

        #region OpenArduinoComPort

        [TestMethod]
        public void OpenArduinoComPort_Positive()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.OpenArduinoComPort();
            _mockPort.Received(1).Open();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OpenArduinoComPort_Negative()
        {
            _mockPort.IsOpen.Returns(true); 
            _mockPort
                 .When(x => x.Open())
                 .Do(x => { throw new IOException(); });
            _arduino.OpenArduinoComPort();
        }

        #endregion OpenArduinoComPort

        #region Move

        [TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void Move_NotInitialized()
        {
            _arduino.Move(200, eRotationalMove.DEFENCE);
        }

        [TestMethod]
        public void Move_TwiceSameCommandExecutedOnce()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(200, eRotationalMove.DEFENCE);
            _arduino.Move(200, eRotationalMove.DEFENCE);
            _mockPort.Received(1).Write(Arg.Any<string>());
        }

        [TestMethod]
        public void Move_Write250_0()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(250, eRotationalMove.NA);
            _mockPort.Received(1).Write("250&0");
        }

        [TestMethod]
        public void Move_WriteMinus1_2()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(-1, eRotationalMove.DEFENCE);
            _mockPort.Received(1).Write("-1&2");
        }

        [TestMethod]
        public void Move_Write333_1()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(333, eRotationalMove.KICK);
            _mockPort.Received(1).Write("333&1");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move_DcSmallerThanMinus1()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(-5, eRotationalMove.KICK);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move_DcBiggerThanMaxTicks()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(MAX_TICKS + 100, eRotationalMove.KICK);
        }

        #endregion Move
    }
}
