using System;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mainGUI
{
    /*Page popup apparaissant lorsqu'on appuie sur le bouton couleur*/
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
            page.color = button.BackgroundColor;
        }
    }
}