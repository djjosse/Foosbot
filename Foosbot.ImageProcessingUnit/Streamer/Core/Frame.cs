// **************************************************************************************
// **																				   **
// **		(C) FOOSBOT - Final Software Engineering Project, 2015 - 2016			   **
// **		(C) Authors: M.Toubian, M.Shimon, E.Kleinman, O.Sasson, J.Gleyzer          **
// **			Advisors: Mr.Resh Amit & Dr.Hoffner Yigal							   **
// **		The information and source code here belongs to Foosbot project			   **
// **		and may not be reproduced or used without authors explicit permission.	   **
// **																				   **
// **************************************************************************************

using Emgu.CV;
using Emgu.CV.Structure;
using Foosbot.ImageProcessingUnit.Streamer.Contracts;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Foosbot.ImageProcessingUnit.Streamer.Core
{
    /// <summary>
    /// Frame with time stamp
    /// </summary>
    public class Frame : IFrame
    {
        /// <summary>
        /// Frame Image
        /// </summary>
        public Image<Gray, byte> Image { get; set; }

        /// <summary>
        /// Frame Time Stamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Create exact copy of current frame
        /// </summary>
        /// <returns>Cloned Frame</returns>
        public IFrame Clone()
        {
            IFrame frameCopy = new Frame();
            frameCopy.Timestamp = this.Timestamp;
            frameCopy.Image = this.Image.Clone();
            return frameCopy;
        }

        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The pointer to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The EMGU CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public BitmapSource ToBitmapSource()
        {
            using (System.Drawing.Bitmap source = Image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the HBitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        /// <summary>
        /// Dispose current frame
        /// </summary>
        public void Dispose()
        {
            if (Image != null)
                Image.Dispose();
        }
    }
}
