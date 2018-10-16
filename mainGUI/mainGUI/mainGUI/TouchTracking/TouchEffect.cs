﻿using mainGUI.TouchTracking;
using Xamarin.Forms;

namespace MainGUI.TouchTracking
{

    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public TouchEffect() : base("ColorPickerSample.TouchEffect") { }

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}