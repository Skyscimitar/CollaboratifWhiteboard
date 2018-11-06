﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Rg.Plugins.Popup.Services;
using WhiteboardClient;
using ColoredForms;
using System.Net;
using Hostserver;

namespace mainGUI
{
    //Ici on définit les actions à effectuer sur cette page.
    public partial class MainPage : ContentPage
    {
        private readonly Dictionary<long,object> temporaryForms =new Dictionary<long,object>();
        private readonly List<object> forms = new List<object>();
        private string option; //variable stockant l'option choisie par l'utilisateur (trait, gomme, cercle, etc.)
        private SKColor _color  = SKColors.Black;
        private float width;
        private float height;
        public string IpAddress { get; }
        private readonly HostServer hostServer;
        public Color color
        {
            get
            {
                return _color.ToFormsColor();
            }
            set
            {
                ColorButton.BackgroundColor = value;
                _color = value.ToSKColor();
            }
        }
        private float strokeWidth = 5;
        private readonly AsyncClient asyncClient;

        public MainPage(HostServer server, string ip)

        {
            hostServer = server;
            BindingContext = this;
            IpAddress = new WebClient().DownloadString("http://icanhazip.com");
            IpAddress = String.Format("Whiteboard IP: {0}", IpAddress);
            InitializeComponent();
            UpdateUIEventHandler.OnUpdateUI += UpdateUi;
            asyncClient = new AsyncClient(ip);
            asyncClient.StartClient();
        }

        public MainPage(string ip)
        {
            BindingContext = this;
            IpAddress = String.Format("Whiteboard IP: {0}", ip);
            InitializeComponent();
            UpdateUIEventHandler.OnUpdateUI += UpdateUi;
            asyncClient = new AsyncClient(ip);
            asyncClient.StartClient();
        }

        private void UpdateUi(Object o, UpdateUIEventArgs eventArgs)
        {
            lock (forms)
            {
<<<<<<< HEAD
                case "PATH":
                    ColoredPath coloredPath = new ColoredPath
                    {
                        Path = eventArgs.path,
                        Color = eventArgs.colour,
                        StrokeWidth = eventArgs.strokeWidth
                    };
                    forms.Add(coloredPath);
                    break;
                case "CIRCLE":
                    ColoredCircle coloredCircle = new ColoredCircle
                    {
                        Radius = eventArgs.radius,
                        StrokeWidth = eventArgs.strokeWidth,
                        Center = eventArgs.point,
                        Color = eventArgs.colour
                    };
                    forms.Add(coloredCircle);
                    break;
                case "LINE":
                    Debug.WriteLine("here");
                    ColoredLine coloredLine = new ColoredLine
                    {
                        Color = eventArgs.colour,
                        Start = eventArgs.start,
                        End = eventArgs.end,
                        StrokeWidth = eventArgs.strokeWidth
                    };
                    forms.Add(coloredLine);
                    break;
                case "REQUEST_STATUS":
                    //triggered when a new client is requesting the current status of the whiteboard
                    //only called on the host's side
                    break;
                case "RESTORE":
                    //triggered when the client connects to a whiteboard and needs to sync his display with 
                    //the current status of the whiteboard
                    break;
            }

=======
                switch (eventArgs.type)
                {
                    case "PATH":
                        ColoredPath coloredPath = new ColoredPath
                        {
                            Path = eventArgs.path,
                            Color = eventArgs.colour,
                            StrokeWidth = eventArgs.strokeWidth
                        };
                        forms.Add(coloredPath);
                        break;
                    case "CIRCLE":
                        ColoredCircle coloredCircle = new ColoredCircle
                        {
                            Radius = eventArgs.radius,
                            StrokeWidth = eventArgs.strokeWidth,
                            Center = eventArgs.point,
                            Color = eventArgs.colour
                        };
                        forms.Add(coloredCircle);
                        break;
                    case "LINE":
                        ColoredLine coloredLine = new ColoredLine
                        {
                            Color = eventArgs.colour,
                            Start = eventArgs.start,
                            End = eventArgs.end,
                            StrokeWidth = eventArgs.strokeWidth
                        };
                        forms.Add(coloredLine);
                        break;
                    case "RECTANGLE":
                        ColoredRectangle coloredRectangle = new ColoredRectangle
                        {
                            Color = eventArgs.colour,
                            Start = eventArgs.start,
                            End = eventArgs.end,
                            StrokeWidth = eventArgs.strokeWidth
                        };
                        forms.Add(coloredRectangle);
                        break;
                    case "CLEAR":
                        forms.Clear();
                        break;
                    case "SIZE":
                        width = eventArgs.width;
                        height = eventArgs.height;
                        break;
                }
>>>>>>> master
            View.InvalidateSurface();
            }
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e) //méthode définissant ce qui s'affiche à l'écran en temps réel
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            float w = (float)Width;
            float h = (float)Height;
            if (width == 0)
                width = w;
            if (height == 0)
                height = h;
            float sx = w / width;
            float sy = h / height;
            canvas.Clear(SKColors.White);
            canvas.Scale(sx, sy);

