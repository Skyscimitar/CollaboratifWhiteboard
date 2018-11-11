using System;
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
    /*Page principale contenant le whiteboard*/
    public partial class MainPage : ContentPage
    {        
        //formes en train d'être dessinées
        private readonly Dictionary<long,object> temporaryForms =new Dictionary<long,object>(); 
        //formes finies
        private List<object> forms = new List<object>();
        //variable stockant l'option choisie par l'utilisateur (trait, gomme, cercle, etc.)
        private string option; 
        //couleur utilisée pour afficher les dessins
        private SKColor _color  = SKColors.Black;
        /*variable initialisée à la largeur de l'écran,
         mais prenant la valeur de la largeur de l'écran hôte au moment de la connexion*/
        private float width;
        //Idem width
        private float height;
        //Adresse IP locale
        public string IpAddress { get; }
        //Server hôte
        private readonly HostServer hostServer;
        //couleur utilisée pour le bouton couleur
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
        //Largeur du trait pour les dessins
        private float strokeWidth = 5;
        //Client qui communique les données du whiteboard avec le serveur
        private readonly AsyncClient asyncClient;

        //Constructeur appelé quand on demande à être hôte
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

        //Constructeur appelé quand on souhaite être client
        public MainPage(string ip)
        {
            BindingContext = this;
            IpAddress = String.Format("Whiteboard IP: {0}", ip);
            InitializeComponent();
            UpdateUIEventHandler.OnUpdateUI += UpdateUi;
            asyncClient = new AsyncClient(ip);
            asyncClient.StartClient();
        }

        //Evenement déclenché quand une nouvelle forme ou instruction est reçue par le client
        private void UpdateUi(Object o, UpdateUIEventArgs eventArgs)
        {
            lock (forms)
            {
                switch (eventArgs.Type)
                {
                    case "PATH":
                        ColoredPath coloredPath = eventArgs.Path;
                        forms.Add(coloredPath);
                        break;
                    case "CIRCLE":
                        ColoredCircle coloredCircle = eventArgs.Circle;
                        forms.Add(coloredCircle);
                        break;
                    case "LINE":
                        ColoredLine coloredLine = eventArgs.Line;
                        forms.Add(coloredLine);
                        break;
                    case "RECTANGLE":
                        ColoredRectangle coloredRectangle = eventArgs.Rectangle;
                        forms.Add(coloredRectangle);
                        break;
                    case "CLEAR":
                        forms.Clear();
                        break;
                    case "SIZE":
                        width = eventArgs.Width;
                        height = eventArgs.Height;
                        break;
                    case "REQUEST_STATUS":
                        asyncClient.RestoreWhiteboard(this.forms, eventArgs.client_id);
                        break;
                    case "RESTORE":
                        forms = eventArgs.Forms;
                        break;
                }
            View.InvalidateSurface();
            }
        }

        //Evenement déclenché à chaque changement sur le whiteboard, redessinant toutes les formes
        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
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
            /*canvas.Scale(sx, sy); 
            Decommenter pour avoir une version qui scale selon la taille de la fenêtre, buggée*/

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

        //Methode appelée lorsqu'on se déplace sur l'écran
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

        //Définit l'option choisie selon le bouton sur lequel on a appuyé
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


        //Ce bouton vide la zône de dessin et génère un popup pour confirmation.
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

        //Ouvre le popup des couleurs quand on appuie sur le bouton couleurs
        private async void ColorButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ColorPage(this));
        }

        //Appelée quand l'option est path et qu'on se déplace sur le canvas
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

        //Idem PathAction avec des cercles
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

        //Idem PathAction avec des lignes droites
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
        
        //Idem PathAction avec des rectangles
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

        //Appelée lorsque le slider Strokewidth est déplacé
        private void StrokeWidth_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            strokeWidth = (float)e.NewValue;
        }

        //Déconnecte le server quand on clique sur précédent.
        protected override bool OnBackButtonPressed()
        {
            if (hostServer != null)
                hostServer.Listener.ListenerSocket.Close();
            return base.OnBackButtonPressed();
        }
    }
}
