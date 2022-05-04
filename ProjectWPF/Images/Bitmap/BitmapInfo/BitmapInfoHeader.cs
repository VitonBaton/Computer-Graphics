using System.Runtime.InteropServices;

namespace ProjectWPF.Images.Bitmap.BitmapInfo
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BitmapInfoHeader
    {
        public int biSize;
        public int biWidth;
        public int biHeight; 
        public short biPlanes; 
        public short biBitCount; 
        public int biCompression; 
        public int biSizeImage; 
        public long biXPelsPerMeter; 
        public long biYPelsPerMeter; 
        public uint biClrUsed; 
        public int biClrImportant; 
    }
}