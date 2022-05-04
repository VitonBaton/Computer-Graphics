using System;
using System.Windows;
using ProjectWPF.Drawing.Primitives;
using Point = ProjectWPF.Drawing.Primitives.Point;

namespace ProjectWPF.Drawing.ClippingAlgorithms.LineClipping
{
    public class CohenSutherlandAlgorithm : ILineClippingAlgorithm
    {
        [Flags]
        private enum Direction{
            None = 0,
            Left = 1 << 0,
            Right = 1 << 1,
            Bot = 1 << 2,
            Top = 1 << 3
        }
        
        public Line? Clip(Line line, Rect rect)
        {
            var x1 = line.P1.X;
            var y1 = line.P1.Y;
            var x2 = line.P2.X;
            var y2 = line.P2.Y;

            if (!CohenSutherland(rect, ref x1, ref y1, ref x2,
                    ref y2))
            {
                return null;
            }

            return new Line
            {
                P1 = new Point(x1,y1),
                P2 = new Point(x2,y2)
            };
        }

        private static bool CohenSutherland(Rect rect,
            ref int xi1,
            ref int yi1,
            ref int xi2,
            ref int yi2)
        {
            double x1 = xi1;
            double y1 = yi1;
            double x2 = xi2;
            double y2 = yi2;
            var res = CohenSutherland(rect, ref x1, ref y1, ref x2, ref y2);
            xi1 = (int) x1;
            yi1 = (int) y1;
            xi2 = (int) x2;
            yi2 = (int) y2;
            return res;
        }

        private static bool CohenSutherland(Rect rect,
            ref double x1,
            ref double y1,
            ref double x2,
            ref double y2)
        {
            var codeA = FindPointCode(rect, x1, y1);
            var codeB = FindPointCode(rect, x2, y2);

            if (codeA ==  0 && codeB == 0)
            {
                return true;
            }

            while ((codeA | codeB) != 0)
            {
                if ((codeA & codeB) != 0)
                {
                    return false;
                }

                double x;
                double y;
                Direction code;
                if (codeA != 0)
                {
                    code = codeA;
                    x = x1;
                    y = y1;
                }
                else
                {
                    code = codeB;
                    x = x2;
                    y = y2;
                }
                if (code.HasFlag(Direction.Left))
                {
                    y += (y1 - y2) * (rect.Left - x) / (x1 - x2);
                    x = rect.Left;
                }
                else if (code.HasFlag(Direction.Right))
                {
                    y += (y1 - y2) * (rect.Right - x) / (x1 - x2);
                    x = rect.Right;
                }
                else if (code.HasFlag(Direction.Bot))
                {
                    x += (x1 - x2) * (rect.Bottom - y) / (y1 - y2);
                    y = rect.Bottom;
                }
                else if (code.HasFlag(Direction.Top))
                {
                    x += (x1 - x2) * (rect.Top - y) / (y1 - y2);
                    y = rect.Top;
                }

                if (code == codeA)
                {
                    x1 = x;
                    y1 = y;
                    codeA = FindPointCode(rect, x1, y1);
                }
                else
                { 
                    x2 = x;
                    y2 = y;
                    codeB = FindPointCode(rect, x2, y2);
                }
            }

            return true;
        }
        
        private static Direction FindPointCode(Rect rect, double x, double y)
        {
            var outCode = new Direction();
            if (x < rect.Left)
                outCode |=  Direction.Left;
            else if (x > rect.Right)
                outCode |=  Direction.Right;
            if (y > rect.Bottom)
                outCode |=  Direction.Bot;
            else if (y < rect.Top)
                outCode |=  Direction.Top;
            return outCode;
        }
    }
}