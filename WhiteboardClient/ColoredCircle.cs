using System;
using SkiaSharp;

namespace WhiteboardClient
{
    public class ColoredCircle
    {
        public SKColor Color { get; set; }
        public SKPoint Center { get; set; }
        public float Radius { get; set; }
        public float StrokeWidth { get; set; }
    }
}
