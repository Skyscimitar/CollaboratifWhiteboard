using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ColoredForms;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace WhiteboardClient
{
    public class AsyncClient
    {

        private const int port = 8080;
        public  Socket Client { get; private set; }

        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private IPAddress IpAddress;
        private IPEndPoint RemoteEndPoint;
        private PacketReceiver Receiver;


        public AsyncClient(string IpAddress)
        {
            if (IPAddress.TryParse(IpAddress, out IPAddress address))
            {
                this.IpAddress = address;
                RemoteEndPoint = new IPEndPoint(this.IpAddress, port);
                Client = new Socket(this.IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                throw new ArgumentException(message: "invalid ip address provided");
            }
        }

        public void StartClient()
        {
            Ping pinger = new Ping();
            PingReply reply = pinger.Send(this.IpAddress);
            if (reply.Status != IPStatus.Success)
            {
                throw new DiscoverHostException(message: "Couldn't ping host");
            }
            Client.BeginConnect(RemoteEndPoint, new AsyncCallback(ConnectCallback), Client);
            connectDone.WaitOne();
            Receiver = new PacketReceiver(Client);
            PacketReceivedEventHandler.OnReceivePackage += ReceivePackage;
            Thread receiveThread = new Thread(Receiver.StartReceiving);
            receiveThread.Start();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Client = (Socket)ar.AsyncState;
                Debug.WriteLine(Client.ToString());
                Debug.WriteLine(Client.RemoteEndPoint.Serialize());
                Client.EndConnect(ar);
                Debug.WriteLine("Socket connected to : {0}", Client.RemoteEndPoint.ToString());

                connectDone.Set();
            } catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void ReceivePackage(Object o, PacketReceivedEventArgs eventArgs)
        {
            Debug.WriteLine("Received Package");
            Dictionary<string, object> pdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventArgs.data);
            Dictionary<string, string> content;
            SKColor Colour;
            string ColourHash;
            SKPath path;
            string SVGpath;
            SKPoint point;
            string coordinates;
            float x;
            float y;
            float strokeWidth;
            float radius;
            UpdateUIEventArgs UiEventArgs;

            switch (pdict["type"])
            {
                case "PATH":
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    SVGpath = content["svgpath"].ToString();
                    path = SKPath.ParseSvgPathData(SVGpath);
                    ColourHash = content["colorHash"];
                    Colour = SKColor.Parse(ColourHash);
                    strokeWidth = float.Parse(content["strokeWidth"]);
                    UiEventArgs = new UpdateUIEventArgs { colour = Colour, path = path, type = "PATH", strokeWidth = strokeWidth };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "CIRCLE":
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    ColourHash = content["colorHash"];
                    Colour = SKColor.Parse(ColourHash);
                    radius = float.Parse(content["radius"]);
                    coordinates = content["coordinates"];
                    x = float.Parse(coordinates.Split(' ')[0]);
                    y = float.Parse(coordinates.Split(' ')[0]);
                    point = new SKPoint(x, y);
                    strokeWidth = float.Parse(content["strokeWidth"]);
                    UiEventArgs = new UpdateUIEventArgs { colour = Colour, radius = radius, point = point, strokeWidth = strokeWidth };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                default:
                    Console.WriteLine("error parsing received data: {0}", eventArgs.data);
                    break;
            }
        }

        private void SendData(JObject json)
        {
            string Json = json.ToString();
            PacketSender sender = new PacketSender(Client);
            sender.Send(Json);
        }

        
        public void Send(ColoredCircle circle)
        {
            string colourHash = circle.Color.ToString();
            float x = circle.Center.X;
            float y = circle.Center.Y;
            string coordinates = x.ToString() + " " + y.ToString();
            float strokeWidth = circle.StrokeWidth;
            float radius = circle.Radius;
            JObject json = new JObject(new JProperty("type", "CIRCLE"),
                                      new JProperty("content", new JObject(
                                          new JProperty("colorHash", colourHash),
                                          new JProperty("coordinates", coordinates),
                                           new JProperty("radius", radius),
                                           new JProperty("strokeWidth", strokeWidth))));
            SendData(json);
        }

        public void Send(ColoredPath path)
        {
            string colourHash = path.Color.ToString();
            string SVGPath = path.Path.ToSvgPathData();
            float strokeWidth = path.StrokeWidth;
            JObject json = new JObject(new JProperty("type", "PATH"),
                                       new JProperty("content", new JObject(
                                          new JProperty("svgpath", SVGPath),
                                          new JProperty("colorHash", colourHash),
                                          new JProperty("strokeWidth", strokeWidth))));
            SendData(json);
        }
    }
}
