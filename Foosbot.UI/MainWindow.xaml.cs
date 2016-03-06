using Foosbot.Common;
using Foosbot.Common.Logs;
using Foosbot.Common.Multithreading;
using Foosbot.ImageProcessing;
using Foosbot.UI.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Foosbot.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        private ImageProcessingUnit _ipu;

        /// <summary>
        /// Used to draw frame as canvas background
        /// </summary>
        private ImageBrush _imageBrush;


        public MainWindow()
        {
            InitializeComponent();

            //Get canvas size
            ImageFrameWidth = _guiImage.Width;
            ImageFrameHeight = _guiImage.Height;

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
            //Start Log
            StartLogger();

            //Start Diagnostics - Processor and Memory Usage
            StartDiagnostics();

            Log.Common.Info("Foosbot Application Started!");

            //Initialize Markups to Show on Screen
            InitializeMarkUps();
            
            //Call the streamer to get capture from camera
            _streamer = new Streamer(UpdateStatistics);
            _streamer.Start();

            //Call ImageProcessingUnit
            _frameReceiver = new UIFrameObserver(_streamer);
            _frameReceiver.Start();
            _ipu = new ImageProcessingUnit(_streamer, UpdateMarkupCircle, UpdateStatistics);
            _ipu.Start();

            //Show frames in diferent thread
            _imageBrush = new ImageBrush();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, z) =>
            {
                ShowVideoStream();
            };
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Show video stream method
        /// Must run in background thread.
        /// Waits for a new frame in Streamer and shows it continiusly
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

                    //Log.Common.Debug("GUI: New frame received: " + frame.Timestamp.ToString("HH:mm:ss.ffff"));
                }
                catch(Exception e)
                {
                    Log.Common.Error(String.Format("[{0}] Unable to show frame in GUI. Reason: {1}", MethodBase.GetCurrentMethod().Name,e.Message));
                }
            }
        }

        #region LOGGER

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
        /// Log Manager
        /// </summary>
        private UILogManager logger;

        /// <summary>
        /// LogStream - list of messages to display
        /// This property is Binded to GUI
        /// </summary>
        public ObservableCollection<UILogMessage> LogStream { get { return _logMessageList; } }

        /// <summary>
        /// Log messages list to display
        /// </summary>
        public ObservableCollection<UILogMessage> _logMessageList = new ObservableCollection<UILogMessage>();
        
        /// <summary>
        /// Starts Logger in background thread
        /// </summary>
        private void StartLogger()
        {
            logger = new UILogManager();
            AutoscrollCheckbox = true;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, z) =>
            {
                //get current filter
                logger.CurrentFilter = eLogType.NotDefined;
                while (true)
                {
                    //Wait and get last message process it if it's not null
                    UILogMessage message = logger.WaitForMessage();
                    if (message!=null) 
                    {
                        if (logger.CurrentFilter.Equals(message.Type) || logger.CurrentFilter.Equals(eLogType.NotDefined))
                        {
                            Dispatcher.BeginInvoke(new ThreadStart(delegate
                            {
                                _logMessageList.Add(message);
                                if (AutoscrollCheckbox)
                                    _guiLog.ScrollIntoView(_logMessageList.Last());
                            }));
                        }
                    }
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion LOGGER

        #region Markup, Statistics, Diagnostics related

        /// <summary>
        /// Markup shape dictionary
        /// Contains markup key and related shape
        /// </summary>
        Dictionary<Helpers.eMarkupKey, FrameworkElement> _markups;

        /// <summary>
        /// Must be called to use markups
        /// </summary>
        private void InitializeMarkUps()
        {
            _markups = new Dictionary<Helpers.eMarkupKey, FrameworkElement>();

            foreach (Helpers.eMarkupKey key in Enum.GetValues(typeof(Helpers.eMarkupKey)))
            {
                switch (key)
                {
                    case Helpers.eMarkupKey.BALL_CIRCLE_MARK:
                    case Helpers.eMarkupKey.BUTTOM_LEFT_CALLIBRATION_MARK:
                    case Helpers.eMarkupKey.BUTTOM_RIGHT_CALLIBRATION_MARK:
                    case Helpers.eMarkupKey.TOP_LEFT_CALLIBRATION_MARK:
                    case Helpers.eMarkupKey.TOP_RIGHT_CALLIBRATION_MARK:
                        _markups.Add(key, new Ellipse());
                        break;
                    case Helpers.eMarkupKey.BUTTOM_LEFT_CALLIBRATION_TEXT:
                    case Helpers.eMarkupKey.BUTTOM_RIGHT_CALLIBRATION_TEXT:
                    case Helpers.eMarkupKey.TOP_LEFT_CALLIBRATION_TEXT:
                    case Helpers.eMarkupKey.TOP_RIGHT_CALLIBRATION_TEXT:
                        _markups.Add(key, new TextBlock());
                        break;
                }
                _guiImage.Children.Add(_markups[key]);
            }
        }

        private double ImageFrameWidth;
        private double ImageFrameHeight;

        /// <summary>
        /// On Update markup function
        /// </summary>
        /// <param name="key">Markup key</param>
        /// <param name="center">Circle center point</param>
        /// <param name="radius">Circle radius</param>
        public void UpdateMarkupCircle(Helpers.eMarkupKey key, Point center, int radius)
        {
            double widthRate = ImageFrameWidth / _streamer.FrameWidth;
            double heightRate = ImageFrameHeight / _streamer.FrameHeight;

            Point presentationCenter = new Point(center.X * widthRate, center.Y * heightRate);
            int presentationRadius = Convert.ToInt32(radius*((widthRate+heightRate)/2));

            Dispatcher.Invoke(new ThreadStart(delegate
            {       
                switch (key)
                {
                    case Helpers.eMarkupKey.BALL_CIRCLE_MARK:
                        (_markups[key] as Shape).StrokeThickness = 2;
                        (_markups[key] as Shape).Stroke = System.Windows.Media.Brushes.Red;
                        break;
                    case Helpers.eMarkupKey.BUTTOM_LEFT_CALLIBRATION_MARK:
                    case Helpers.eMarkupKey.BUTTOM_RIGHT_CALLIBRATION_MARK:
                    case Helpers.eMarkupKey.TOP_LEFT_CALLIBRATION_MARK:
                    case Helpers.eMarkupKey.TOP_RIGHT_CALLIBRATION_MARK:
                        (_markups[key] as Shape).StrokeThickness = 2;
                        (_markups[key] as Shape).Stroke = System.Windows.Media.Brushes.Green;
                        break;
                    case Helpers.eMarkupKey.BUTTOM_LEFT_CALLIBRATION_TEXT:
                        (_markups[key] as TextBlock).FontSize = 9;
                        (_markups[key] as TextBlock).Text = String.Format("BL:{0}x{1}", Convert.ToInt32(center.X), Convert.ToInt32(center.Y));
                        (_markups[key] as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        break;
                    case Helpers.eMarkupKey.BUTTOM_RIGHT_CALLIBRATION_TEXT:
                        (_markups[key] as TextBlock).FontSize = 9;
                        (_markups[key] as TextBlock).Text = String.Format("BR:{0}x{1}", Convert.ToInt32(center.X), Convert.ToInt32(center.Y));
                        (_markups[key] as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        break;
                    case Helpers.eMarkupKey.TOP_LEFT_CALLIBRATION_TEXT:
                        (_markups[key] as TextBlock).FontSize = 9;
                        (_markups[key] as TextBlock).Text = String.Format("TL:{0}x{1}", Convert.ToInt32(center.X), Convert.ToInt32(center.Y));
                        (_markups[key] as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        break;
                    case Helpers.eMarkupKey.TOP_RIGHT_CALLIBRATION_TEXT:
                        (_markups[key] as TextBlock).FontSize = 9;
                        (_markups[key] as TextBlock).Text = String.Format("TR:{0}x{1}", Convert.ToInt32(center.X), Convert.ToInt32(center.Y));
                        (_markups[key] as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        break;
                    default:
                        break;
                }
                
                _markups[key].Width = presentationRadius*2;
                _markups[key].Height = presentationRadius*2;
                Canvas.SetLeft(_markups[key], presentationCenter.X - presentationRadius);
                Canvas.SetTop(_markups[key], presentationCenter.Y - presentationRadius);
            }));
        }

        public void UpdateStatistics(Helpers.eStatisticsKey key, string info)
        {
            Dispatcher.Invoke(new ThreadStart(delegate
            {
                switch(key)
                {
                    case Helpers.eStatisticsKey.FrameInfo:
                        _guiFrameInfo.Content = info;
                        break;
                    case Helpers.eStatisticsKey.ProccessInfo:
                        _guiProcessInfo.Content = info;
                        break; 
                }
            }));
        }

        #endregion Markup, Statistics, Diagnostics related
        
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
                    UpdateStatistics(Helpers.eStatisticsKey.ProccessInfo, info);
                    Thread.Sleep(1000);
                }
            };
            worker.RunWorkerAsync();
        }

        #endregion CPU and Memory Diagnostic Info
    }
}
