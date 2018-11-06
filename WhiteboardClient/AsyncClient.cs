using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ColoredForms;
using SkiaSharp;

namespace WhiteboardClient
{
    public class AsyncClient
    {

        private const int port = 8080;
        public Socket Client { get; private set; }

        private readonly static ManualResetEvent connectDone = new ManualResetEvent(false);
        private readonly static ManualResetEvent sendDone = new ManualResetEvent(false);
        private readonly static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private readonly IPAddress IpAddress;
        private readonly IPEndPoint RemoteEndPoint;
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
                Client.EndConnect(ar);
                Debug.WriteLine("Socket connected to : {0}", Client.RemoteEndPoint);
                //request the current state of the whiteboard from the host
                connectDone.Set();
            } catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void ReceivePackage(Object o, PacketReceivedEventArgs eventArgs)
        {
            Dictionary<string, object> pdict = JsonConvert.DeserializeObject<Dictionary<string, object>>(eventArgs.data);
            Dictionary<string, string> content;;


            UpdateUIEventArgs UiEventArgs;

            switch (pdict["type"])
            {
                case "PATH":
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    ColoredPath path = DictToPath(content);
                    UiEventArgs = new UpdateUIEventArgs { Type = "PATH", Path = path};
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "CIRCLE":
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    ColoredCircle circle = DictToCircle(content);
                    UiEventArgs = new UpdateUIEventArgs { Type = "CIRCLE", Circle = circle };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "LINE":
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    ColoredLine line = DictToLine(content);
                    UiEventArgs = new UpdateUIEventArgs {Type="LINE", Line = line};
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "REQUEST_STATUS":
                    //Called for the host, when a new client is requesting the whiteboard's current state
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    int id = int.Parse(content["id"]);
                    //the id corresponds to the client's id from the server's perspective
                    UiEventArgs = new UpdateUIEventArgs { Type = "REQUEST_STATUS", client_id = id };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "RECTANGLE":
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    ColoredRectangle rectangle = DictToRectangle(content);
                    UiEventArgs = new UpdateUIEventArgs { Rectangle = rectangle, Type = "RECTANGLE" };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "SIZE":
                    content = JsonConvert.DeserializeObject<Dictionary<string, string>>(pdict["content"].ToString());
                    float w = float.Parse(content["width"]);
                    float h = float.Parse(content["height"]);
                    UiEventArgs = new UpdateUIEventArgs { width = w, height = h, Type = "SIZE" };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "CLEAR":
                    UiEventArgs = new UpdateUIEventArgs { Type = "CLEAR" };
                    UpdateUIEventHandler.OnUpdateUI(this, UiEventArgs);
                    break;
                case "RESTORE":
                    break;
                default:
                    Console.WriteLine("error parsing received data: {0}", eventArgs.data);
                    throw new ArgumentException(eventArgs.data);
            }
        }

        private ColoredPath DictToPath(Dictionary<string, string> content)
        {
            string SVGpath = content["svgpath"].ToString();
            SKPath path = SKPath.ParseSvgPathData(SVGpath);
            string ColourHash = content["colorHash"];
            SKColor Colour = SKColor.Parse(ColourHash);
            float strokeWidth = float.Parse(content["strokeWidth"]);
            return new ColoredPath { Color = Colour, Path = path, StrokeWidth = strokeWidth };
        }

        private ColoredLine DictToLine(Dictionary<string, string> content)
        {
            string ColourHash = content["colorHash"];
            SKColor Colour = SKColor.Parse(ColourHash);
            float strokeWidth = float.Parse(content["strokeWidth"]);
            string coordinates = content["coordinates"];
            float x1 = float.Parse(coordinates.Split(' ')[0]);
            float x2 = float.Parse(coordinates.Split(' ')[1]);
            float y1 = float.Parse(coordinates.Split(' ')[2]);
            float y2 = float.Parse(coordinates.Split(' ')[3]);
            SKPoint start = new SKPoint(x1, y1);
            SKPoint end = new SKPoint(x2, y2);
            return new ColoredLine { Start = start, End = end, Color = Colour, StrokeWidth = strokeWidth };
        }

        private ColoredCircle DictToCircle(Dictionary<string, string> content)
        {
            string ColourHash = content["colorHash"];
            SKColor Colour = SKColor.Parse(ColourHash);
            float radius = float.Parse(content["radius"]);
            string coordinates = content["coordinates"];
            float x = float.Parse(coordinates.Split(' ')[0]);
            float y = float.Parse(coordinates.Split(' ')[1]);
            SKPoint point = new SKPoint(x, y);
            float strokeWidth = float.Parse(content["strokeWidth"]);
            return new ColoredCircle { Center = point, Color = Colour, Radius = radius, StrokeWidth = strokeWidth };
        }

