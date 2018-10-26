using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mainGUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPage : PopupPage
    {
        private readonly MainPage page;
        public ColorPage(MainPage page)
        {
            this.page = page;
            InitializeComponent();
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }

        private void ColorButton_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Color color = button.BackgroundColor;
            page.color = color;
            button.BackgroundColor = color;
        }
    }
}