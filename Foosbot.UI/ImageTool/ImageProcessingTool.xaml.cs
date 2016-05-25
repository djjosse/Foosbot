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
using System.Windows.Controls;

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
                _labelA.Content = "Pre-Processed cropped frame based on Calibration Marks";
                _labelB.Content = "Ball Circle Tracker - Cropped based on Last Known Coordinate";
                _labelC.Content = "Frame Pre-Processed for Motion Detection";
                _labelD.Content = "Motion Detection Foreground Frame";

                LoadSliderValues();
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
                _imagePack.Streamer.Detach(_monitorA);
        }

        private void LoadSliderValues()
        {
            //Set limits for each
            _slA.Maximum = 0.9;
            _slA.Minimum = 0.1;
            _slA.TickFrequency = 0.1;
            _slB.Maximum = 0.9;
            _slB.Minimum = 0.1;
            _slB.TickFrequency = 0.1;
            _slC.Maximum = 0.9;
            _slC.Minimum = 0.1;
            _slC.TickFrequency = 0.1;
            _slD.Maximum = 0.9;
            _slD.Minimum = 0.1;
            _slD.TickFrequency = 0.1;
            _slE.Maximum = 0.9;
            _slE.Minimum = 0.1;
            _slE.TickFrequency = 0.1;
            _slF.Maximum = 0.9;
            _slF.Minimum = 0.1;
            _slF.TickFrequency = 0.1;

            //Set current value for each
            _tbA.Text = "0.5";
            _tbB.Text = "0.5";
            _tbC.Text = "0.5";
            _tbD.Text = "0.5";
            _tbE.Text = "0.5";
            _tbF.Text = "0.5";
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider current = sender as Slider;

            //change value to new one after change
            switch(current.Name)
            {
                case "_slA":
                   // MessageBox.Show("Changed");
                    break;
                case "_slB":
                    // MessageBox.Show("Changed");
                    break;
                case "_slC":
                    // MessageBox.Show("Changed");
                    break;
                case "_slD":
                    // MessageBox.Show("Changed");
                    break;
                case "_slE":
                    // MessageBox.Show("Changed");
                    break;
                case "_slF":
                    // MessageBox.Show("Changed");
                    break;
            }
        }
    }
}
