using Foosbot.Common.Logs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Foosbot.UI.Logger
{
    /// <summary>
    /// User Interface Log manager class
    /// </summary>
    public class UILogManager
    {
        /// <summary>
        /// Log Manager Constructor
        /// </summary>
        public UILogManager()
        {
            CurrentFilter = eLogType.NotDefined;
            HasUpdates = false;
            AllEntries = new ObservableCollection<UILogMessage>();
        }

        /// <summary>
        /// Property defines if there are new messages to show
        /// </summary>
        public bool HasUpdates { get; private set; }

        /// <summary>
        /// Current filter to show messages for
        /// </summary>
        public eLogType CurrentFilter {get; set;}

        /// <summary>
        /// All entries collection
        /// </summary>
        private ObservableCollection<UILogMessage> AllEntries { get; set; }

        /// <summary>
        /// This function is busy-waiting for next log message
        /// </summary>
        /// <returns>Last message</returns>
        public UILogMessage WaitForMessage()
        {
            Color color = Colors.Black;
            eLogType type = eLogType.NotDefined;
            String text = "";
            while (String.IsNullOrEmpty(text))
            {
                if (Log.Common.HasMessages)
                {
                    LogMessage lastMessage = Log.Common.LastMessage;
                    color = DefineColor(lastMessage.Category);
                    type = eLogType.Common;
                    text = DefineMessage(lastMessage, type);
                    if (CurrentFilter.Equals(eLogType.NotDefined) || CurrentFilter.Equals(eLogType.Common))
                        HasUpdates = true;
                }
                else if (Log.Image.HasMessages)
                {
                    LogMessage lastMessage = Log.Image.LastMessage;
                    color = DefineColor(lastMessage.Category);
                    type = eLogType.Image;
                    text = DefineMessage(lastMessage, type);
                    if (CurrentFilter.Equals(eLogType.NotDefined) || CurrentFilter.Equals(eLogType.Image))
                        HasUpdates = true;
                }
            }
            if (!String.IsNullOrEmpty(text))
            {
                UILogMessage message = new UILogMessage() { FontColor = color, Message = text, Type = type };
                AllEntries.Add(message);
                return message;
            }
            return null;
        }

        /// <summary>
        /// Message to printable format
        /// </summary>
        /// <param name="message">Message to format</param>
        /// <param name="type">Type of log</param>
        /// <returns>Message as string</returns>
        private string DefineMessage(LogMessage message, eLogType type)
        {
            return String.Format("{0}\t{1}\t{2}\t{3}", message.TimeStamp.ToString("HH:mm:ss:fff"), message.Category, type, message.Description);
        }

        /// <summary>
        /// Defines color for specific message category
        /// </summary>
        /// <param name="category">Message Category as string</param>
        /// <returns></returns>
        private Color DefineColor(string category)
        {
            eLogCategory categoryEnum;
            Enum.TryParse<eLogCategory>(category, true, out categoryEnum);
            switch (categoryEnum)
            {
                case eLogCategory.Error:
                    return Colors.Red;
                case eLogCategory.Info:
                    return Colors.DarkGreen;
                case eLogCategory.Warn:
                    return Colors.DarkOrange;
                default:
                    return Colors.Black;
            }
        }
    }
}
