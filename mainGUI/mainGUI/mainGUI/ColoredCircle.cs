using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace mainGUI
{
    public class ColoredCircle
    {
        public SKColor Color { get; set; }
        public SKPoint Center { get; set; }
        public float Radius { get; set; }
        public float StrokeWidth { get; set; }
    }
}
