using SkiaSharp;


namespace ColoredForms
{
    /*Classe permettant de stocker la couleur avec un objet de type SKPath. 
    Potentiellement à généraliser à toutes les formes ensuite.*/
    public class ColoredPath : ColoredForm
    {
        public SKPath Path { get; set; }
    }
}
