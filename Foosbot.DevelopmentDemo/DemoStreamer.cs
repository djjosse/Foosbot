// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.ImageProcessingUnit.Streamer.Core;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Foosbot.DevelopmentDemo
{
    /// <summary>
    /// Demo Streamer - generates empty frames for further Demo Unit use
    /// </summary>
    public class DemoStreamer : FramePublisher
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DemoStreamer()
        {
            //Set configuration parameters - frame rate, width and height
            SetCameraConfiguration();

            //Update GUI
            UpdateDiagnosticInfo();
        }

        /// <summary>
        /// Start Method:
        /// 1. Set camera configuration
        /// 2. Start generating empty frame in infinite loop
        /// </summary>
        public override void Start()
        {
            //Generate location and timestamp in loop in new thread
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, z) =>
            {
                ProcessFrame(null, null);
            };
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Generate Empty frames in rate of FrameRate set
        /// in loop with timestamp and notify observers
        /// </summary>
        /// <param name="sender">Irrelevant Pamater</param>
        /// <param name="e">Irrelevant Pamater</param>
        protected override void ProcessFrame(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1 / (double)FrameRate));
                    Frame frame = new Frame();
                    frame.Timestamp = DateTime.Now;

                    Data = frame;
                    NotifyAll();
                }
                catch (Exception ex)
                {
                    Log.Image.Error(String.Format(
                        "[{0}] Error generating frame in demo streamer. Reason: {1}",
                        MethodBase.GetCurrentMethod().Name, ex.Message));
                }
            }
        }

        /// <summary>
        /// Update Diagnostic Info Method for Gui
        /// </summary>
        protected override void UpdateDiagnosticInfo()
        {
            Statistics.UpdateFrameInfo("Streamer is in Demo Mode.");
        }
    }
}
