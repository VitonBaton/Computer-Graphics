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
using Point = ProjectWPF.Drawing.Primitives.Point;

namespace ProjectWPF.Drawing
{
    public static class PrimitivesDrawerExtension
    {
        public static void DrawLineP(this WriteableBitmap bitmap, Line line, Color color, Rect? clipRect = null)
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

                var x1 = (int) newLine.Value.P1.X;
                var y1 = (int) newLine.Value.P1.Y;
                var x2 = (int) newLine.Value.P2.X;
                var y2 = (int) newLine.Value.P2.Y;
                
                if (x1 == x2 && y1 == y2)
                {
                    bitmap.SetPixel(x1, y1, color);
                    return;
                }
                
                var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1); // Проверяем рост отрезка по оси икс и по оси игрек
                // Отражаем линию по диагонали, если угол наклона слишком большой
                if (steep)
                {
                    (x1, y1) = (y1, x1);
                    (x2, y2) = (y2, x2);
                }

                // Если линия растёт не слева направо, то меняем начало и конец отрезка местами
                if (x1 > x2)
                {
                    (x1, x2) = (x2, x1);
                    (y1, y2) = (y2, y1);
                }

                var dx = x2 - x1;
                var dy = Math.Abs(y2 - y1);
                var error = dx /
                            2; // Здесь используется оптимизация с умножением на dx, чтобы избавиться от лишних дробей
                var yStep = (y1 < y2) ? 1 : -1; // Выбираем направление роста координаты y
                var y = y1;
                for (var x = x1; x <= x2; x++)
                {
                    bitmap.SetPixel(steep ? y : x, steep ? x : y, color); // Не забываем вернуть координаты на место
                    error -= dy;
                    if (error < 0)
                    {
                        y += yStep;
                        error += dx;
                    }
                }
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
            Polygon? clipPolygon = null)
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

            foreach (var line in lines)
            {
                bitmap.DrawLineP(line, color);
            }
        }
    }
}