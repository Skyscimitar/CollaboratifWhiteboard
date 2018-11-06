using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace WhiteboardClient
{
    public class UpdateUIEventArgs: EventArgs
    {
        public SKPath path { get; set; }
        public SKColor colour { get; set; }
        public SKPoint point { get; set; }
        public SKPoint start { get; set; }
        public SKPoint end { get; set; }
        public float strokeWidth { get; set; }
        public float radius { get; set; }
        public string type { get; set; }
        public int client_id { get; set; }
        public float width { get; set; }
        public float height { get; set; }
    }
}
