using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using ColoredForms;
using System.Collections.Generic;

namespace WhiteboardClient
{
    public class UpdateUIEventArgs: EventArgs
    {
        public string Type { get; set; }
        public int client_id { get; set; }
        public ColoredRectangle Rectangle { get; set; }
        public ColoredCircle Circle { get; set; }
        public ColoredLine Line { get; set; }
        public ColoredPath Path { get; set; }
        public List<object> Forms { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

    }
}
