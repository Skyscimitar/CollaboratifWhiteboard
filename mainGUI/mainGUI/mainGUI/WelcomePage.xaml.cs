using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhiteboardClient;
using Hostserver;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mainGUI
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WelcomePage : ContentPage
	{
		public WelcomePage ()
		{
			InitializeComponent ();

		}

        private async void ServerButton_Clicked(object sender, EventArgs e)
        {
            HostServer server = new HostServer();
            Thread sThread = new Thread(server.StartListening);
            sThread.Start();
            await Navigation.PushAsync(new MainPage("host"));
        }

        private async void ClientButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage("client"));
        }
    }
}