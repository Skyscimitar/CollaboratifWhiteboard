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
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;

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
            HostServer server = new HostServer(Width,Height);
            Thread sThread = new Thread(server.StartListening);
            sThread.Start();
            await Navigation.PushAsync(new MainPage("127.0.0.1", true));
        }

        private async void ClientButton_Clicked(object sender, EventArgs e)
        {
            string ip = IPEntry.Text;
            if (ip != "" && ip != null)
            {
                string[] parsedIp = ip.Split('.');
                if (Int32.TryParse(parsedIp[0], out int n1) && Int32.TryParse(parsedIp[1], out int n2) && Int32.TryParse(parsedIp[2], out int n3) && Int32.TryParse(parsedIp[3], out int n4))
                {
                    if (parsedIp.Length == 4 && IsValidOctet(n1) && IsValidOctet(n2) && IsValidOctet(n3) && IsValidOctet(n4))
                    {
                        await Navigation.PushAsync(new MainPage(ip, false));
                    }
                }
            }
            else
                await Navigation.PushAsync(new MainPage("127.0.0.1", false));
        }

        private bool IsValidOctet(int n)
        {
            return (n <= 255 && n >= 0);
        }
    }
}