using System.Collections.Generic;
using System.Linq;
using ProjectWPF.Drawing.Primitives;

namespace ProjectWPF.Drawing.ClippingAlgorithms.PolygonClipping
{
    public class WeilerAthertonAlgorithm : IPolygonClippingAlgorithm
    {
        public IEnumerable<Line> Clip(Polygon polygon, Polygon clipPolygon)
        {
            var intersectionPoints = polygon.IntersectWith(clipPolygon).ToList();

            if (!intersectionPoints.Any())
            {
                if (clipPolygon.HasPointInside(polygon.Points[0]))
                {
                    return polygon.Lines;
                }

                return null;
            }

            var polygonPoints = IntersectionEnrich(polygon, intersectionPoints);
            var borderPoints = IntersectionEnrich(clipPolygon, intersectionPoints);

            var inOut = InOutIntersection(intersectionPoints, polygon, clipPolygon);
            
            return GetClippingPolygon(polygonPoints, inOut);
        }

        private static IList<Point> IntersectionEnrich(Polygon polygon, IEnumerable<Point> intersection)
        {
            IList<Point> points = new List<Point>(polygon.Points);

            var intersectionArray = intersection as Point[] ?? intersection.ToArray();
            for (var i = 0; i < points.Count; i++)
            {
                var line = new Line(points[i], points[(i + 1) % points.Count]);
                foreach (var point in intersectionArray.Where(point => line.HasPointOnLine(point)))
                {
                    points.Insert(++i, point);
                }
            }

            return points;
        }

        private static IList<(Point, bool)> InOutIntersection(List<Point> intersection, Polygon polygon,
            Polygon clipPolygon)
        {
            var isOutside = !clipPolygon.HasPointInside(polygon.Points[0]);
            var result = new List<(Point, bool)>(intersection.Count);
            foreach (var point in intersection)
            {
                result.Add((point, isOutside));
                isOutside = !isOutside;
            }

            return result;
        }

        private IEnumerable<Line> GetClippingPolygon(IList<Point> enrichPolygon,
            IList<(Point, bool)> inOutIntersection)
        {
            var clipping = new List<Line>();
            var points = enrichPolygon;

            var intersectIndex = inOutIntersection.IndexOf(inOutIntersection
                .First(p => p.Item2));
            var start = inOutIntersection[intersectIndex].Item1;
            
            var index = points.IndexOf(start);
            var current = start;
            Point next;
            do
            {
                index = (index + 1) % points.Count;
                next = points[index];
                clipping.Add(new Line(current, next));
                current = next;
                if (IsIntersection(next, inOutIntersection))
                {
                    intersectIndex = (intersectIndex + 2) % inOutIntersection.Count;
                    current = inOutIntersection[intersectIndex].Item1;
                    next = current;
                    index = points.IndexOf(current);
                }
            } while (!start.Equals(next));

            return clipping;
        }

        private static bool IsIntersection(int index, IList<Point> points,
            IEnumerable<(Point, bool)> inOutIntersection)
        {
            var point = points[index];
            return inOutIntersection
                .ToList().Exists(pair => pair.Item1.Equals(point));
        }

        private static bool IsIntersection(Point point,
            IEnumerable<(Point, bool)> inOutIntersection)
        {
            return inOutIntersection
                .ToList().Exists(pair => pair.Item1.Equals(point));
        }
    }
}