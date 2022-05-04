using System;

namespace ProjectWPF.Drawing.Primitives
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point AddVector(double v1, double v2)
        {
            return new Point(X + v1, Y + v2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point point)
            {
                return Math.Abs(X - point.X) < 0.001 && Math.Abs(Y - point.Y) < 0.001;
            }
            return base.Equals(obj);
        }
    }
}