using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace mainGUI
{
    public class ColorChangedEventArgs : EventArgs
    {
        public SKColor color { get; set; }
    }
}
