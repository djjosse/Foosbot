// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.ImageProcessingUnit.Detection.Core;
using Foosbot.ImageProcessingUnit.Process.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using Foosbot.ImageProcessingUnit.Streamer.Core;
using Foosbot.ImageProcessingUnit.Tools.Contracts;
using Foosbot.ImageProcessingUnit.Tools.Core;
using Foosbot.ImageProcessingUnit.Tools.Enums;
using System;

namespace Foosbot.ImageProcessingUnit.Process.Core
{
    /// <summary>
    /// Actual Image Processing Unit
    /// This Unit is fully responsible for Image Processing in Foosbot
    /// It performs calibration, sets common Image Parameters and
    /// performs ball detection and tracking every time new image received from camera streamer
    /// </summary>
    public class FrameProcessingUnit : ImagingProcess
    {
        /// <summary>
        /// Last frame received from streamer
        /// </summary>
        private IFrame _currentFrame;

        /// <summary>
        /// Calibration Unit Instance
        /// </summary>
        private ICalibration _calibration;

        /// <summary>
        /// Ball Tracker
        /// </summary>
        private Tracker _ballTracker;

        /// <summary>
        /// Last Received Frame Time stamp
        /// </summary>
        private DateTime _lastFrameTimeStamp;

        /// <summary>
        /// Detection Statistics Analyzer tool
        /// </summary>
        public IDetectionAnalyzer AnalyzerTool { get; private set; }

        /// <summary>
        /// Image Processing Unit Constructor
        /// </summary>
        /// <param name="streamer">Streamer to get frames from</param>
        /// <param name="ballTracker">Ball Tracker [default is null - will be created]</param>
        /// <param name="imagingData">Imaging data [default is null - will be created]</param>
        /// <param name="analyzerTool">Statistics Tool [default is null - will be created]</param>
        public FrameProcessingUnit(FramePublisher streamer, Tracker ballTracker = null, IImageData imagingData = null, IDetectionAnalyzer analyzerTool = null)
            : base(streamer, imagingData) 
        {
            _ballTracker = ballTracker ?? new BallTracker(ImagingData, streamer);
            AnalyzerTool = analyzerTool ?? new DetectionStatisticAnalyzer();
            _lastFrameTimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Initialization method calls calibration mechanism till finished
        /// At the end when calibration is finished it also sets IsInitialized property to True
        /// </summary>
        public override void Initialize()
        {
            if (!IsInitialized)
            {
                if (_calibration == null)
                {
                    _calibration = new CalibrationUnit(ImagingData);
                    SetMonitors();
                }

                _calibration.Calibrate(_currentFrame);

                if (_calibration.CurrentState.Equals(eCalibrationState.Finished))
                    IsInitialized = true;
            }
        }

        /// <summary>
        /// Loop method to run
        /// </summary>
        public override void Job()
        {
            try
            {
                _publisher.Dettach(this);
                using (_currentFrame = _publisher.Data.Clone())
                {

                    VerifyDifferentFrameByTimeStamp(_currentFrame.Timestamp);

                    //Performs calibration if required
                    Initialize();
                    if (IsInitialized)
                    {
                        AnalyzerTool.Begin();
                        bool detectionResult = _ballTracker.Detect(_currentFrame);
                        AnalyzerTool.Finalize(detectionResult);
                        BallLocationUpdater.UpdateAndNotify();
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Image.Debug("Exception in image processing flow: " + ex.Message);
            }
            finally
            {
                _publisher.Attach(this);
            }
        }

        /// <summary>
        /// Verify Different Frame received by time stamp
        /// </summary>
        /// <param name="currentFrameTime">Current frame time stamp</param>
        /// <exception cref="InvalidOperationException">Thrown in case frame with such time stamp already received</exception>
        private void VerifyDifferentFrameByTimeStamp(DateTime currentFrameTime)
        {
            if (currentFrameTime == _lastFrameTimeStamp)
                throw new InvalidOperationException("Received same image twice.");
        }

        /// <summary>
        /// Set Computer Vision Monitors
        /// </summary>
        private void SetMonitors()
        {
            _calibration.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorA, ImageProcessingMonitorA);
            _calibration.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorB, ImageProcessingMonitorB);
            _calibration.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorC, ImageProcessingMonitorC);
            _calibration.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorD, ImageProcessingMonitorD);
            _ballTracker.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorA, ImageProcessingMonitorA);
            _ballTracker.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorB, ImageProcessingMonitorB);
            _ballTracker.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorC, ImageProcessingMonitorC);
            _ballTracker.ComputerVisionMonitors.Add(eComputerVisionMonitor.MonitorD, ImageProcessingMonitorD);
            _ballTracker.MotionInspector.MotionMonitor = ImageProcessingMonitorD;
        }
    }
}
