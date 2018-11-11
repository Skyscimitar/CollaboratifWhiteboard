using SkiaSharp;

namespace ColoredForms
{
    //Classe abstraite dont héritent toutes les formes
    public abstract class ColoredForm
    {
        public SKColor Color { get; set; }
        public float StrokeWidth { get; set; }
    }
}
