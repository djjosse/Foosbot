using Foosbot.Common.Multithreading;
using Foosbot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Foosbot.UI
{
    /// <summary>
    /// Observer of streamer in MainWindow
    /// </summary>
    public class UIFrameObserver:Observer<Frame>
    {
        public UIFrameObserver(Publisher<Frame> streamer) : base(streamer) 
        {
            _isSet = false;
        }

        public volatile bool _isSet;
        public bool Set
        {
            get
            {
                bool set = _isSet;
                if (set)
                    _isSet = false;
                return set;
            }
        }

        public override void Job()
        {
            _isSet = true;
        }
    }
}
