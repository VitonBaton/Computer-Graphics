//using System.Drawing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWPF.Drawing.ClippingAlgorithms.LineClipping;
using ProjectWPF.Drawing.ClippingAlgorithms.PolygonClipping;
using ProjectWPF.Drawing.Primitives;
using ProjectWPF.Drawing.RasterisationAlgorithms;
using Point = ProjectWPF.Drawing.Primitives.Point;

namespace ProjectWPF.Drawing
{
    public static class PrimitivesDrawerExtension
    {
        public static void DrawLineP(this WriteableBitmap bitmap, Line line, Color color,
            IRasterisationAlgorithm drawAlg, Rect? clipRect = null)
        {
            using (bitmap.GetBitmapContext())
            {
                var topX = 0d;
                var topY = 0d;
                var num1 = bitmap.Width;
                var num2 = bitmap.Height;

                if (clipRect.HasValue)
                {
                    var rect = clipRect.Value;
                    topX = rect.X;
                    topY = rect.Y;
                    num1 = rect.Width;
                    num2 = rect.Height;
                }

                var alg = new CohenSutherlandAlgorithm();

                var newLine = alg.Clip(line, new Rect(topX, topY, num1, num2));

                if (newLine is null)
                {
                    return;
                }

                drawAlg.DrawLine(bitmap, newLine.Value, color);
            }
        }

        public static void DrawCircleP(this WriteableBitmap bitmap,int x0, int y0, int radius, Color color)
        {
            if (x0 + radius >= bitmap.Width || y0+radius >= bitmap.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), "Окружность выходит за границы изображения");
            }
            
            
            void Plot(int first, int second) => bitmap.SetPixel(first, second, color);
            
            var x = radius;
            var y = 0;
            var radiusError = 1 - x;
            while (x >= y)
            {
                Plot(x + x0, y + y0);
                Plot(y + x0, x + y0);
                Plot(-x + x0, y + y0);
                Plot(-y + x0, x + y0);
                Plot(-x + x0, -y + y0);
                Plot(-y + x0, -x + y0);
                Plot(x + x0, -y + y0);
                Plot(y + x0, -x + y0);
                y++;
                if (radiusError < 0)
                {
                    radiusError += 2 * y + 1;
                }
                else
                {
                    x--;
                    radiusError += 2 * (y - x + 1);
                }
            }
        }

        public static void DrawPolygon(this WriteableBitmap bitmap, Polygon polygon, Color color,
            IRasterisationAlgorithm drawAlg, Polygon? clipPolygon = null, Point? center = null)
        {
            IEnumerable<Line> lines = polygon.Lines;
            if (clipPolygon != null)
            {
                var alg = new WeilerAthertonAlgorithm();
                var newPolygon = alg.Clip(polygon, clipPolygon.Value);
                if (newPolygon is null)
                {
                    return;
                }

                lines = newPolygon;
            }

            if (center != null)
            {
                lines = lines.Select(line => new Line(
                    new Point(center.Value.X + line.P1.X, center.Value.Y - line.P1.Y),
                    new Point(center.Value.X + line.P2.X, center.Value.Y - line.P2.Y)
                ));
            }
            
            foreach (var line in lines)
            {
                bitmap.DrawLineP(line, color,drawAlg);
            }
        }
    }
}