        private ColoredRectangle DictToRectangle(Dictionary<string, string> content)
        {
            string ColourHash = content["colorHash"];
            SKColor color = SKColor.Parse(ColourHash);
            float strokeWidth = float.Parse(content["strokeWidth"]);
            string coordinates = content["coordinates"];
            float x1 = float.Parse(coordinates.Split(' ')[0]);
            float x2 = float.Parse(coordinates.Split(' ')[1]);
            float y1 = float.Parse(coordinates.Split(' ')[2]);
            float y2 = float.Parse(coordinates.Split(' ')[3]);
            SKPoint start = new SKPoint(x1, y1);
            SKPoint end = new SKPoint(x2, y2);
            return new ColoredRectangle {Start = start, End = end, Color = color, StrokeWidth = strokeWidth };
        }

        private void SendData(JObject json)
        {
            string Json = json.ToString();
            PacketSender sender = new PacketSender(Client);
            sender.Send(Json);
        }

        
        public void Send(ColoredCircle circle)
        {
            JObject json = Jsonify(circle);
            SendData(json);
        }

        public void Send(ColoredPath path)
        {
            JObject json = Jsonify(path);
            SendData(json);
        }

        public void Send(ColoredLine line)
        {
            JObject json = Jsonify(line);
            SendData(json);
        }

        public void RestoreWhiteboard(List<object> form, int client_id)
        {
            //send the current status of the whiteboard to the server
            //TODO serialize the forms object
            JObject json = new JObject(new JProperty("type", "RESTORE"),
                new JProperty("client_id", client_id),
                new JProperty("content", JsonifyFormList(form)));
            SendData(json);
        }

        public void Send(ColoredRectangle rectangle)
        {
            JObject json = Jsonify(rectangle);
            SendData(json);
        }

        public void Send(string action)
        {
            if (action == "CLEAR")
            {
                JObject json = new JObject(new JProperty("type", "CLEAR"));
                SendData(json);
            }
        }

        private JArray JsonifyFormList(List<object> form)
        {
            JArray jArray = new JArray();
            JObject jObject;
            foreach (object o in form)
            {
                var colouredForm = o as ColoredForm;
                if (o == null)
                {
                    throw new ArgumentException();
                }
                if (colouredForm as ColoredLine != null)
                {
                    jObject = Jsonify(colouredForm as ColoredLine);
                    jArray.Add(jObject);
                }
                else if (colouredForm as ColoredPath != null)
                {
                    jObject = Jsonify(colouredForm as ColoredPath);
                    jArray.Add(jObject);
                }
                else if (colouredForm as ColoredCircle != null)
                {
                    jObject = Jsonify(colouredForm as ColoredCircle);
                    jArray.Add(jObject);
                }
                else if (colouredForm as ColoredRectangle != null)
                {
                    jObject = Jsonify(colouredForm as ColoredRectangle);
                    jArray.Add(jObject);
                }
            }
            return jArray;
        }


        private JObject Jsonify(ColoredRectangle rectangle)
        {
            string colourHash = rectangle.Color.ToString();
            float strokeWidth = rectangle.StrokeWidth;
            float x1 = rectangle.Start.X;
            float x2 = rectangle.End.X;
            float y1 = rectangle.Start.Y;
            float y2 = rectangle.End.Y;
            string coordinates = x1.ToString() + " " + x2.ToString() + " " + y1.ToString() + " " + y2.ToString();
            JObject json = new JObject(new JProperty("type", "RECTANGLE"),
                new JProperty("content", new JObject(
                    new JProperty("colorHash", colourHash),
                    new JProperty("coordinates", coordinates),
                    new JProperty("strokeWidth", strokeWidth))));
            return json;
        }

        private JObject Jsonify(ColoredCircle circle)
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
            return json;
        }

        private JObject Jsonify(ColoredPath path)
        {
            string colourHash = path.Color.ToString();
            string SVGPath = path.Path.ToSvgPathData();
            float strokeWidth = path.StrokeWidth;
            JObject json = new JObject(new JProperty("type", "PATH"),
                                       new JProperty("content", new JObject(
                                          new JProperty("svgpath", SVGPath),
                                          new JProperty("colorHash", colourHash),
                                          new JProperty("strokeWidth", strokeWidth))));
            return json;
        }

        private JObject Jsonify(ColoredLine line)
        {
            string colourHash = line.Color.ToString();
            float strokeWidth = line.StrokeWidth;
            float x1 = line.Start.X;
            float x2 = line.End.X;
            float y1 = line.Start.Y;
            float y2 = line.End.Y;
            string coordinates = x1.ToString() + " " + x2.ToString() + " " + y1.ToString() + " " + y2.ToString();
            JObject json = new JObject(new JProperty("type", "LINE"),
                new JProperty("content", new JObject(
                    new JProperty("colorHash", colourHash),
                    new JProperty("coordinates", coordinates),
                    new JProperty("strokeWidth", strokeWidth))));
            return json;
        }
    }
}
