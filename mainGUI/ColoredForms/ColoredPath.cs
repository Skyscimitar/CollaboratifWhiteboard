using SkiaSharp;


namespace ColoredForms
{
    /*Classe permettant de stocker la couleur avec un objet de type SKPath. */
    public sealed class ColoredPath : ColoredForm
    {
        public SKPath Path { get; set; }
    }
}
