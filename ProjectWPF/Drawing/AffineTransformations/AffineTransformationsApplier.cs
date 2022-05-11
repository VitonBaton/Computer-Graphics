using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using ProjectWPF.Drawing.Primitives;

namespace ProjectWPF.Drawing.AffineTransformations
{
    public static class AffineTransformationsApplier
    {
        public static Polygon Apply(Polygon polygon, params IAffineTransformation[] transformations)
        {
            var transformation = transformations.Select(trans => trans.GetTransformation())
                .Aggregate(Matrix.Build.DenseDiagonal(3, 3, 1), (current, m) => current.Multiply(m));
            return new Polygon(polygon.Points.Select(p =>
            {
                var vector = Matrix.Build.DenseOfRows(new[] {new[] {p.X, p.Y, 1}});
                var point = vector.Multiply(transformation);
                return new Point(point[0, 0], point[0, 1]);
            }));
        }
    }
}