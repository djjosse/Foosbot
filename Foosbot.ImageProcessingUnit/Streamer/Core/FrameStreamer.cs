// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using DirectShowLib;
using EasyLog;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Foosbot.Common.Logs;
using System;
using System.Reflection;

namespace Foosbot.ImageProcessingUnit.Streamer.Core
{
    /// <summary>
    /// Frame Streamer Class - contains actual capture device
    /// </summary>
    public class FrameStreamer : FramePublisher
    {
        /// <summary>
        /// Key of camera hardware ID in configuration
        /// </summary>
        private const string HARDWARE_ID_KEY = "CameraHardwareId";

        /// <summary>
        /// Capture device - camera to retrieve frames from
        /// </summary>
        private Capture _capture;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public FrameStreamer()
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
            Log.Print("Video capture started!", eCategory.Info, LogTag.IMAGE);
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
                Log.Print(String.Format("Failed to deal with frame. Reason: {0}", ex.Message), eCategory.Error, LogTag.IMAGE);
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
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].DevicePath.Contains(hardwareId))
                {
                    Log.Print(String.Format("Capture device set: [{0}]", cameras[i].Name), eCategory.Debug, LogTag.IMAGE);
                    return new Capture(i);
                }
            }

            string error = String.Format("Camera device with id: [{0}] not found. Please verify camera is connected.", hardwareId);
            Log.Print(error, eCategory.Error, LogTag.IMAGE);
            throw new ConfigurationException(error);
        }

        /// <summary>
        /// Sets requested camera configuration from Configuration File
        /// </summary>
        protected override void SetCameraConfiguration(int frameWidth = -1, int frameHeight = -1, int frameRate = -1)
        {
            //Set parameters as given or from configuration file
            base.SetCameraConfiguration(frameWidth, frameHeight, frameRate);

            //Apply parameters to capture device
            _capture.FlipHorizontal = true;
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
            Statistics.TryUpdateFrameInfo(frameInfo);
        }

        #endregion protected member functions
    }
}
