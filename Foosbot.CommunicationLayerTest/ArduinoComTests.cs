using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foosbot.CommunicationLayer;
using NSubstitute;
using Foosbot.Common.Exceptions;
using System.IO;
using Foosbot.Common.Protocols;
using Foosbot.Common.Enums;
using Foosbot.CommunicationLayer.Contracts;
using Foosbot.CommunicationLayer.Core;

namespace Foosbot.CommunicationLayerTest
{
    [TestClass]
    public class ArduinoComTests
    {
        private const string CATEGORY = "ArduinoCom";

        ISerialPort _mockPort;
        ArduinoCom _arduino;
        IEncoder _mockEncoder;
        int MAX_TICKS = 3100;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockPort = Substitute.For<ISerialPort>();
            _mockEncoder = Substitute.For<IEncoder>();
            _arduino = new ArduinoCom(_mockPort, _mockEncoder)
            {
                RodType = eRod.GoalKeeper
            };
            _arduino.MaxTicks = MAX_TICKS;
        }

        #region InitializeTest

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InitializeTest_PortNotOpen()
        {
            _mockPort.IsOpen.Returns(false);
            _arduino.Initialize();
        }

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InitializeTest_ExceptionOnWriteLine()
        {
            byte initMockByte = 255;
            _mockPort.IsOpen.Returns(true);
            _mockEncoder.EncodeInitialization().Returns(initMockByte);
            _mockPort
                .When(x => x.Write(initMockByte))
                .Do(x => { throw new TimeoutException(); });

            _arduino.Initialize();
            _arduino.RequestCalibration();
        }

        [TestCategory(CATEGORY), TestMethod]
        public void InitializeTest_Positive()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            Assert.IsTrue(_arduino.IsInitialized);
        }

        #endregion InitializeTest

        #region OpenArduinoComPort

        [TestCategory(CATEGORY), TestMethod]
        public void OpenArduinoComPort_Positive()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.OpenArduinoComPort();
            _mockPort.Received(1).Open();
        }

        [TestCategory(CATEGORY), TestMethod]
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

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InitializationException))]
        public void Move_NotInitialized()
        {
            _arduino.Move(200, eRotationalMove.DEFENCE);
        }

        [TestCategory(CATEGORY), TestMethod]
        public void Move_TwiceSameCommandExecutedOnce()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(200, eRotationalMove.DEFENCE);
            _arduino.Move(200, eRotationalMove.DEFENCE);
            //one in Initialize and one in move
            _mockPort.Received(2).Write(Arg.Any<byte>());
        }

        //FIX Following tests:
        //[TestCategory(CATEGORY), TestMethod]
        //public void Move_Write250_0()
        //{
        //    _mockPort.IsOpen.Returns(true);
        //    _arduino.Initialize();
        //    _arduino.Move(250, eRotationalMove.NA);
        //    //todo:
        //    //_mockPort.Received(1).Write("250&0");
        //}

        //[TestCategory(CATEGORY), TestMethod]
        //public void Move_WriteMinus1_2()
        //{
        //    _mockPort.IsOpen.Returns(true);
        //    _arduino.Initialize();
        //    _arduino.Move(-1, eRotationalMove.DEFENCE);
        //    //todo:
        //    //_mockPort.Received(1).Write("-1&2");
        //}

        //[TestCategory(CATEGORY), TestMethod]
        //public void Move_Write333_1()
        //{
        //    _mockPort.IsOpen.Returns(true);
        //    _arduino.Initialize();
        //    _arduino.Move(333, eRotationalMove.KICK);
        //    //todo:
        //    //_mockPort.Received(1).Write("333&1");
        //}

        [TestCategory(CATEGORY), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move_DcSmallerThanMinus1()
        {
            _mockPort.IsOpen.Returns(true);
            _arduino.Initialize();
            _arduino.Move(-5, eRotationalMove.KICK);
        }

        [TestCategory(CATEGORY), TestMethod]
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
