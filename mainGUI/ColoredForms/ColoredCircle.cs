using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace ColoredForms
{
    public class ColoredCircle : ColoredForm
    {
        public SKPoint Center { get; set; }
        public float Radius { get; set; }
    }
}
