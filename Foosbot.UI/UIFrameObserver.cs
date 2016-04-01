// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Foosbot.Common.Multithreading;
using Foosbot.ImageProcessing;

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
