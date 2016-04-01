// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using System;
using System.Diagnostics;

namespace Foosbot.ImageProcessing
{
    /// <summary>
    /// Detection Statistical Analyzer for image processing unit
    /// </summary>
    public class DetectionStatisticAnalyzer
    {
        #region Private Members

        /// <summary>
        /// Current Working Second timestamp
        /// </summary>
        private DateTime _currenWorkingSecond;

        /// <summary>
        /// Total Frames received in last second
        /// </summary>
        private int _totalFramesPerSecond;

        /// <summary>
        /// Total Successfull detection for last second
        /// </summary>
        private int _successDetectionFrame;

        /// <summary>
        /// Time spent on ball location detection in past second
        /// </summary>
        private TimeSpan _spentOnDetectionInSecond;

        /// <summary>
        /// Detection Stopwatch
        /// </summary>
        private Stopwatch detectionWatch;

        #endregion Private Members

        /// <summary>
        /// Constructor
        /// </summary>
        public DetectionStatisticAnalyzer()
        { 
            _currenWorkingSecond = DateTime.Now;
        }

        /// <summary>
        /// Steps to perform each detection started
        /// 1. Count frame
        /// 2. Start detection stopwatch
        /// If not same second as in prevoius frame then update statistics and start from the beginning
        /// </summary>
        public void Next()
        {
            DateTime now = DateTime.Now;
            if (_currenWorkingSecond.Second != now.Second)
            {
                double rate = (_totalFramesPerSecond < 1) ? 100 : 100 * _successDetectionFrame / _totalFramesPerSecond;
                double aveDetectTime = (_totalFramesPerSecond < 1) ? 0 : _spentOnDetectionInSecond.Milliseconds / _totalFramesPerSecond;
                Statistics.UpdateBasicImageProcessingInfo(String.Format("Detection: Rate {0}% ({1}/{2}) Average T {3}(ms)",
                        rate, _successDetectionFrame, _totalFramesPerSecond, aveDetectTime));
                _totalFramesPerSecond = 0;
                _successDetectionFrame = 0;
                _spentOnDetectionInSecond = TimeSpan.FromMilliseconds(0);
                _currenWorkingSecond = now;
            }
            _totalFramesPerSecond++;
            detectionWatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Steps to perform after each detection finished
        /// 1. Count detection if succesfull
        /// 2. Stop the detection stopwatch
        /// </summary>
        /// <param name="isBallLocationFound">Detection result</param>
        public void Finalize(bool isBallLocationFound)
        {
            detectionWatch.Stop();
            _spentOnDetectionInSecond += detectionWatch.Elapsed;
            if (isBallLocationFound)
                _successDetectionFrame++;
        }
    }
}
