using SkiaSharp;

namespace ColoredForms
{
    public abstract class ColoredForm
    {
        public SKColor Color { get; set; }
        public float StrokeWidth { get; set; }
    }
}
