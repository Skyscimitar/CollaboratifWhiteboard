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
        private PacketSender sender;

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
            Dictionary<string, object> pdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventArgs.data);
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
                    coordinates = (pdict["content"] as Dictionary<string, string>)["coordinates"];
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

        public void Send(ColoredPath path)
        {
            string colourHash = path.Color.ToString();
            string SVGPath = path.Path.ToSvgPathData();
            float strokeWidth = path.StrokeWidth;
            JObject json = new JObject(new JProperty("type", "PATH"), 
                                       new JProperty("content",new JObject(
                                          new JProperty("svgpath", SVGPath),
                                          new JProperty("colorHash", colourHash),
                                          new JProperty("strokeWidth", strokeWidth))));
            SendPacket(json);
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
            SendPacket(json);
        }

        private void SendPacket(JObject json)
        {
            string Json = json.ToString();
            sender = new PacketSender(socket);
            sender.Send(Json);
        }
    }
}