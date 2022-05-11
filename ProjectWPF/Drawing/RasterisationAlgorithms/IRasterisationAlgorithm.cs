using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWPF.Drawing.Primitives;

namespace ProjectWPF.Drawing.RasterisationAlgorithms
{
    public interface IRasterisationAlgorithm
    {
        void DrawLine(WriteableBitmap bitmap, Line line, Color color);
    }
}