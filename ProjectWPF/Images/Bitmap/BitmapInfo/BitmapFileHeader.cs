using System.Runtime.InteropServices;

namespace ProjectWPF.Images.Bitmap.BitmapInfo
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BitmapFileHeader
    {
        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }
}