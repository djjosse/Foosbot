using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        /// Flag passed to streamer will be set on new frame received
        /// </summary>
        private ManualResetEvent _streamerFrameEvent;

        public MainWindow()
        {
            InitializeComponent();
            Log.Common.Info("Foosbot Application Started!");

            this.Loaded += OnWindowLoaded;
            
        }

        /// <summary>
        /// On window loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //Call the streamer to get capture from camera
            _streamerFrameEvent = new ManualResetEvent(false);
            _streamer = new Streamer(_streamerFrameEvent);

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
                //Wait for streamer to raise event
                _streamerFrameEvent.WaitOne();

                //Get frame
                Foosbot.ImageProcessing.Frame frame = _streamer.CurrentFrame;
                BitmapSource source = frame.ToBitmapSource();
                source.Freeze();

                //Show frame
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    _guiImage.Source = source;
                }));

                Log.Image.Debug("New frame received: " + frame.timestamp.ToString("HH:mm:ss.ffff"));
            }
        }
    }
}
