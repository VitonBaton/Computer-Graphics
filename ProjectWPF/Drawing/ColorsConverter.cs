using System;

namespace ProjectWPF.Drawing
{
    public static class ColorsConverter
    {
        public static (short H, byte S, byte V) RgbToHsv(byte R, byte G, byte B)
        {
            const double tolerance = 0.00001;
            
            var r = R / 255d;
            var g = G / 255d;
            var b = B / 255d;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            double h;

            switch (Math.Abs(max - r) < tolerance)
            {
                case true when G >= B:
                    h = 60 * (g - b) / (max - min) + 0;
                    break;
                case true when G < B:
                    h = 60 * (g - b) / (max - min) + 360;
                    break;
                default:
                {
                    if (Math.Abs(max - g) < tolerance)
                    {
                        h = 60 * (b - r) / (max - min) + 120;
                    }
                    else
                    {
                        h = 60 * (b - r) / (max - min) + 120;
                    }

                    break;
                }
            }

            var s = (Math.Abs(max) < tolerance) ? 0 : 1 - min / max;

            return ((short) Math.Round(h), (byte) Math.Round(s * 100), (byte) Math.Round(max * 100));
        }
        
        public static (byte R,byte G,byte B) HsvToRgb(short h, byte s, byte v)
        {
            var Hi = (h / 60) % 6;  // Hi = [H/60] mod 6

            var Vmin = (100 - s) * v / 100d;  // Vmin = ((100 - S)* V)/ 100

            var a = (v - Vmin) * (h % 60) / 60d;  // a = (V - Vmin) * (H mod 60)/ 60

            var Vinc = Vmin + a;
            var Vdec = v - a;

            double r, g, b;

            switch (Hi)
            {
                case 0:
                    r = v;
                    g = Vinc;
                    b = Vmin;
                    break;
                case 1:
                    r = Vdec;
                    g = v;
                    b = Vmin;
                    break;
                case 2:
                    r = Vmin;
                    g = v;
                    b = Vinc;
                    break;
                case 3:
                    r = Vmin;
                    g = Vdec;
                    b = v;
                    break;
                case 4:
                    r = Vinc;
                    g = Vmin;
                    b = v;
                    break;
                default:
                    r = v;
                    g = Vmin;
                    b = Vdec;
                    break;
            }

            return ((byte) Math.Round((r * 255 / 100)), (byte) Math.Round((g * 255 / 100)), (byte) Math.Round((b * 255 / 100)));
        }

        public static (byte X, byte Y, byte Z) RgbToXyz(byte R, byte G, byte B)
        {
            var doubleR = R / 255d;
            var doubleG = G / 255d;
            var doubleB = B / 255d;

            if (doubleR > 0.04045)
            {
                doubleR = Math.Pow((doubleR + 0.055) / 1.055, 2.4);
            }
            else
            {
                doubleR /= 12.92;
            }

            if (doubleG > 0.04045)
            {
                doubleG = Math.Pow((doubleG + 0.055) / 1.055, 2.4);
            }
            else
            {
                doubleG /= 12.92;
            }
            
            if (doubleB > 0.04045)
            {
                doubleB = Math.Pow((doubleB + 0.055) / 1.055, 2.4);
            }
            else
            {
                doubleB /= 12.92;
            }

            doubleR *= 100;
            doubleG *= 100;
            doubleB *= 100;

            var X = doubleR * 0.4124 + doubleG * 0.3576 + doubleB * 0.1805;
            var Y = doubleR * 0.2126 + doubleG * 0.7152 + doubleB * 0.0722;
            var Z = doubleR * 0.0193 + doubleG * 0.1192 + doubleB * 0.9505;

            return ((byte)Math.Round(X), (byte)Math.Round(Y), (byte)Math.Round(Z));
        }
        
        public static (byte R, byte G, byte B) XyzToRgb(byte X, byte Y, byte Z)
        {
            double adj(double C)
            {
                if (Math.Abs(C) < 0.0031308)
                {
                    return 12.92 * C;
                }

                return 1.055 * Math.Pow(C, 0.41666) - 0.055;
            }
            
            var doubleX = X / 100d;
            var doubleY = Y / 100d;
            var doubleZ = Z / 100d;

            var doubleR = 3.2404542 * doubleX - 1.5371385 * doubleY - 0.4985314 * doubleZ;
            var doubleG = -0.9692660 * doubleX + 1.8760108 * doubleY + 0.0415560 * doubleZ;
            var doubleB = 0.0556434 * doubleX - 0.2040259 * doubleY + 1.0572252 * doubleZ;

            var R = adj(doubleR) * 255;
            var G = adj(doubleG) * 255;
            var B = adj(doubleB) * 255;

            byte crop(double C)
            {
                if (C > 255)
                {
                    return 255;
                }

                return (byte)Math.Round(C);
            }
            
            
            return (crop(R),crop(G), crop(B));
        }
    }
}