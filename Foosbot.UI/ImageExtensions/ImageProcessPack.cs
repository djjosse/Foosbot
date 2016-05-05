using Foosbot.DevelopmentDemo;
using Foosbot.ImageProcessingUnit.Process.Core;
using Foosbot.ImageProcessingUnit.Streamer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Foosbot.UI.ImageExtensions
{
    public class ImageProcessPack
    {
        private static bool _isCreated = false;

        private bool _isWorking = false;

        public FramePublisher Streamer { get; private set; }

        public FrameUiMonitor UiMonitor { get; private set; }

        public ImagingProcess ImageProcessUnit { get; private set; }

        private ImageProcessPack(FramePublisher framePublisher, FrameUiMonitor uiMonitor, ImagingProcess imagingProcess)
        {
            Streamer = framePublisher;
            UiMonitor = uiMonitor;
            ImageProcessUnit = imagingProcess;
            _isCreated = true;
        }

        ~ImageProcessPack()
        {
            _isCreated = false;
        }

        public void Start()
        {
            if (!_isWorking)
            {
                Streamer.Start();
                UiMonitor.Start();
                ImageProcessUnit.Start();
                _isWorking = true;
            }
        }

        /// <summary>
        /// Selected mode in configuration file property
        /// </summary>
        public static bool IsDemoMode { get; private set; }

        /// <summary>
        /// Builds Image Processing Unit and all relevant components
        /// </summary>
        /// <param name="dispatcher">Dispatcher used to present frames in GUI relevant thread</param>
        /// <param name="screen">Canvas to draw frames on</param>
        /// <returns>ImageProcessPack with all componenets</returns>
        public static ImageProcessPack Create(Dispatcher dispatcher, System.Windows.Controls.Canvas screen)
        {
            if (!_isCreated)
            {
                IsDemoMode = Configuration.Attributes.GetValue<bool>(Configuration.Names.KEY_IS_DEMO_MODE);

                //Frame/Demo streamer
                FramePublisher framePublisher;
                if (!IsDemoMode)
                {
                    framePublisher = new FrameStreamer();
                }
                else
                {
                    framePublisher = new DemoStreamer();
                }

                FrameUiMonitor uiMonitor = new FrameUiMonitor(framePublisher, dispatcher, screen);

                //Initialize Marks
                double widthRate = screen.Width / framePublisher.FrameWidth;
                double heightRate = screen.Height / framePublisher.FrameHeight;
                Marks.Initialize(dispatcher, screen, widthRate, heightRate);

                //Image/Demo processing unit
                ImagingProcess imagingProcess;
                if (!IsDemoMode)
                {
                    imagingProcess = new FrameProcessingUnit(framePublisher);
                }
                else
                {
                    imagingProcess = new DemoProcessingUnit(framePublisher);
                }

                return new ImageProcessPack(framePublisher, uiMonitor, imagingProcess);
            }
            return null;
        }
    }
}
