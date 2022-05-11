

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ProjectWPF.Drawing.AffineTransformations
{
    public interface IAffineTransformation
    {
        Matrix<double> GetTransformation();
    }
}