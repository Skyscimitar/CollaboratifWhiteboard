using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;


namespace WhiteboardClient
{
    public class Client
    {
        public Socket socket {get; set; }
        public PacketReceiver receiver;

        public Client(Socket socket)
        {
            this.socket = socket;
            receiver = new PacketReceiver(socket);
            PacketReceivedEventHandler.OnReceivePackage += ReceivePackage;
            receiver.StartReceiving();
        }

        public void Disconnect()
        {
            //ends the connection (basically kills the client)
            receiver.Disconnect();
        }

        void ReceivePackage(Object o, PacketReceivedEventArgs eventArgs)
        {
            // {"type": "PATH", "Content": {...}
            JObject jObject = JObject.Parse(eventArgs.data);
            Dictionary<string, object> pdict = jObject.ToObject<Dictionary<string, object>>();
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
                    SVGpath = (pdict["content"] as Dictionary<string, string>)["svgpath"];
                    path = SKPath.ParseSvgPathData(SVGpath);
                    ColourHash = (pdict["content"] as Dictionary<string, string>)["colorHash"];
                    Colour = SKColor.Parse(ColourHash);
                    strokeWidth = float.Parse((pdict["content"] as Dictionary<string, string>)["strokeWidth"]);
                    UiEventArgs = new UpdateUIEventArgs { colour = Colour, path = path, type = "PATH", strokeWidth = strokeWidth };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "CIRCLE":
                    ColourHash = (pdict["content"] as Dictionary<string, string>)["colorHash"];
                    Colour= SKColor.Parse(ColourHash);
                    radius = float.Parse((pdict["content"] as Dictionary<string,string>)["radius"]);
                    coordinates = (pdict["content"] as Dictionary<string, string>)["coorditates"];
                    x = float.Parse(coordinates.Split(' ')[0]);
                    y = float.Parse(coordinates.Split(' ')[0]);
                    point = new SKPoint(x, y);
                    strokeWidth = float.Parse((pdict["content"] as Dictionary<string, string>)["strokeWidth"]);
                    UiEventArgs = new UpdateUIEventArgs { colour = Colour, radius = radius, point = point, strokeWidth = strokeWidth };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                default:
                    Console.WriteLine("error parsing received data: {0}", eventArgs.data);
                    break;
            }
        }
    }
}