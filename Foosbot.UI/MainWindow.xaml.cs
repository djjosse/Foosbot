// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using EasyLog;
using Foosbot.Common.Enums;
using Foosbot.Common.Logs;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.CommunicationLayer;
using Foosbot.CommunicationLayer.Core;
using Foosbot.DecisionUnit;
using Foosbot.DecisionUnit.Core;
using Foosbot.UI.ImageExtensions;
using Foosbot.UI.ImageTool;
using Foosbot.VectorCalculation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Foosbot.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private members

        LogWindow logWin = new LogWindow("Foosbot Event Log", LogTag.ALL_TAGS);

        /// <summary>
        /// Current Foosbot mode
        /// </summary>
        private bool _isArduinoConnected = false;

        /// <summary>
        /// Imager Processing Pack - gui monitor, streamer, image processing unit
        /// </summary>
        private ImageProcessPack _imageProcessingPack;

        #endregion private members

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            this.Loaded += OnWindowLoaded;
            Closed += Window_Closing;

            EasyLog.Log.Print("Main");
        }

        /// <summary>
        /// On window loaded - Start  Foosbot Application and Threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeStatistics();

                //get operation mode from configuration file
                _isArduinoConnected = Configuration.Attributes.GetValue<bool>(Configuration.Names.KEY_IS_ARDUINOS_CONNECTED);

                //Start Diagnostics - Processor and Memory Usage
                StartDiagnostics();

                _imageProcessingPack = ImageProcessPack.Create(Dispatcher, _guiImage);
                _imageProcessingPack.Start();

                
                VectorCalculationUnit vectorCalcullationUnit = 
                    new VectorCalculationUnit(_imageProcessingPack.ImageProcessUnit.BallLocationUpdater,
                                                _imageProcessingPack.ImageProcessUnit.ImagingData);
                vectorCalcullationUnit.Start();
                
                MainDecisionUnit decisionUnit = new MainDecisionUnit(vectorCalcullationUnit.LastBallLocationPublisher);
                decisionUnit.Start();

                
                if (_isArduinoConnected)
                {
                    Dictionary<eRod, CommunicationUnit> communication = CommunicationFactory.Create(decisionUnit.RodActionPublishers);
                    foreach (eRod key in communication.Keys)
                    {
                        if (communication[key] != null)
                            communication[key].Start();
                    }
                }
                 
            }
            catch(Exception ex)
            {
                MessageBox.Show("Application can not start. Reason: " + ex.Message);
                Close();
            }
        }

        #region Statistics

        public void InitializeStatistics()
        {
            Dictionary<eStatisticsKey, Label> allStatisticElements = new Dictionary<eStatisticsKey, Label>()
            {
                { eStatisticsKey.FrameInfo, _guiFrameInfo },
                { eStatisticsKey.ProccessInfo, _guiProcessInfo },
                { eStatisticsKey.BasicImageProcessingInfo, _guiIpBasicInfo },
                { eStatisticsKey.BallCoordinates, _guiIpBallCoordinates }
            };
            Statistics.Initialize(Dispatcher, allStatisticElements);
        }

        #endregion Statistics

        #region CPU and Memory Diagnostic Info

        /// <summary>
        /// CPU Performance Counter
        /// </summary>
        private PerformanceCounter _CPUCounter = new PerformanceCounter();

        /// <summary>
        /// Memory Performance Counter
        /// </summary>
        private PerformanceCounter _RAMCounter = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName);

        /// <summary>
        /// Show CPU and Memory Usage
        /// </summary>
        private void StartDiagnostics()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, z) =>
            {
                _CPUCounter.CategoryName = "Processor";
                _CPUCounter.CounterName = "% Processor Time";
                _CPUCounter.InstanceName = "_Total";

                while (true)
                {
                    float ramMB = _RAMCounter.NextValue() / 1048576; //Convert bytes to MB
                    string info = String.Format("CPU: {0} % RAM: {1} MB", _CPUCounter.NextValue().ToString(), ramMB.ToString());
                    Statistics.TryUpdateProccessInfo(info);
                    Thread.Sleep(1000);
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion CPU and Memory Diagnostic Info

        private void OpenLog(object sender, RoutedEventArgs e)
        {
            logWin.Visibility = System.Windows.Visibility.Visible;
        }

        private void OpenImageProcessingTool(object sender, RoutedEventArgs e)
        {
            ImageProcessingTool iTool = new ImageProcessingTool(_imageProcessingPack);
            iTool.Show();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            logWin.Dispose();
        }
    }
}
