using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using System.Threading;
using Foosbot.Common.Multithreading;
using System.Reflection;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Streams frames from Camera in to Image Processing Unit
    /// </summary>
    /// <author>Joseph Gleyzer</author>
    /// <date>05.02.2016</date>
    public class RealStreamer : Streamer
    {
        #region Constans

        /// <summary>
        /// Key of camera hardware ID in configuration
        /// </summary>
        private const string HARDWARE_ID_KEY = "CameraHardwareId";

        #endregion Constans

        #region private and protected members

        /// <summary>
        /// Capture Device
        /// </summary>
        protected Capture _capture;

        #endregion private and protected members

        /// <summary>
        /// Constructor
        /// </summary>
        public RealStreamer(Helpers.UpdateStatisticsDelegate onUpdateStatistics)
            : base(onUpdateStatistics)
        {
            //Get camera device and set configuration
            _capture = GetCamera();
            SetCameraConfiguration();
        }

        /// <summary>
        /// Start streaming
        /// </summary>
        public override void Start()
        {
            //Subscribe on new frame event
            _capture.ImageGrabbed += ProcessFrame;
            _capture.Start();
            Log.Image.Info("Video capture started!");
            UpdateDiagnosticInfo();
        }

        #region protected member functions

        /// <summary>
        /// Frame Process Function called on Image Grabbed Event
        /// </summary>
        protected override void ProcessFrame(object sender, EventArgs e)
        {
            //Unsubscribe to stop receiving events
            _capture.ImageGrabbed -= ProcessFrame;
            try
            {
                Mat f = new Mat();
                _capture.Retrieve(f, 0);
                //Get frame from camera
                Frame frame = new Frame();
                frame.Timestamp = DateTime.Now;
                frame.Image = new Image<Gray, byte>(f.Bitmap);

                Data = frame;
                NotifyAll();
            }
            catch (Exception ex)
            {
                Log.Image.Error(String.Format(
                    "[{0}] Failed to deal with frame. Reason: {1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            }
            finally
            {
                //Subscribe back to receive events
                _capture.ImageGrabbed += ProcessFrame;
            }
        }

        /// <summary>
        /// Get Camera Device by device hardware Id
        /// </summary>
        protected Capture GetCamera()
        {
            string hardwareId = Configuration.Attributes.GetValue(HARDWARE_ID_KEY);

            DsDevice[] cameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            CvInvoke.UseOpenCL = true; 
            for (int i = 0; i< cameras.Length; i++)
            {
                if (cameras[i].DevicePath.Contains(hardwareId))
                {
                    Log.Image.Debug(String.Format("Capture device set: [{0}]", cameras[i].Name));
                    return new Capture(i);
                }
            }

            string error = String.Format("Camera device with id: [{0}] not found. Please verify camera is connected.", hardwareId);
            Log.Image.Error(error);
            throw new ConfigurationException(error);
        }

        /// <summary>
        /// Sets requested camera configuration
        /// </summary>
        protected void SetCameraConfiguration()
        {
            FrameWidth = Configuration.Attributes.GetValue<int>("FrameWidth");
            FrameHeight = Configuration.Attributes.GetValue<int>("FrameHeight");
            FrameRate = Configuration.Attributes.GetValue<int>("FrameRate");

            //_capture.FlipHorizontal = true;
            _capture.SetCaptureProperty(CapProp.FrameWidth, FrameWidth);
            _capture.SetCaptureProperty(CapProp.FrameHeight, FrameHeight);
            _capture.SetCaptureProperty(CapProp.Fps, FrameRate);
        }

        /// <summary>
        /// Update Streamer Diagnostic Info - FPS, Frame Width and Height
        /// </summary>
        protected override void UpdateDiagnosticInfo()
        {
            string frameInfo = String.Format("Frame Size: {0}x{1} F/S: {2}",
                _capture.GetCaptureProperty(CapProp.FrameWidth).ToString(),
                _capture.GetCaptureProperty(CapProp.FrameHeight).ToString(),
                _capture.GetCaptureProperty(CapProp.Fps).ToString());
            UpdateStatistics(Helpers.eStatisticsKey.FrameInfo, frameInfo);
        }

        #endregion protected member functions
    }
}