            var touchStroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            lock (forms)
            {
                foreach (var touchForm in forms)
                {
                    if (touchForm is ColoredPath touchPath)
                    {
                        touchStroke.Color = touchPath.Color;
                        touchStroke.StrokeWidth = touchPath.StrokeWidth;
                        canvas.DrawPath(touchPath.Path, touchStroke);
                    }

                    if (touchForm is ColoredCircle touchCircle)
                    {
                        touchStroke.Color = touchCircle.Color;
                        touchStroke.StrokeWidth = touchCircle.StrokeWidth;
                        canvas.DrawCircle(touchCircle.Center, touchCircle.Radius, touchStroke);
                    }

                    if (touchForm is ColoredLine touchLine)
                    {
                        touchStroke.Color = touchLine.Color;
                        touchStroke.StrokeWidth = touchLine.StrokeWidth;
                        canvas.DrawLine(touchLine.Start, touchLine.End, touchStroke);
                    }

                    if (touchForm is ColoredRectangle touchRectangle)
                    {
                        touchStroke.Color = touchRectangle.Color;
                        touchStroke.StrokeWidth = touchRectangle.StrokeWidth;
                        SKPoint bottomLeftCorner = touchRectangle.Start;
                        SKPoint topLeftCorner = new SKPoint(touchRectangle.Start.X, touchRectangle.End.Y);
                        SKPoint bottomRightCorner = new SKPoint(touchRectangle.End.X, touchRectangle.Start.Y);
                        SKPoint topRightCorner = touchRectangle.End;
                        canvas.DrawLine(bottomLeftCorner, topLeftCorner, touchStroke);
                        canvas.DrawLine(topLeftCorner, topRightCorner, touchStroke);
                        canvas.DrawLine(topRightCorner, bottomRightCorner, touchStroke);
                        canvas.DrawLine(bottomRightCorner, bottomLeftCorner, touchStroke);
                    }
                }
            }

