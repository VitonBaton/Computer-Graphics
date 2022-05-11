using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ProjectWPF.Drawing.AffineTransformations
{
    public class AffineTranslation : IAffineTransformation
    {
        private readonly double _v1;
        private readonly double _v2;

        public AffineTranslation(double v1, double v2)
        {
            this._v1 = v1;
            this._v2 = v2;
        }
        public Matrix<double> GetTransformation()
        {
            return Matrix.Build.DenseOfArray(new[,]
            {
                {1, 0, 0},
                {0, 1, 0},
                {_v1, _v2, 1}
            });
        }
    }
}