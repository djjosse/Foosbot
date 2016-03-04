using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using Foosbot.Common.Logs;

namespace Foosbot.UI.Logger
{
    /// <summary>
    /// User Interface log message to show
    /// This class is binded with GUI
    /// </summary>
    public class UILogMessage : INotifyPropertyChanged
    {
        private eLogType _type;
        public eLogType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private const string FONT_COLOR = "FontColor";
        private Color _fontColor;
        public Color FontColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                OnPropertyChanged(FONT_COLOR);
            }
        }

        private const string MESSAGE = "Message";

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(MESSAGE);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

        }
    }
}
