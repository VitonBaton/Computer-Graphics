using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWPF.Drawing.Primitives;
using ProjectWPF.Drawing.RasterisationAlgorithms;
using Point = ProjectWPF.Drawing.Primitives.Point;

namespace ProjectWPF.Drawing
{
    public class GraphicDrawer
    {
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public double Shift { get; set; }
        
        public int Coeff { get; set; }

        public void Draw(WriteableBitmap bitmap, IRasterisationAlgorithm drawAlg)
        {
            short h;
            byte s = 100;
            byte v = 100;
            
            //var points1 = new List<(int X, int Y)>();
            //var points2 = new List<(int X, int Y)>();
            var lengths1 = new List<double>();
            var lengths2 = new List<double>();
            
            var center = new
            {
                X = (int)bitmap.Width / 2,
                Y = (int)bitmap.Height / 2
            };

            bitmap.DrawLineP(new Line
                (
                    new Point(0, center.Y),
                    new Point((int) bitmap.Width - 1, center.Y)
                ),
                Colors.Black,
                drawAlg);
            
            bitmap.DrawLineP(new Line(
                    new Point(center.X, 0),
                    new Point(center.X, (int) bitmap.Height - 1)
                ),
                Colors.Black,
                drawAlg);
            
            // double F1(double angle) => (1 + Math.Sin(Math.Sqrt(angle))) / 3;
            // double F2(double angle) => 1;
            
            // double F1(double angle) => (1 + Math.Abs(Math.Cos(3* angle))) / 5;
            // double F2(double angle) => (1 + Math.Sin(2*Math.Cos(3* angle))*Math.Sin(2*Math.Cos(3* angle))) / 2;

            double F1(double angle) => (2 - Math.Abs(Math.Cos(4*angle))) / 4;
            double F2(double angle) => (2 - Math.Sin(2 * Math.Cos(4 * angle)) * Math.Sin(2 * Math.Cos(4 * angle))) / 2;
            
            for (var angle = StartAngle; angle < EndAngle; angle += Shift)
            {
                var radian = AngleToRadian(angle);
                
                var radius1 = F1(radian);
                var radius2 = F2(radian);
                
                var first = ConvertPolarToCartesian(F1(radian), radian);
                var second = ConvertPolarToCartesian(F2(radian), radian);

                lengths1.Add(radius1);
                lengths2.Add(radius2);
                
                h = (short)Math.Round(angle);
                var (r, g, b) = ColorsConverter.HsvToRgb(h, s, v);
                var color = Color.FromRgb(r, g, b);
                bitmap.DrawLineP(
                    new Line(
                        new Point(center.X + first.X, center.Y - first.Y),
                        new Point(center.X + second.X, center.Y - second.Y)
                    ),
                    color,
                    drawAlg);
            }

            var r1 = (int) Math.Round(Coeff * lengths1.Min());
            var r2 = (int) Math.Round(Coeff * lengths2.Max());
            
            bitmap.DrawCircleP(center.X,center.Y,r1,Colors.Black);
            bitmap.DrawCircleP(center.X,center.Y,r2,Colors.Black);
        }

        private double AngleToRadian(double angle)
        {
            return angle * Math.PI / 180;
        }
        
        private (int X, int Y) ConvertPolarToCartesian(double r, double angle)
        {
            return ((int)Math.Round(Coeff * r * Math.Cos(angle)), (int)Math.Round(Coeff * r * Math.Sin(angle)));
        }
    }
}