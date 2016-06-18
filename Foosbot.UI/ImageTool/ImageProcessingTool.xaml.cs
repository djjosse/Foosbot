// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Contracts;
using Foosbot.UI.ImageExtensions;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Foosbot.UI.ImageTool
{
    /// <summary>
    /// Interaction logic for ImageProcessingTool.xaml
    /// </summary>
    public partial class ImageProcessingTool : Window, IInitializable
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
                Initialize();
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
            this.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Is Initialized property
        /// </summary>
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (!IsInitialized)
            {
                //Set limits for each
                _slA.Maximum = 255;
                _slA.Minimum = 0;
                _slA.TickFrequency = 10;
                _slB.Maximum = 500;
                _slB.Minimum = 0;
                _slB.TickFrequency = 10;
                _slC.Maximum = 100;
                _slC.Minimum = 0;
                _slC.TickFrequency = 5;
                _slD.Maximum = 3.0;
                _slD.Minimum = 0.5;
                _slD.TickFrequency = 0.1;
                _slE.Maximum = 255;
                _slE.Minimum = 0;
                _slE.TickFrequency = 10;
                _slF.Maximum = 100;
                _slF.Minimum = 0;
                _slF.TickFrequency = 1;
                _slG.Maximum = 1.5;
                _slG.Minimum = 0.0;
                _slG.TickFrequency = 0.05;


                //Set current value for each
                _tbA.Text = _imagePack.CircleDetectionGrayThreshold.ToString();
                _tbB.Text = _imagePack.CircleDetectionCannyThreshold.ToString();
                _tbC.Text = _imagePack.CircleDetectionAccumulatorThreshold.ToString();
                _tbD.Text = _imagePack.CircleDetectionInverseRatio.ToString();
                _tbE.Text = _imagePack.MotionDetectionGrayThreshold.ToString();
                _tbF.Text = _imagePack.MinimalMotionAreaThreshold.ToString();
                _tbG.Text = _imagePack.MinimalMotionPixelsFactor.ToString();

                IsInitialized = true;
            }
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsInitialized)
            {
                Slider current = sender as Slider;

                //change value to new one after change
                switch (current.Name)
                {
                    case "_slA":
                        _imagePack.CircleDetectionGrayThreshold = Convert.ToInt32(current.Value);
                        break;
                    case "_slB":
                        _imagePack.CircleDetectionCannyThreshold = current.Value;
                        break;
                    case "_slC":
                        _imagePack.CircleDetectionAccumulatorThreshold = current.Value;
                        break;
                    case "_slD":
                        _imagePack.CircleDetectionInverseRatio = current.Value;
                        break;
                    case "_slE":
                        _imagePack.MotionDetectionGrayThreshold = Convert.ToInt32(current.Value); ;
                        break;
                    case "_slF":
                        _imagePack.MinimalMotionAreaThreshold = current.Value;
                        break;
                    case "_slG":
                        _imagePack.MinimalMotionPixelsFactor = current.Value;
                        break;
                }
            }
        }

    }
}
