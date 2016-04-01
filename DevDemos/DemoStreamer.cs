// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot;
using Foosbot.Common.Multithreading;
using Foosbot.ImageProcessing;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace DevDemos
{
    public class DemoStreamer : Streamer
    {
        public Observer<Frame> DemoImageProcessingUnit { get; set; }

        public DemoStreamer()
        {
            //you can use if you want
            FrameWidth = Configuration.Attributes.GetValue<int>("FrameWidth");
            FrameHeight = Configuration.Attributes.GetValue<int>("FrameHeight");
            FrameRate = Configuration.Attributes.GetValue<int>("FrameRate");
        }

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

        protected override void ProcessFrame(object sender, EventArgs e)
        {
            //you can use if you want
            while (true)
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1 / (double)FrameRate));
                    Frame frame = new Frame();
                    frame.Timestamp = DateTime.Now;

                    Data = frame;
                    Notify(DemoImageProcessingUnit);

                    UpdateDiagnosticInfo();
                }
                catch (Exception ex)
                {
                    Log.Image.Error(String.Format(
                        "[{0}] Error generating frame in demo streamer. Reason: {1}",
                        MethodBase.GetCurrentMethod().Name, ex.Message));
                }
            }
        }

        protected override void UpdateDiagnosticInfo()
        {
            //you can use if you want
            Statistics.UpdateFrameInfo(String.Format(
                "Generating frames in DevDemo: {0}", DateTime.Now.ToString("ss:ffff")));
        }
    }
}
