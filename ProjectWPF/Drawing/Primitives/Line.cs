using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace ProjectWPF.Drawing.Primitives
{
    public struct Line
    {
        public Point P1 { get; set; }
        public Point P2 { get; set; }

        public Line(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public IEnumerable<Point> IntersectWith(Polygon polygon)
        {
            var thisLine = this;
            return polygon.Lines
                .Select(line => line.IntersectWith(thisLine))
                .Where(p => p.HasValue)
                .Select(p => p.Value);
        }
        
        public Point? IntersectWith(Line line)
        {
            return IntersectWith(line.P1, line.P2);
        }

        private Point? IntersectWith(Point p1, Point p2)
        {
            var x1 = this.P1.X;
            var y1 = this.P1.Y;
            var x2 = this.P2.X;
            var y2 = this.P2.Y;
            
            var x3 = p1.X;
            var y3 = p1.Y;
            var x4 = p2.X;
            var y4 = p2.Y;
            
            var s1_x = x2 - x1;
            var s1_y = y2 - y1;
            var s2_x = x4 - x3;
            var s2_y = y4 - y3;

            double s, t;
            double d = -s2_x * s1_y + s1_x * s2_y;

            s = (-s1_y * (x1 - x3) + s1_x * (y1 - y3)) / d;
            t = (s2_x * (y1 - y3) - s2_y * (x1 - x3)) / d;

            if (!(s >= 0) || !(s <= 1) || !(t >= 0) || !(t <= 1)) return null;
            
            // Collision detected
            var x = x1 + t * s1_x;
            var y = y1 + t * s1_y;
            return new Point(x, y);
        }

        public bool HasPointOnLine(Point p)
        {
            var a = P1;
            var b = P2;

            var crossProduct = (p.Y - a.Y) * (b.X - a.X)
                                 - (p.X - a.X) * (b.Y - a.Y);
            if (Math.Abs(crossProduct) > 0.1) {
                return false;
            }

            var dotProduct = (p.X - a.X) * (b.X - a.X)
                                + (p.Y - a.Y) * (b.Y - a.Y);
            if (Math.Abs(dotProduct) < 0.1) {
                return false;
            }

            var squaredLengthBa = (b.X - a.X) * (b.X - a.X)
                                     + (b.Y - a.Y) * (b.Y - a.Y);
            return !(dotProduct > squaredLengthBa);
        }
    }
}