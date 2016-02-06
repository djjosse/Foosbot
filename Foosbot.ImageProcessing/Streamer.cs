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

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Streams frames from Camera in to Image Processing Unit
    /// </summary>
    /// <author>Joseph Gleyzer</author>
    /// <date>05.02.2016</date>
    public class Streamer
    {
        /// <summary>
        /// Key of camera hardware ID in configuration
        /// </summary>
        private const string HARDWARE_ID = "CameraHardwareId";

        /// <summary>
        /// Capture Device
        /// </summary>
        private Capture _capture;

        /// <summary>
        /// Streamer Diagnostics metadata
        /// </summary>
        public Diagnostics Metadata { get; set; }

        /// <summary>
        /// Current Frame - last frame received
        /// </summary>
        public Frame CurrentFrame { get; set; }

        /// <summary>
        /// On new frame received reset this event
        /// </summary>
        private ManualResetEvent _streamerFrameEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        public Streamer(ManualResetEvent streamerFrameEvent)
        {
            _streamerFrameEvent = streamerFrameEvent;

            _capture = GetCamera();

            SetCameraConfiguration();

            //Subscribe on events
            _capture.ImageGrabbed += ProcessFrame;
            _capture.Start();
            Log.Image.Info("Video capture started!");
        }

        /// <summary>
        /// Streamer main flow
        /// </summary>
        public void ProcessFrame(object sender, EventArgs e)
        {
            //Unsubscribe to stop receiving events
            _capture.ImageGrabbed -= ProcessFrame;
            try
            {
                //Get frame from camera
                Frame frame = new Frame();
                frame.timestamp = DateTime.Now;
                frame.image = new Image<Gray, byte>(_capture.QueryFrame().Clone().Bitmap);

                CurrentFrame = frame;

                //Notify all 
                _streamerFrameEvent.Set();
            }
            catch (Exception ex)
            {
                Log.Image.Error("Failed to deal with frame. Reason: " + ex.Message);
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
        /// <returns>Capture camera device</returns>
        private Capture GetCamera()
        {
            string hardwareId = Configuration.Attributes.GetValue(HARDWARE_ID);

            DsDevice[] cameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

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
        private void SetCameraConfiguration()
        {
            _capture.SetCaptureProperty(CapProp.Fps, 30);
            _capture.SetCaptureProperty(CapProp.FrameWidth, 1000);
            _capture.SetCaptureProperty(CapProp.FrameHeight, 1000);
        }
    }
}