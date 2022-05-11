using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWPF.Drawing.Primitives;

namespace ProjectWPF.Drawing.RasterisationAlgorithms
{
    public class WuAlgorithm : IRasterisationAlgorithm
    {
        public void DrawLine(WriteableBitmap bitmap, Line line, Color color)
        {
            var x1 = (int) line.P1.X;
            var y1 = (int) line.P1.Y;
            var x2 = (int) line.P2.X;
            var y2 = (int) line.P2.Y;
                
            if (x1 == x2 && y1 == y2)
            {
                bitmap.SetPixel(x1, y1, color);
                return;
            }

            if (x1 == x2 || y1 == y2)
            {
                var alg = new BresenhamAlgorithm();
                alg.DrawLine(bitmap, line, color);
                return;
            }
            
            var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            if (steep)
            {
                (x1, y1) = (y1, x1);
                (x2, y2) = (y2, x2);
            }
            if (x1 > x2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }

            void DrawPoint(bool side,int xCoord,int yCoord,double intense)
            {
                if (intense > 1 || intense < 0)
                {
                    throw new ArgumentException("Intense should be in range from 0 to 1");
                }

                var newColor = new Color
                {
                    ScA = (float) intense,
                    ScB = color.ScB,
                    ScG = color.ScG,
                    ScR = color.ScR
                };

                bitmap.SetPixel(side ? yCoord : xCoord, side ? xCoord : yCoord, newColor);
            }
            
            DrawPoint(steep, x2, y2, 1); // Эта функция автоматом меняет координаты местами в зависимости от переменной steep
            DrawPoint(steep, x1, y1, 1); // Последний аргумент — интенсивность в долях единицы
            double dx = x2 - x1;
            double dy = y2 - y1;
            var gradient = dy / dx;
            var y = y1 + gradient;
            for (var x = x1 + 1; x <= x2 - 1; x++)
            {
                DrawPoint(steep, x, (int)y, 1 - (y - (int)y));
                DrawPoint(steep, x, (int)y + 1, y - (int)y);
                y += gradient;
            }
        }
    }
}