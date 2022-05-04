using System.Runtime.InteropServices;

namespace ProjectWPF.Images.Utils
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RgbColor
    {
        public byte B;
        public byte G;
        public byte R;
    }
}