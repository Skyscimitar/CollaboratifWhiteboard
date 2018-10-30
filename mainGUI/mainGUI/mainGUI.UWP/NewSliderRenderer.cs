using mainGUI.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Slider), typeof(NewSliderRenderer))]
namespace mainGUI.UWP
{
    public class NewSliderRenderer : SliderRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Slider> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var slider = new FormsSlider
                {
                    IsThumbToolTipEnabled = false
                };
                SetNativeControl(slider);
            }
            if(e.OldElement != null)
            {
                var nativeCtrl = this;
                
            }
            if (e.NewElement != null)
            {
                var slider = Control;
                slider.IsThumbToolTipEnabled = false;                
            }
        }

    }
}