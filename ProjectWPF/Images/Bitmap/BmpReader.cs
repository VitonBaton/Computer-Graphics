using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWPF.Images.Bitmap.BitmapInfo;
using ProjectWPF.Images.Bitmap.Utils;
using ProjectWPF.Images.Utils;

namespace ProjectWPF.Images.Bitmap
{
    public static class BmpReader
    {
        public static Image Read(string filename)
        {
            var result = new Image();
            using (var filestream = File.OpenRead(filename))
            {
                var fileHeader = StructMarshallerExtension.ReadStruct<BitmapFileHeader>(filestream);

                if (fileHeader.bfType != 0x4D42)
                {
                    throw new ArgumentException("File is not bmp image");
                }
                
                var infoHeader = StructMarshallerExtension.ReadStruct<BitmapInfoHeader>(filestream);

                if (infoHeader.biBitCount != 24)
                {
                    throw new ArgumentException("Cannot read this bit format");
                }

                filestream.Seek(fileHeader.bfOffBits, SeekOrigin.Begin);

                //var bitmap = new WriteableBitmap(infoHeader.biWidth,infoHeader.biHeight, 96, 96, PixelFormats.Bgr24, null);
                //var builder = new BitmapBuilder(infoHeader.biWidth, infoHeader.biHeight, PixelFormats.Bgr24);

                // for (var i = 0; i < infoHeader.biHeight; i++)
                // {
                //     for (var j = 0; j < infoHeader.biWidth; j++)
                //     {
                //         var color = StructMarshaller.ReadStruct<RgbColor>(filestream);
                //         builder.SetPixel(color.R, color.G, color.B, j, infoHeader.biHeight - i);
                //     }
                //
                //     filestream.Seek(infoHeader.biWidth % 4, SeekOrigin.Current);
                // }
                var bitmap = BitmapFactory.New(infoHeader.biWidth, infoHeader.biHeight);
                using (bitmap.GetBitmapContext())
                {
                    var index = infoHeader.biWidth*infoHeader.biHeight - infoHeader.biWidth;
                    for (var i = 0; i < infoHeader.biHeight; i++)
                    {
                        for (var j = 0; j < infoHeader.biWidth; j++)
                        {
                            var color = filestream.ReadStruct<RgbColor>();
                            bitmap.SetPixeli(index++, color.R, color.G, color.B);
                        }
                        index -= infoHeader.biWidth * 2;
                        filestream.Seek(infoHeader.biWidth % 4, SeekOrigin.Current);
                    }
                }

                result.Source = bitmap;
                result.Width = infoHeader.biWidth;
                result.Height = infoHeader.biHeight;
            }

            return result;
        }
    }
}