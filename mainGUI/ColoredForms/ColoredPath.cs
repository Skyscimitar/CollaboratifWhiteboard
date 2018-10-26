using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace ColoredForms
{
    /*Classe permettant de stocker la couleur avec un objet de type SKPath. 
    Potentiellement à généraliser à toutes les formes ensuite.*/
    public class ColoredPath : ColoredForm
    {
        public SKPath Path { get; set; }
        public float StrokeWidth { get; set; }
    }
}
