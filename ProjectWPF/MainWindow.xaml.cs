using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjectWPF.Drawing;
using ProjectWPF.Drawing.Primitives;
using ProjectWPF.Images.Bitmap;
using Image = System.Windows.Controls.Image;
using Point = ProjectWPF.Drawing.Primitives.Point;

namespace ProjectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MosaicDialog _mosaicDialog = new MosaicDialog();

        private GraphicParamsDialog _paramsDialog = new GraphicParamsDialog();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void CanExecuteCloseCommand(object sender,
            CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecutedCloseCommand(object sender,
            ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Mosaic_Click(object sender, RoutedEventArgs e)
        {
            _mosaicDialog = new MosaicDialog(_mosaicDialog);
            if (_mosaicDialog.ShowDialog() == true)
            {
                var width = _mosaicDialog.SelectedWidth;
                var height = _mosaicDialog.SelectedHeight;
                Canvas.Height = height;
                Canvas.Width = width;
                var size = _mosaicDialog.SelectedBlockSize;
                var colors = _mosaicDialog.SelectedColors;
                var rectanglesLists = new List<GeometryGroup>();
                var geometryDrawings = new List<GeometryDrawing>();

                foreach (var color in colors)
                {
                    var rectangles = new GeometryGroup();
                    rectanglesLists.Add(rectangles);
                }

                var random = new Random();

                var brush = new DrawingBrush();

                var drawingGroup = new DrawingGroup();

                for (var i = 0; i < Canvas.Width; i += size)
                {
                    for (var j = 0; j < Canvas.Height; j += size)
                    {
                        var index = random.Next(4);
                        var rectangleGeometry = GeneratePart(size, size, i, j);
                        rectanglesLists[index].Children.Add(rectangleGeometry);
                    }
                }

                for (var i = 0; i < rectanglesLists.Count; i++)
                {
                    var geometryDrawing = new GeometryDrawing();
                    geometryDrawing.Brush = new SolidColorBrush(colors[i]);
                    geometryDrawing.Geometry = rectanglesLists[i];
                    drawingGroup.Children.Add(geometryDrawing);
                }

                brush.Drawing = drawingGroup;

                var finalRect = new Rectangle() {Width = width, Height = height};

                var bitmapSource = RenderToBitmap(drawingGroup, finalRect.Width, finalRect.Height);

                var image = new Image
                {
                    Source = bitmapSource,
                    Width = finalRect.Width,
                    Height = finalRect.Height
                };
                Canvas.Children.Clear();
                Canvas.Children.Add(image);
            }

        }

        private void ColorConversion_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            var colorConversion = new ColorConversion();
            Canvas.Children.Add(colorConversion);
            Canvas.Width = 0;
        }

        private void BmpReading_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "BMP images (.bmp)|*.bmp"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;

            Canvas.Children.Clear();
            var image = BmpReader.Read(dlg.FileName);
            Canvas.Children.Add(image);
            Canvas.Width = image.Width;
            Canvas.Height = image.Height;
        }

        private void BmpSaving_Click(object sender, RoutedEventArgs e)
        {
            if (Canvas.Children.Count == 0 || Canvas.Width == 0 || double.IsNaN(Canvas.Width))
            {
                MessageBox.Show("There's nothing to save", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var render = new RenderTargetBitmap((int) Canvas.Width, (int) Canvas.Height, 96, 96, PixelFormats.Pbgra32);
            render.Render(Canvas.Children[0]);

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "BMP images (.bmp)|*.bmp"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;

            var writer = new BmpWriter(render);
            writer.Write(dlg.FileName);

            MessageBox.Show("File successfully saved");
        }

        private void LzwCoding_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            var lzwEncoder = new LzwCoding();
            Canvas.Children.Add(lzwEncoder);
            Canvas.Width = 0;
        }

        private RectangleGeometry GeneratePart(int width, int height, int marginLeft, int marginTop)
        {
            return new RectangleGeometry(new Rect(marginLeft, marginTop, width, height));
        }

        private BitmapSource RenderToBitmap(System.Windows.Media.Drawing drawing, double width, double height)
        {
            int actualWidth = (int) width;
            int actualHeight = (int) height;

            RenderTargetBitmap renderBitmap =
                new RenderTargetBitmap((int) width, (int) height, 96, 96, PixelFormats.Pbgra32);

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawDrawing(drawing);
            }

            renderBitmap.Render(visual);
            return renderBitmap;
        }

        private void Primitives_Click(object sender, RoutedEventArgs e)
        {
            _paramsDialog = new GraphicParamsDialog(_paramsDialog);

            if (!_paramsDialog.ShowDialog() == true)
            {
                return;
            }

            var bitmap = BitmapFactory.New(512, 512);
            var drawer = new GraphicDrawer
            {
                Coeff = 100,
                StartAngle = _paramsDialog.SelectedStartAngle,
                EndAngle = _paramsDialog.SelectedEndAngle,
                Shift = _paramsDialog.SelectedShift
            };

            drawer.Draw(bitmap);

            var image = new Image
            {
                Width = 512,
                Height = 512,
                Source = bitmap
            };

            Canvas.Children.Clear();
            Canvas.Children.Add(image);
            Canvas.Width = image.Width;
            Canvas.Height = image.Height;
        }

        private void Clipping_Click(object sender, RoutedEventArgs e)
        {
            var (width, height) = (1024, 1024);

            var bitmap = BitmapFactory.New(width, height);
            var image = new Image
            {
                Width = width,
                Height = height,
                Source = bitmap
            };
            Canvas.Children.Clear();
            Canvas.Children.Add(image);
            Canvas.Width = image.Width;
            Canvas.Height = image.Height;

            image.MouseMove += SegmentIntersection(image, bitmap);
        }

        private MouseEventHandler SegmentIntersection(IInputElement image, WriteableBitmap bitmap)
        {
            var rect = new Rect(30, 30, 200, 100);

            var line1 = new Line
            {
                P1 = new Point(10, 600),
                P2 = new Point(600, 10)
            };

            var line2 = new Line
            {
                P1 = new Point(400, 500),
                P2 = new Point(40, 20)
            };

            var line3 = new Line
            {
                P1 = new Point(30, 240),
                P2 = new Point(540, 540)
            };

            return (o, args) =>
            {
                var point = args.GetPosition(image);
                if ((rect.Width / 2 + point.X < bitmap.Width) && (rect.Height / 2 + point.Y < bitmap.Height))
                {
                    rect.X = point.X - rect.Width / 2;
                    rect.Y = point.Y - rect.Height / 2;
                    Console.WriteLine(rect.X);
                    Console.WriteLine(rect.Y);
                    Console.WriteLine();
                    bitmap.Clear();
                    bitmap.DrawRectangle((int) rect.X, (int) rect.Y, (int) rect.Right, (int) rect.Bottom, Colors.Black);
                    bitmap.DrawLineP(line1, Colors.Red, rect);
                    bitmap.DrawLineP(line2, Colors.Blue, rect);
                    bitmap.DrawLineP(line3, Colors.Green, rect);
                }
            };
        }

        private void ClippingPolygon_Click(object sender, RoutedEventArgs e)
        {
            var (width, height) = (1024, 1024);

            var bitmap = BitmapFactory.New(width, height);
            var image = new Image
            {
                Width = width,
                Height = height,
                Source = bitmap
            };
            Canvas.Children.Clear();
            Canvas.Children.Add(image);
            Canvas.Width = image.Width;
            Canvas.Height = image.Height;

            var clipPolygon = new Polygon(new(double X,double Y)[] {(20, 20), (150, 20), (150, 150), (20, 150)});
            var polygon = new Polygon(new(double X,double Y)[] {(10, 10), (50, 40), (200, 20), (50, 80)});

            (double x, double y) = (50, 50);

            clipPolygon.AddVector(20, 20);
            
            bitmap.DrawPolygon(polygon, Colors.Red, clipPolygon);
            bitmap.DrawPolygon(clipPolygon,Colors.Black);
            image.MouseMove += PolygonIntersection(image, bitmap);
        }

        private MouseEventHandler PolygonIntersection(IInputElement image, WriteableBitmap bitmap)
        {
            var clipPolygon = new Polygon(new(double X,double Y)[]
                {(82, 28), (177, 89), (82, 159), (92, 116), (0, 116), (0, 71), (92, 71)});
            var polygon = new Polygon(new (double, double)[] {(40, 40), (200, 160), (800, 80), (200, 320)});
            bitmap.DrawPolygon(polygon, Colors.Red, clipPolygon);

            (double x, double y) = (88, 88);
            
            polygon.AddVector(50,50);
            
            return (o, args) =>
            {
                var point = args.GetPosition(image);
                clipPolygon.AddVector(point.X - x, point.Y - y);
                (x, y) = (point.X, point.Y);
                
                bitmap.Clear();
                bitmap.DrawPolygon(clipPolygon, Colors.Black);
                bitmap.DrawPolygon(polygon, Colors.Red, clipPolygon);
            };
        }
    }
}