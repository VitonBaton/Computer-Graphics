namespace ProjectWPF.Drawing.AffineTransformations
{
    public static class AffineTransformations
    {
        public static IAffineTransformation Dilatation(double a, double b)
        {
            return new AffineDilatation(a, b);
        }
        
        public static IAffineTransformation Translation(double v1, double v2)
        {
            return new AffineTranslation(v1, v2);
        }
        
        public static IAffineTransformation Rotation(double angle)
        {
            return new AffineRotation(angle);
        }
    }
}