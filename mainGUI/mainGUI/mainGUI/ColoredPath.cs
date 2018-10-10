using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace mainGUI
{
    /*Classe permettant de stocker la couleur avec un objet de type SKPath. 
    Potentiellement à généraliser à toutes les formes ensuite.*/
    public class ColoredPath
    {
        public SKColor Color { get; set; }
        public SKPath Path { get; set; }
        public float StrokeWidth { get; set; }
    }
}
