using Foosbot.UI.ImageExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Foosbot.UI.ImageTool
{
    /// <summary>
    /// Interaction logic for ImageProcessingTool.xaml
    /// </summary>
    public partial class ImageProcessingTool : Window
    {
        ImageProcessPack _imagePack;

        FrameUiMonitor _monitorA;
        FrameUiMonitor _monitorB;
        FrameUiMonitor _monitorC;
        FrameUiMonitor _monitorD;

        public ImageProcessingTool(ImageProcessPack imagePack)
        {
            InitializeComponent();
            _imagePack = imagePack;
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

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

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_monitorA != null)
                _imagePack.Streamer.Dettach(_monitorA);
        }
    }
}
