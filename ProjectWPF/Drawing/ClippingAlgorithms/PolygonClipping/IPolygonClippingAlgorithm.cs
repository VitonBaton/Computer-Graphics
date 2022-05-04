using System.Collections.Generic;
using ProjectWPF.Drawing.Primitives;

namespace ProjectWPF.Drawing.ClippingAlgorithms.PolygonClipping
{
    public interface IPolygonClippingAlgorithm
    {
        IEnumerable<Line> Clip(Polygon polygon, Polygon clipPolygon);
    }
}