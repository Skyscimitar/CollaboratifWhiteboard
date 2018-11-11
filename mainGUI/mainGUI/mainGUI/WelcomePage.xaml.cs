using System;
using System.Threading;
using Hostserver;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mainGUI
{
    /*Comportement de la page d'accueille apparaissant au lancement de l'application*/
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WelcomePage : ContentPage
	{
		public WelcomePage ()
		{
			InitializeComponent ();
		}

        /*Quand on clique sur server, un serveur est créé et le constructeur correspondant 
         est appelé dans MainPage*/
        private async void ServerButton_Clicked(object sender, EventArgs e)
        {
            HostServer server = new HostServer(Width,Height);
            Thread sThread = new Thread(server.StartListening);
            sThread.Start();
            await Navigation.PushAsync(new MainPage(server, "127.0.0.1"));
        }

        //Recupère l'IP indiquée et se connecte au serveur correspondant s'il existe
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
                        await Navigation.PushAsync(new MainPage(ip));
                    }
                }
            }
            else
                await Navigation.PushAsync(new MainPage("127.0.0.1"));
        }
        
        private bool IsValidOctet(int n)
        {
            return (n <= 255 && n >= 0);
        }
    }
}