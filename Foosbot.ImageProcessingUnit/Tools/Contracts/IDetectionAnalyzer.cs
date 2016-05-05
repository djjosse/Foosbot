// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

namespace Foosbot.ImageProcessingUnit.Tools.Contracts
{
    /// <summary>
    /// Detection Analyzer Tool interface
    /// </summary>
    public interface IDetectionAnalyzer
    {
        /// <summary>
        /// Detection Rate in percent
        /// </summary>
        double DetectionRate { get; }

        /// <summary>
        /// Average Time taken for detection
        /// </summary>
        double AverageDetectionTime { get; }

        /// <summary>
        /// Frames per second then detection was successful
        /// </summary>
        int DetectedFPS { get; }

        /// <summary>
        /// Total frames received per second
        /// </summary>
        int TotalFPS { get; }

        /// <summary>
        /// Steps to perform each detection started
        /// 1. Count frame
        /// 2. Start detection stopwatch
        /// If not same second as in previous frame then update statistics and start from the beginning
        /// </summary>
        void Begin();

        /// <summary>
        /// Steps to perform after each detection finished
        /// 1. Count detection if successful
        /// 2. Stop the detection stopwatch
        /// </summary>
        /// <param name="isBallLocationFound">Detection result</param>
        void Finalize(bool isBallLocationFound);
    }
}
