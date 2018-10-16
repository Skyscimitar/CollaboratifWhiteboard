using System;
using Xamarin.Forms;

namespace MainGUI
{
    public class ColorChangedEventArgs : EventArgs
    {
        public ColorChangedEventArgs(Color color)
        {
            this.Color = color;
        }
        public Color Color { get; private set; }
    }
}