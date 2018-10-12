using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace WhiteboardClient
{
    public class UpdateUIEventArgs: EventArgs
    {
        public SKPath path { get; set; }
        public SKColor color { get; set; }
        public SKPoint point { get; set; }
        public float lineWidth { get; set; }
        public float radius { get; set; }
        public string type { get; set; }
    }
}