            foreach(var touchForm in temporaryForms.Values)
            {
                if(touchForm is ColoredPath touchPath)
                {
                    touchStroke.Color = touchPath.Color;
                    touchStroke.StrokeWidth = touchPath.StrokeWidth;
                    canvas.DrawPath(touchPath.Path, touchStroke);
                }

                if(touchForm is ColoredCircle touchCircle)
                {
                    touchStroke.Color = touchCircle.Color;
                    touchStroke.StrokeWidth = touchCircle.StrokeWidth;
                    canvas.DrawCircle(touchCircle.Center, touchCircle.Radius, touchStroke);
                }

                if (touchForm is ColoredLine touchLine)
                {
                    touchStroke.Color = touchLine.Color;
                    touchStroke.StrokeWidth = touchLine.StrokeWidth;
                    canvas.DrawLine(touchLine.Start, touchLine.End, touchStroke);
                }

                if (touchForm is ColoredRectangle touchRectangle)
                {
                    touchStroke.Color = touchRectangle.Color;
                    touchStroke.StrokeWidth = touchRectangle.StrokeWidth;
                    SKPoint bottomLeftCorner = touchRectangle.Start;
                    SKPoint topLeftCorner = new SKPoint(touchRectangle.Start.X, touchRectangle.End.Y);
                    SKPoint bottomRightCorner = new SKPoint(touchRectangle.End.X, touchRectangle.Start.Y);
                    SKPoint topRightCorner = touchRectangle.End;
                    canvas.DrawLine(bottomLeftCorner, topLeftCorner, touchStroke);
                    canvas.DrawLine(topLeftCorner, topRightCorner, touchStroke);
                    canvas.DrawLine(topRightCorner, bottomRightCorner, touchStroke);
                    canvas.DrawLine(bottomRightCorner, bottomLeftCorner, touchStroke);
                }
            }
        }

        //méthode définissant ce qu'il se passe quand on appuie sur l'écran. On créera certainement des sous-méthodes selon l'option à l'avenir
        private void SKCanvasView_Touch(object sender, SKTouchEventArgs e) 
        {
            if (option == "rubber")
                PathAction(e, SKColors.White);
            else if (option == "path")
                PathAction(e, _color);
            else if (option == "circle")
                CircleAction(e, _color);
            else if (option == "line")
                LineAction(e, _color);
            else if (option == "rectangle")
                RectangleAction(e, _color);

            e.Handled = true;
            ((SKCanvasView)sender).InvalidateSurface();
        }

        //Ces méthodes bouton permettant de choisir l'option seront à unifier (elles font toutes la même chose en fait)
        private void OptionButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.BackgroundColor = Color.Gray;

            foreach (var child in ButtonGrid.Children)
            {
                if (child is Button b && !b.Equals(button))
                    b.BackgroundColor = Color.LightGray;
            }

            if (button.Equals(PathButton))
                option = "path";
            else if (button.Equals(RubberButton))
                option = "rubber";
            else if (button.Equals(CircleButton))
                option = "circle";
            else if (button.Equals(LineButton))
                option = "line";
            else if (button.Equals(RectangleButton))
                option = "rectangle";
        }


        //Ce bouton vide la zône de dessin, sera potentiellement à réserver à l'hôte.
        private async void ClearButton_Clicked(object sender, EventArgs e)
        {
            var view = (SKCanvasView) this.FindByName("View");
            bool answer = await DisplayAlert ("WARNING", "This will clear the canvas for all users, are you sure you want to continue?", "Yes", "No");
            if (answer)
            {
                asyncClient.Send("CLEAR");
                forms.Clear();
                temporaryForms.Clear();
                view.InvalidateSurface();
            }
        }

        private async void ColorButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ColorPage(this));
        }

        private void PathAction(SKTouchEventArgs e, SKColor color)
        {
            switch (e.ActionType)
            {
                //Quand on appuie, commencer à dessiner
                case SKTouchAction.Pressed:
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    temporaryForms[e.Id] = new ColoredPath { Path = p, Color = color, StrokeWidth=strokeWidth };
                    break;
                //Quand on bouge et qu'on est en train d'appuyer, continuer à dessiner
                case SKTouchAction.Moved:
                    if (e.InContact && temporaryForms.ContainsKey(e.Id))
                        ((ColoredPath)temporaryForms[e.Id]).Path.LineTo(e.Location);
                    break;
                //Quand on relache, enregistrer le dessin
                case SKTouchAction.Released:
                    if (temporaryForms.ContainsKey(e.Id))
                    {
                        forms.Add(temporaryForms[e.Id]);
                        asyncClient.Send((ColoredPath)temporaryForms[e.Id]);
                        temporaryForms.Remove(e.Id);
                    }
                    break;
                //Quand on annule, faire disparaitre le dessin
                case SKTouchAction.Cancelled:
                    temporaryForms.Remove(e.Id);
                    break;
            }
        }

        private void CircleAction(SKTouchEventArgs e, SKColor color)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    var c = new ColoredCircle { Color = color, Center = e.Location, Radius = 0.1F, StrokeWidth=strokeWidth };
                    temporaryForms[e.Id] = c; 
                    break;
                case SKTouchAction.Moved:
                    if (e.InContact && temporaryForms.ContainsKey(e.Id))
                    {
                        c = (ColoredCircle) temporaryForms[e.Id];
                        c.Radius = (float)Math.Sqrt(Math.Pow(e.Location.X - c.Center.X, 2) + Math.Pow(e.Location.Y - c.Center.Y, 2));
                    }
                    break;
                case SKTouchAction.Released:
                    if (temporaryForms.ContainsKey(e.Id))
                    {
                        forms.Add(temporaryForms[e.Id]);
                        asyncClient.Send((ColoredCircle)temporaryForms[e.Id]);
                        temporaryForms.Remove(e.Id);
                    }
                    break;
                case SKTouchAction.Cancelled:
                    temporaryForms.Remove(e.Id);
                    break;
            }
        }

        private void LineAction(SKTouchEventArgs e, SKColor color)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    var l = new ColoredLine { Color = color, Start = e.Location, End = e.Location, StrokeWidth = strokeWidth };
                    temporaryForms[e.Id] = l;
                    break;
                case SKTouchAction.Moved:
                    
                    if (e.InContact && temporaryForms.ContainsKey(e.Id))
                    {
                        l = (ColoredLine)temporaryForms[e.Id];
                        l.End = e.Location;
                    }
                    break;
                case SKTouchAction.Released:
                    if (temporaryForms.ContainsKey(e.Id))
                    {
                        forms.Add(temporaryForms[e.Id]);
                        asyncClient.Send((ColoredLine)temporaryForms[e.Id]);
                        temporaryForms.Remove(e.Id);
                    }
                    break;
                case SKTouchAction.Cancelled:
                    temporaryForms.Remove(e.Id);
                    break;
            }
        }

        private void RectangleAction(SKTouchEventArgs e, SKColor color)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    var r = new ColoredRectangle { Color = color, Start = e.Location, End = e.Location, StrokeWidth = strokeWidth };
                    temporaryForms[e.Id] = r;
                    break;
                case SKTouchAction.Moved:

                    if (e.InContact && temporaryForms.ContainsKey(e.Id))
                    {
                        r = (ColoredRectangle)temporaryForms[e.Id];
                        r.End = e.Location;
                    }
                    break;
                case SKTouchAction.Released:
                    if (temporaryForms.ContainsKey(e.Id))
                    {
                        forms.Add(temporaryForms[e.Id]);
                        asyncClient.Send((ColoredRectangle)temporaryForms[e.Id]);
                        temporaryForms.Remove(e.Id);
                    }
                    break;
                case SKTouchAction.Cancelled:
                    temporaryForms.Remove(e.Id);
                    break;
            }
        }

        private void StrokeWidth_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            strokeWidth = (float)e.NewValue;
        }

        protected override bool OnBackButtonPressed()
        {
            if (hostServer != null)
                hostServer.listener.ListenerSocket.Close();
            return base.OnBackButtonPressed();
        }
    }
}
