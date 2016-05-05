// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using Foosbot.Common.Multithreading;
using Foosbot.ImageProcessingUnit.Process.Core;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Foosbot.UI.ImageExtensions
{
    /// <summary>
    /// Present images received from Frame Publisher instance in GUI
    /// </summary>
    public class FrameUiMonitor : FrameObserver, IInitializable
    {
        /// <summary>
        /// Dispatcher instance used to show frame canvas thread
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// Canvas to draw image on
        /// </summary>
        private System.Windows.Controls.Canvas _canvas;

        /// <summary>
        /// Used to draw frame as canvas background
        /// </summary>
        private ImageBrush _imageBrush;

        /// <summary>
        /// Used to avoid receiving and displaying same frame twice
        /// </summary>
        private DateTime _lastFrameTimeStamp;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="publisher">Frame publisher instance</param>
        /// <param name="dispatcher">Dispatcher instance used to show frame canvas thread</param>
        /// <param name="screen">Canvas to draw image on</param>
        public FrameUiMonitor(Publisher<IFrame> publisher, Dispatcher dispatcher, System.Windows.Controls.Canvas screen)
            : base(publisher)
        {
            _dispatcher = dispatcher;
            _canvas = screen;
            _imageBrush = new ImageBrush(); 
        }

        /// <summary>
        /// Present Image each time new one given
        /// </summary>
        public override void Job()
        {
            try
            {
                _publisher.Dettach(this);
                Initialize();
                
                //Get frame
                using (IFrame frame = _publisher.Data)
                {
                    //Verify it is really a new frame and it is not empty
                    if (frame != null &&
                        !frame.Timestamp.Equals(_lastFrameTimeStamp) &&
                        frame.Image != null)
                    {
                        _lastFrameTimeStamp = frame.Timestamp;

                        BitmapSource source = frame.ToBitmapSource();
                        source.Freeze();

                        //Show frame
                        _dispatcher.Invoke(new ThreadStart(delegate
                        {
                            _imageBrush.ImageSource = source;
                            _canvas.Background = _imageBrush;
                        }));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Common.Error(String.Format("[{0}] Unable to show frame in GUI. Reason: {1}", MethodBase.GetCurrentMethod().Name, e.Message));
            }
            finally
            {
                _publisher.Attach(this);
            }
        }

        /// <summary>
        /// Is Initialized property
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initialization Method:
        /// 1. Set canvas background image as green
        /// </summary>
        public void Initialize()
        {
            if (!IsInitialized)
            {
                _dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            _canvas.Background = System.Windows.Media.Brushes.Green;
                        }));

                IsInitialized = true;
            }
        }
    }
}
