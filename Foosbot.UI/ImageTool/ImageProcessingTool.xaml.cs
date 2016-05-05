// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.UI.ImageExtensions;
using System.ComponentModel;
using System.Windows;

namespace Foosbot.UI.ImageTool
{
    /// <summary>
    /// Interaction logic for ImageProcessingTool.xaml
    /// </summary>
    public partial class ImageProcessingTool : Window
    {
        /// <summary>
        /// Image Processing Pack Instance
        /// </summary>
        ImageProcessPack _imagePack;

        #region Frame Monitor to present frame streams for user

        FrameUiMonitor _monitorA;
        FrameUiMonitor _monitorB;
        FrameUiMonitor _monitorC;
        FrameUiMonitor _monitorD;

        #endregion Frame Monitor to present frame streams for user

        /// <summary>
        /// Image Processing Tool Window Constructor
        /// </summary>
        /// <param name="imagePack">Image processing pack to work with</param>
        public ImageProcessingTool(ImageProcessPack imagePack)
        {
            InitializeComponent();
            _imagePack = imagePack;
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        /// <summary>
        /// Operations to perform once the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!ImageProcessPack.IsDemoMode)
            {
                _monitorA = new FrameUiMonitor(_imagePack.ImageProcessUnit.ImageProcessingMonitorA,
                    Dispatcher, _guiImageA);
                _monitorB = new FrameUiMonitor(_imagePack.ImageProcessUnit.ImageProcessingMonitorB,
                    Dispatcher, _guiImageB);
                _monitorC = new FrameUiMonitor(_imagePack.ImageProcessUnit.ImageProcessingMonitorC,
                    Dispatcher, _guiImageC);
                _monitorD = new FrameUiMonitor(_imagePack.ImageProcessUnit.ImageProcessingMonitorD,
                    Dispatcher, _guiImageD);
                _monitorA.Start();
                _monitorB.Start();
                _monitorC.Start();
                _monitorD.Start();
                _labelA.Content = "Pre-Processed frame";
                _labelB.Content = "Cropped frame based on Calibration Marks";
                _labelC.Content = "Cropped frame based on Last Known Coordinate";
                _labelD.Content = "Motion Detection frame";
            }
            else
            {
                MessageBox.Show("Image Processing Tools are not available in Demo Mode!");
                Close();
            }
        }

        /// <summary>
        /// Operations to perform on Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_monitorA != null)
                _imagePack.Streamer.Dettach(_monitorA);
        }
    }
}
