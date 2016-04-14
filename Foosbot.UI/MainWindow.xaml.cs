// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using DevDemos;
using Foosbot.Common.Logs;
using Foosbot.Common.Multithreading;
using Foosbot.Common.Protocols;
using Foosbot.CommunicationLayer;
using Foosbot.DecisionUnit;
using Foosbot.DecisionUnit.Core;
using Foosbot.ImageProcessing;
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

        /// <summary>
        /// Streamer to get frames from
        /// </summary>
        private Streamer _streamer;

        /// <summary>
        /// Receives frames from streamer and shows them
        /// </summary>
        private UIFrameObserver _frameReceiver;

        /// <summary>
        /// Image Processing Unit to work with
        /// </summary>
        private AbstractImageProcessingUnit _ipu;

        /// <summary>
        /// Used to draw frame as canvas background
        /// </summary>
        private ImageBrush _imageBrush;

        /// <summary>
        /// Current Foosbot mode
        /// </summary>
        private bool _isDemoMode = false;

        /// <summary>
        /// Current Foosbot mode
        /// </summary>
        private bool _isArduinoConnected = false;

        #endregion private members

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            this.Loaded += OnWindowLoaded;
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
                _isDemoMode = Configuration.Attributes.GetValue<bool>(Configuration.Names.KEY_IS_DEMO_MODE);
                _isArduinoConnected = Configuration.Attributes.GetValue<bool>(Configuration.Names.KEY_IS_ARDUINOS_CONNECTED);

                //Set canvas background as green image
                _guiImage.Background = System.Windows.Media.Brushes.Green;

                //Init Gui Log
                AutoscrollCheckbox = true;
                Log.InitializeGuiLog(UpdateLog);
                string startMessage = Configuration.Attributes.GetValue<string>("startMessage");
                Log.Common.Debug(startMessage);

                //Start Diagnostics - Processor and Memory Usage
                StartDiagnostics();

                //Call the streamer to get capture from camera
                if (!_isDemoMode) _streamer = new RealStreamer();
                else _streamer = new DemoStreamer();

                
                Marks.Initialize(Dispatcher, _guiImage, _guiImage.Width / _streamer.FrameWidth, _guiImage.Height / _streamer.FrameHeight);
                _streamer.Start();

                //Call ImageProcessingUnit
                _frameReceiver = new UIFrameObserver(_streamer);
                _frameReceiver.Start();
                if (!_isDemoMode) 
                    _ipu = new ImageProcessingUnit(_streamer);
                else 
                    _ipu = new DemoImageProcessingUnit(_streamer as DemoStreamer);
                _ipu.Start();

                //Show frames in different thread
                _imageBrush = new ImageBrush();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, z) =>
                {
                    ShowVideoStream();
                };
                worker.RunWorkerAsync();

               // BackgroundWorker worker2 = new BackgroundWorker();
               // worker2.DoWork += (s, z) =>
               // {
              //      while (!_ipu.IsCallibrated) { Thread.Sleep(100);/* wait till calibrated*/}

                    VectorCallculationUnit vectorCalcullationUnit = new VectorCallculationUnit(_ipu.LastBallLocationPublisher, _ipu.BallRadius);
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
              //  };
              //  worker2.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Application can not start. Reason: " + ex.Message);
                Close();
            }
        }

        /// <summary>
        /// Show video stream method
        /// Must run in background thread.
        /// Waits for a new frame in Streamer and shows it continuously
        /// </summary>
        public void ShowVideoStream()
        {
            while (true)
            {
                try
                {
                    while (!_frameReceiver.Set) { }

                    //Get frame
                    Foosbot.ImageProcessing.Frame frame = _streamer.Data;
                    BitmapSource source = frame.ToBitmapSource();
                    source.Freeze();

                    //Show frame
                    Dispatcher.BeginInvoke(new ThreadStart(delegate
                    {
                        _imageBrush.ImageSource = source;
                        _guiImage.Background = _imageBrush;
                    }));
                }
                catch(Exception e)
                {
                    Log.Common.Error(String.Format("[{0}] Unable to show frame in GUI. Reason: {1}", MethodBase.GetCurrentMethod().Name,e.Message));
                }
            }
        }

        #region LOGGER

        /// <summary>
        /// Update Log function to be passed as delegate
        /// </summary>
        /// <param name="type">Log type</param>
        /// <param name="category">Log category</param>
        /// <param name="timestamp">Log timestamp</param>
        /// <param name="message">Log message</param>
        public void UpdateLog(eLogType type, eLogCategory category, DateTime timestamp, string message)
        {
            UILogMessage logmessage = new UILogMessage();
            logmessage.FontColor = DefineColor(category);
            logmessage.Message = DefineMessage(type, category, timestamp, message);
            logmessage.Type = type;
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                _logMessageList.Add(logmessage);
                if (AutoscrollCheckbox)
                    _guiLog.ScrollIntoView(_logMessageList.Last());
            }));
        }

        /// <summary>
        /// Message to printable format
        /// </summary>
        /// <param name="message">Message to format</param>
        /// <param name="type">Type of log</param>
        /// <returns>Message as string</returns>
        private string DefineMessage(eLogType type, eLogCategory category, DateTime timestamp, string message)
        {
            return String.Format("{0}\t{1}\t{3}", timestamp.ToString("HH:mm:ss:fff"), category, type, message);
        }

        /// <summary>
        /// Defines color for specific message category
        /// </summary>
        /// <param name="category">Message Category as string</param>
        /// <returns></returns>
        private Color DefineColor(eLogCategory category)
        {
            switch (category)
            {
                case eLogCategory.Error:
                    return Colors.Red;
                case eLogCategory.Info:
                    return Colors.DarkGreen;
                case eLogCategory.Warn:
                    return Colors.DarkOrange;
                default:
                    return Colors.Black;
            }
        }

        /// <summary>
        /// Log Autoscrolling checkbox value
        /// </summary>
        public bool AutoscrollCheckbox
        {
            get { return (bool)GetValue(AutoscrollCheckboxProperty); }
            set { SetValue(AutoscrollCheckboxProperty, value); }
        }

        /// <summary>
        /// Log Autoscrolling checkbox properties
        /// </summary>
        public static readonly DependencyProperty AutoscrollCheckboxProperty =
           DependencyProperty.Register("AutoscrollCheckbox", typeof(bool),
             typeof(MainWindow), new UIPropertyMetadata(false));

        /// <summary>
        /// LogStream - list of messages to display
        /// This property is Binded to GUI
        /// </summary>
        public ObservableCollection<UILogMessage> LogStream { get { return _logMessageList; } }

        /// <summary>
        /// Log messages list to display
        /// </summary>
        public ObservableCollection<UILogMessage> _logMessageList = new ObservableCollection<UILogMessage>();
        
        #endregion LOGGER

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
                    Statistics.UpdateProccessInfo(info);
                    Thread.Sleep(1000);
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion CPU and Memory Diagnostic Info

    }
}
