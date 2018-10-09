using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace mainGUI
{
    //Ici on définit les actions à effectuer sur cette page.
    public partial class MainPage : ContentPage
    {
        private Dictionary<long,object> temporaryForms =new Dictionary<long,object>();
        private List<object> forms = new List<object>();
        private Dictionary<string, Dictionary<long, object>> temporaryFormsClients = new Dictionary<string, Dictionary<long, object>>();
        private Dictionary<string, List<object>> formsClients = new Dictionary<string, List<object>>();
        private string option; //variable stockant l'option choisie par l'utilisateur (trait, gomme, cercle, etc.)
        private SKColor color = SKColors.Black;

        public MainPage()
        {
            InitializeComponent();
        }


        private void OnPainting(object sender, SKPaintSurfaceEventArgs e) //méthode définissant ce qui s'affiche à l'écran en temps réel
        {
            var surface = e.Surface;
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var touchPathStroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5
            };

            var touchCircleStroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5
            };

            foreach (var touchForm in forms)
            {
                if (touchForm is ColoredPath touchPath)
                {
                    touchPathStroke.Color = touchPath.Color;
                    canvas.DrawPath(touchPath.Path, touchPathStroke);
                }

                if (touchForm is ColoredCircle touchCircle)
                {
                    touchCircleStroke.Color = touchCircle.Color;
                    canvas.DrawCircle(touchCircle.Center, touchCircle.Radius, touchCircleStroke);
                }
            }

            foreach(var touchForm in temporaryForms.Values)
            {
                if(touchForm is ColoredPath touchPath)
                {
                    touchPathStroke.Color = touchPath.Color;
                    canvas.DrawPath(touchPath.Path, touchPathStroke);
                }

                if(touchForm is ColoredCircle touchCircle)
                {
                    touchCircleStroke.Color = touchCircle.Color;
                    canvas.DrawCircle(touchCircle.Center, touchCircle.Radius, touchCircleStroke);
                }
            }
        }

        //méthode définissant ce qu'il se passe quand on appuie sur l'écran. On créera certainement des sous-méthodes selon l'option à l'avenir
        private void SKCanvasView_Touch(object sender, SKTouchEventArgs e) 
        {
            if (option == "rubber")
                PathAction(e, SKColors.White);
            else if (option == "path")
                PathAction(e, color);
            else if (option == "circle")
                CircleAction(e, color);

            e.Handled = true;
            ((SKCanvasView)sender).InvalidateSurface();
        }

        //Ces méthodes bouton permettant de choisir l'option seront à unifier (elles font toutes la même chose en fait)
        private void OptionButton_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (button.Equals(FindByName("PathButton")))
                option = "path";
            else if (button.Equals(FindByName("RubberButton")))
                option = "rubber";
            else if (button.Equals(FindByName("CircleButton")))
                option = "circle";
            foreach (Button b in ((Grid)button.Parent).Children)
                b.BackgroundColor = Color.LightGray;
            button.BackgroundColor = Color.Gray;
        }


        //Ce bouton vide la zône de dessin, sera potentiellement à réserver à l'hôte.
        private void ClearButton_Clicked(object sender, EventArgs e)
        {
            var view = (SKCanvasView) this.FindByName("View");
            forms.Clear();
            temporaryForms.Clear();
            view.InvalidateSurface();
        }

        private void PathAction(SKTouchEventArgs e, SKColor color)
        {
            switch (e.ActionType)
            {
                //Quand on appuie, commencer à dessiner
                case SKTouchAction.Pressed:
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    temporaryForms[e.Id] = new ColoredPath { Path = p, Color = color };
                    break;
                //Quand on bouge et qu'on est en train d'appuyer, continuer à dessiner
                case SKTouchAction.Moved:
                    if (e.InContact)
                        ((ColoredPath)temporaryForms[e.Id]).Path.LineTo(e.Location);
                    break;
                //Quand on relache, enregistrer le dessin
                case SKTouchAction.Released:
                    forms.Add(temporaryForms[e.Id]);
                    temporaryForms.Remove(e.Id);
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
                    var c = new ColoredCircle { Color = color, Center = e.Location, Radius = 0.1F };
                    temporaryForms[e.Id] = c;
                    break;
                case SKTouchAction.Moved:
                    if (e.InContact)
                    {
                        c = (ColoredCircle) temporaryForms[e.Id];
                        c.Radius = (float)Math.Sqrt(Math.Pow(e.Location.X - c.Center.X, 2) + Math.Pow(e.Location.Y - c.Center.Y, 2));
                    }
                    break;
                case SKTouchAction.Released:
                    forms.Add(temporaryForms[e.Id]);
                    temporaryForms.Remove(e.Id);
                    break;
                case SKTouchAction.Cancelled:
                    temporaryForms.Remove(e.Id);
                    break;
            }
        }
    }
}
