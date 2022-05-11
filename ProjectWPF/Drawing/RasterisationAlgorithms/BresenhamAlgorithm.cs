using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWPF.Drawing.Primitives;

namespace ProjectWPF.Drawing.RasterisationAlgorithms
{
    public class BresenhamAlgorithm : IRasterisationAlgorithm
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
}