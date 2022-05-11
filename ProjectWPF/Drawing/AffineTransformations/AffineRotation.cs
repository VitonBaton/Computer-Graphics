using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ProjectWPF.Drawing.AffineTransformations
{
    public class AffineRotation : IAffineTransformation
    {
        private readonly double _angle;

        public AffineRotation(double angle)
        {
            this._angle = angle * Math.PI / 180;
        }
        public Matrix<double> GetTransformation()
        {
            return Matrix.Build.DenseOfArray(new[,]
            {
                {Math.Cos(_angle), Math.Sin(_angle), 0},
                {-Math.Sin(_angle), Math.Cos(_angle), 0},
                {0, 0, 1}
            }); 
        }
    }
}