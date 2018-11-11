using SkiaSharp;

namespace ColoredForms
{
    public sealed class ColoredLine : ColoredForm
    {
        public SKPoint Start { get; set; }
        public SKPoint End { get; set; }
    }
}
