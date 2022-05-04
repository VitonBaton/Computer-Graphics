using System;
using System.IO;
using System.Windows.Media.Imaging;
using ProjectWPF.Images.Bitmap.BitmapInfo;
using ProjectWPF.Images.Bitmap.Utils;
using ProjectWPF.Images.Utils;

namespace ProjectWPF.Images.Bitmap
{
    public class BmpWriter
    {
        private readonly WriteableBitmap _bitmap;

        private readonly int _width;
        private readonly int _height;
        
        public BmpWriter(BitmapSource source)
        {
            _bitmap = BitmapFactory.ConvertToPbgra32Format(source);
            _width = (int) _bitmap.Width;
            _height = (int) _bitmap.Height;
        }

        public void Write(string filename)
        {
            using (var filestream = File.Create(filename))
            {
                var fileHeader = GenerateFileHeader();
                var infoHeader = GenerateInfoHeader();

                filestream.WriteStruct(fileHeader);
                filestream.WriteStruct(infoHeader);
                
                var writeCount = _width % 4;
                var trash = new byte[writeCount];
                
                filestream.Seek(fileHeader.bfOffBits, SeekOrigin.Begin);
                for (var i = _height - 1; i >= 0; i--)
                {
                    for (var j = 0; j < _width; j++)
                    {
                        var color = _bitmap.GetPixel(j, i);
                        var rgb = new RgbColor
                        {
                            R = color.R,
                            G = color.G,
                            B = color.B
                        };
                        filestream.WriteStruct(rgb);
                    }

                    if (i == 2)
                    {
                        Console.WriteLine(0);
                    }
                    
                    filestream.Write(trash, 0, writeCount);
                }
            }
        }

        private BitmapInfoHeader GenerateInfoHeader()
        {
            var infoHeader = new BitmapInfoHeader
            {
                biWidth = _width,
                biHeight = _height,
                biPlanes = 1,
                biBitCount = 24,
                biCompression = 0,
                biSizeImage = 3 * _width * _height +
                              _height * (_width % 4),
                biXPelsPerMeter = 0,
                biYPelsPerMeter = 0,
                biClrUsed = 0xff0000,
                biClrImportant = 0
            };
            unsafe
            {
                infoHeader.biSize = sizeof(BitmapInfoHeader) + 76;
            }

            return infoHeader;
        }

        private BitmapFileHeader GenerateFileHeader()
        {
            var fileHeader = new BitmapFileHeader
            {
                bfType = 0x4D42,
                bfReserved1 = 0,
                bfReserved2 = 0
            };
            unsafe
            {
                fileHeader.bfOffBits = sizeof(BitmapFileHeader) + sizeof(BitmapInfoHeader) + 76;
            }

            fileHeader.bfSize = fileHeader.bfOffBits + 3 * _width * _height +
                                _height * (_width % 4);
            return fileHeader;
        }
    }
}