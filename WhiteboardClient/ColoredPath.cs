using System;
using SkiaSharp;
namespace WhiteboardClient
{
    public class ColoredPath
    {
        public SKColor Color { get; set; }
        public SKPath Path { get; set; }
        public float StrokeWidth { get; set; }
    }
}
