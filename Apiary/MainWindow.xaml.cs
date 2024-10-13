using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Numerics;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Apiary
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private DateTime lastUpdateTime = DateTime.Now;
        private int frameCount = 0;
        private double fps = 0;
        private CanvasGeometry hexagonGeometry;
        private CanvasRenderTarget hexagonRenderTarget;
        private GaussianBlurEffect blurEffect;

        public MainWindow()
        {
            this.InitializeComponent();
            CompositionTarget.Rendering += UpdateFps;
        }

        private void HexagonCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            float radius = 75;
            //Vector2 center = new Vector2((float)sender.ActualWidth / 2, (float)sender.ActualHeight / 2);
            Vector2 location = new Vector2(250, 250);

            //for (var x=0; x<=500;x++) {
            //    DrawGlowHexagon(ds, sender, new Vector2(250f, 250f), radius, Colors.Cyan);
            //}
            DrawGlowHexagon(ds, sender, new Vector2(250f, 250f), radius, Colors.Cyan);
            DrawGlowHexagon(ds, sender, new Vector2(350f, 250f), radius, Colors.Yellow);
            DrawGlowHexagon(ds, sender, new Vector2(555f, 250f), radius, Colors.HotPink);
            DrawGlowHexagon(ds, sender, new Vector2(250f, 355f), radius, Colors.SkyBlue);
            DrawGlowHexagon(ds, sender, new Vector2(550f, 355f), radius, Colors.DarkCyan);
        }


        private void DrawGlowHexagon(CanvasDrawingSession ds, ICanvasResourceCreator resourceCreator, Vector2 location, float radius, Windows.UI.Color color)
        {
            // Create the hexagon geometry if not already created
            if (hexagonGeometry == null)
            {
                hexagonGeometry = CreateHexagonGeometry(resourceCreator, new Vector2(radius, radius), radius);
            }

            // Create the hexagon render target if not already created
            float targetSize = radius * 10;
            //if (hexagonRenderTarget == null)
            {

                // Step 1: Draw the hexagon to an initial render target
                var initialRenderTarget = new CanvasRenderTarget(resourceCreator, targetSize, targetSize, 96);
                using (var clds = initialRenderTarget.CreateDrawingSession())
                {
                    clds.Clear(Colors.Transparent);
                    clds.DrawGeometry(hexagonGeometry, color, 10);
                }

                // Step 2: Apply the Gaussian blur effect to create the blurred version of the hexagon
                var blurEffect = new GaussianBlurEffect
                {
                    Source = initialRenderTarget,
                    BlurAmount = 15.0f
                };

                // Step 3: Create the final hexagon render target to store the blurred effect in the passed color
                hexagonRenderTarget = new CanvasRenderTarget(resourceCreator, targetSize, targetSize, 96);
                using (var clds = hexagonRenderTarget.CreateDrawingSession())
                {
                    clds.Clear(Colors.Transparent);
                    Vector2 offset = new Vector2(50, 50);
                    clds.DrawImage(blurEffect, offset);
                    clds.DrawGeometry(hexagonGeometry, offset, color, 5); // Draw the hexagon line in white
                    clds.DrawGeometry(hexagonGeometry, offset, Colors.White, 3); // Draw the hexagon line in white
                }
            }

            // Draw the hexagon on top of the glow
            ds.DrawImage(hexagonRenderTarget, location);

            // Draw FPS
            ds.DrawText($"FPS: {fps:F1}", 10, 10, Colors.Yellow);
        }

        private CanvasGeometry CreateHexagonGeometry(ICanvasResourceCreator resourceCreator, Vector2 center, float radius)
        {
            using (var pathBuilder = new CanvasPathBuilder(resourceCreator))
            {
                for (int i = 0; i < 6; i++)
                {
                    float angle = (float)(Math.PI / 3.0 * i);
                    float x = center.X + radius * (float)Math.Cos(angle);
                    float y = center.Y + radius * (float)Math.Sin(angle);

                    if (i == 0)
                    {
                        pathBuilder.BeginFigure(x, y);
                    }
                    else
                    {
                        pathBuilder.AddLine(x, y);
                    }
                }

                pathBuilder.EndFigure(CanvasFigureLoop.Closed);
                return CanvasGeometry.CreatePath(pathBuilder);
            }
        }

        private void UpdateFps(object sender, object e)
        {
            frameCount++;
            var currentTime = DateTime.Now;
            var elapsedSeconds = (currentTime - lastUpdateTime).TotalSeconds;
            if (elapsedSeconds >= 1.0)
            {
                fps = frameCount / elapsedSeconds;
                frameCount = 0;
                lastUpdateTime = currentTime;
                HexagonCanvas.Invalidate(); // Forces the canvas to redraw
            }
        }


    }


}
