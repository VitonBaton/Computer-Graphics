using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using ProjectWPF.Drawing;
using Color = System.Windows.Media.Color;

namespace ProjectWPF
{
    public partial class ColorConversion : UserControl
    {
        private bool _isChangedByMe;
        
        public ColorConversion()
        {
            InitializeComponent();
        }

        private void Hsb_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_isChangedByMe)
            {
                _isChangedByMe = false;
                return;
            }
            if (Hsb1?.Value == null || Hsb2?.Value == null || Hsb3?.Value == null) return;
            var (r,g,b) = ColorsConverter.HsvToRgb(Hsb1.Value.Value, Hsb2.Value.Value, Hsb3.Value.Value);
            if (ClrView != null)
            {
                ClrView.SelectedColor = Color.FromRgb(r, g, b);
            }

            var (x, y, z) = ColorsConverter.RgbToXyz(r, g, b);  
            
            if (Xyz1?.Value == null || Xyz2?.Value == null || Xyz3?.Value == null) return;
            _isChangedByMe = true;
            Xyz1.Value = x;
            _isChangedByMe = true;
            Xyz2.Value = y;
            _isChangedByMe = true;
            Xyz3.Value = z;
        }

        private void Xyz_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_isChangedByMe)
            {
                _isChangedByMe = false;
                return;
            }
            if (Xyz1?.Value == null || Xyz2?.Value == null || Xyz3?.Value == null) return;
            var (r,g,b) = ColorsConverter.XyzToRgb(Xyz1.Value.Value, Xyz2.Value.Value, Xyz3.Value.Value);
            if (ClrView != null)
            {
                ClrView.SelectedColor = Color.FromRgb(r, g, b);
            }

            var (h, s, v) = ColorsConverter.RgbToHsv(r, g, b);
            if (Hsb1?.Value == null || Hsb2?.Value == null || Hsb3?.Value == null) return;
            
            _isChangedByMe = true;
            Hsb1.Value = h;
            _isChangedByMe = true;
            Hsb2.Value = s;
            _isChangedByMe = true;
            Hsb3.Value = v;
        }
    }
}