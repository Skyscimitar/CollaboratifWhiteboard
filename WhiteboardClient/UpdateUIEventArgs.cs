using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace WhiteboardClient
{
    public class UpdateUIEventArgs: EventArgs
    {
        public SKPath Path { get; set; }
        public SKColor Colour { get; set; }
        public SKPoint Point { get; set; }
        public SKPoint Start { get; set; }
        public SKPoint End { get; set; }
        public float StrokeWidth { get; set; }
        public float Radius { get; set; }
        public string Type { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
