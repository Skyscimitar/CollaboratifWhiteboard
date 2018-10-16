using System;
using System.Collections.Generic;
using mainGUI.TouchTracking;
using MainGUI.TouchTracking;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainGUI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPage : PopupPage
    {

        private static readonly List<ColorChoice> Colors;
        private static bool _colorsInitialized;
        private static ColorChoice _pickedColor;

        private const int ColorsPerRow = 5;
        private const int CanvasPadding = 5;
        private bool _colorChanged;

        private readonly SKPaint _clrPickPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        private readonly SKPaint _pickedClrPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5,
            IsAntialias = true,
        };

        static ColorPage()
        {
            Colors = new List<ColorChoice> {

                new ColorChoice("#25c5db"),
                new ColorChoice("#0098a6"),
                new ColorChoice("#0e47a1"),
                new ColorChoice("#1665c1"),
                new ColorChoice("#039be6"),

                new ColorChoice("#64b5f6"),
                new ColorChoice("#ff7000"),
                new ColorChoice("#ff9f00"),
                new ColorChoice("#ffb200"),
                new ColorChoice("#cf9702"),

                new ColorChoice("#8c6e63"),
                new ColorChoice("#6e4c42"),
                new ColorChoice("#d52f31"),
                new ColorChoice("#ff1643"),
                new ColorChoice("#f44236"),

                new ColorChoice("#ec407a"),
                new ColorChoice("#ad1457"),
                new ColorChoice("#6a1b9a"),
                new ColorChoice("#ab48bf"),
                new ColorChoice("#b968c7"),

                new ColorChoice("#00695b"),
                new ColorChoice("#00887a"),
                new ColorChoice("#4cb6ac"),
                new ColorChoice("#307c32"),
                new ColorChoice("#43a047"),

                new ColorChoice("#64dd16"),
                new ColorChoice("#222222"),
                new ColorChoice("#5f7c8c"),
                new ColorChoice("#b1bec6"),
                new ColorChoice("#465a65"),

                new ColorChoice("#607d8d"),
                new ColorChoice("#91a5ae"),
            };
        }


        public ColorPage()
        {
        }

        public event EventHandler<ColorChangedEventArgs> ColorChanged;


        private async void OnClose(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }

        private async void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            if (!_colorsInitialized)
            {
                InitializeColorPicks(info.Width);
            }

            // draw the colors
            foreach (var cp in Colors)
            {
                _clrPickPaint.Color = cp.Color.ToSKColor();
                canvas.DrawCircle(cp.Position.X, cp.Position.Y, cp.Radius, _clrPickPaint);
            }

            // check if there is a selected color
            if (_pickedColor == null) { return; }

            // draw the highlight for the picked color
            _pickedClrPaint.Color = _pickedColor.Color.ToSKColor();
            canvas.DrawCircle(_pickedColor.Position.X, _pickedColor.Position.Y, _pickedColor.Radius + 10, _pickedClrPaint);

            if (_colorChanged)
            {
                ColorChanged?.Invoke(this, new ColorChangedEventArgs(_pickedColor.Color));
                _colorChanged = false;
                await Navigation.PopPopupAsync();
            }
        }

        private static void InitializeColorPicks(int skImageWidth)
        {
            var contentWidth = skImageWidth - (CanvasPadding * 2);
            var colorWidth = contentWidth / ColorsPerRow;

            var sp = new SKPoint((colorWidth / 2) + CanvasPadding, (colorWidth / 2) + CanvasPadding);
            var col = 1;
            var row = 1;
            var radius = (colorWidth / 2) - 10;

            foreach (var color in Colors)
            {
                if (col > ColorsPerRow)
                {
                    col = 1;
                    row += 1;
                }
                // calc current position
                var x = sp.X + (colorWidth * (col - 1));
                var y = sp.Y + (colorWidth * (row - 1));

                color.Radius = radius;
                color.Position = new SKPoint(x, y);
                col += 1;
            }
            _colorsInitialized = true;
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            if (args.Type == TouchActionType.Released)
            {
                // get the sk point pixel
                var pnt = ConvertToPixel(args.Location);

                // loop through all colors
                foreach (var color in Colors)
                {
                    // check if selecting a color
                    if (color.IsTouched(pnt))
                    {
                        _colorChanged = true;
                        _pickedColor = color;
                        break; // get out of loop
                    }
                }
                canvasView.InvalidateSurface();
            }
        }

        private SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }
    }
}