using System.Collections.Generic;
using System.Linq;

namespace ProjectWPF.Drawing.Primitives
{
    public struct Polygon
    {
        public List<Line> Lines
        {
            get
            {
                var result = new List<Line>();
                for (var i = 0; i < Points.Count; i++)
                {
                    result.Add(new Line(Points[i], Points[(i + 1) % Points.Count]));
                }

                return result;
            }
        }

        public IReadOnlyList<Point> Points { get; set; } 

        public Polygon(IEnumerable<Point> points)
        {
            var pointsArray = points as Point[] ?? points.ToArray();
            Points = new List<Point>(pointsArray);
            
            for (var i = 0; i < pointsArray.Length; i++)
            {
                Lines.Add(new Line(pointsArray[i], pointsArray[(i + 1) % pointsArray.Length]));
            }
        }

        public Polygon(IEnumerable<(double X, double Y)> points)
        {
            var pointsArray = points as (double X, double Y)[] ?? points.ToArray();
            Points = pointsArray.Select(p => new Point(p.X, p.Y)).ToList();
            
            for (var i = 0; i < pointsArray.Length; i++)
            {
                var p1 = new Point(pointsArray[i].X,
                    pointsArray[i].Y);
                var p2 = new Point(pointsArray[(i + 1) % pointsArray.Length].X,
                    pointsArray[(i + 1) % pointsArray.Length].Y);
                
                Lines.Add(new Line(p1, p2));
            }
        }

        public IEnumerable<Point> IntersectWith(Polygon polygon)
        {
            return Lines.SelectMany(line => line.IntersectWith(polygon)).Distinct().Except(Points);
        }

        public bool HasPointInside(Point point)
        {
            var result = false;
            
            double minX = Points.Select(p => p.X).Min();
            double maxX = Points.Select(p => p.X).Max();
            double minY = Points.Select(p => p.Y).Min();
            double maxY = Points.Select(p => p.Y).Max();

            if (point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY)
            {
                return false;
            }
            
            var j = Points.Count - 1;
            
            for (var i = 0; i < Points.Count; j = i++)
            {
                if ((Points[i].Y > point.Y) != (Points[j].Y > point.Y) &&
                    point.X < (Points[j].X - Points[i].X) *
                        (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) + Points[i].X)
                {
                    result = !result;
                }
            }

            return result;
        }

        public void AddVector(double v1, double v2)
        {
            Points = Points.Select(p => p.AddVector(v1, v2)).ToList();
        }
    }
}