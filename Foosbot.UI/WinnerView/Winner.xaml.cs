using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace Foosbot.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Winner : Window
    {
        public Winner(string winner,string player1Score, string player2Score)
        {
            InitializeComponent();
            Label winnerLabel = (Label)this.FindName("WinnerLabel");
            Label scoreLabel = (Label)this.FindName("ScoreLabel");
            winnerLabel.Content = string.Format("{0} WINS!!!", winner);
            scoreLabel.Content = string.Format("{0} - {1}", player1Score,player2Score);
            SoundPlayer audio = new SoundPlayer(Properties.Resources.Sound_Win);
            audio.Play();
        }
    }
}
