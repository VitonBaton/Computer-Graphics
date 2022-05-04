using System.Windows;
using ProjectWPF.Drawing.Primitives;

namespace ProjectWPF.Drawing.ClippingAlgorithms.LineClipping
{
    public interface ILineClippingAlgorithm
    {
        Line? Clip(Line line, Rect rect);
    }
}