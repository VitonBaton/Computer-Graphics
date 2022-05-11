namespace ProjectWPF.Drawing.RasterisationAlgorithms
{
    public static class RasterizationAlgorithms
    {
        public static readonly IRasterisationAlgorithm Wu = new WuAlgorithm();

        public static readonly IRasterisationAlgorithm Bresenham = new BresenhamAlgorithm();
    }
}