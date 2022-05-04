using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace ProjectWPF
{
    /// <summary>
    /// Логика взаимодействия для MosaicData.xaml
    /// </summary>
    public partial class MosaicDialog : Window
    {
        public int SelectedBlockSize
        {
            get
            {
                var text = ((ComboBoxItem)selectedBlockSize.SelectedItem).Content.ToString();
                int result = 0;
                switch (text)
                {
                    case "2x2": result = 2;
                        break;
                    case "4x4": result = 4;
                        break;
                    case "8x8": result = 8;
                        break;
                }
                return result;
            }
        }

        public int SelectedWidth
        {
            get
            {
                if (widthValue.Value is null)
                {
                    throw new ArgumentNullException();
                }
                return widthValue.Value.Value;
            }
        }

        public int SelectedHeight
        {
            get
            {
                if (heightValue.Value is null)
                {
                    throw new ArgumentNullException();
                }
                return heightValue.Value.Value;
            }
        }

        public List<Color> SelectedColors
        {
            get
            {
                var result = new List<Color>();
                if (clrPcker_First.SelectedColor is null)
                {
                    throw new ArgumentNullException();
                }
                result.Add(clrPcker_First.SelectedColor.Value);

                if (clrPcker_Second.SelectedColor is null)
                {
                    throw new ArgumentNullException();
                }
                result.Add(clrPcker_Second.SelectedColor.Value);

                if (clrPcker_Third.SelectedColor is null)
                {
                    throw new ArgumentNullException();
                }
                result.Add(clrPcker_Third.SelectedColor.Value);

                if (clrPcker_Fourth.SelectedColor is null)
                {
                    throw new ArgumentNullException();
                }
                result.Add(clrPcker_Fourth.SelectedColor.Value);

                return result;
            }
        }

        public MosaicDialog()
        {
            InitializeComponent();
        }

        public MosaicDialog(MosaicDialog dialog) : this()
        {
            this.heightValue.Value = dialog.heightValue.Value;
            this.widthValue.Value = dialog.widthValue.Value;
            this.clrPcker_First.SelectedColor = dialog.clrPcker_First.SelectedColor;
            this.clrPcker_Second.SelectedColor = dialog.clrPcker_Second.SelectedColor;
            this.clrPcker_Third.SelectedColor = dialog.clrPcker_Third.SelectedColor;
            this.clrPcker_Fourth.SelectedColor = dialog.clrPcker_Fourth.SelectedColor;
            this.selectedBlockSize.SelectedIndex = dialog.selectedBlockSize.SelectedIndex;
        }

        private void TextBox_PreviewTextInput (object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            e.Handled = ValidationHelper.IsNumber(textBox.Text);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }

    public static class ValidationHelper
    {
        private static Regex numbers = new Regex("[^0 - 9.-] +");
        public static bool IsNumber(string text)
        { //regex that allows numeric input only
            return numbers.IsMatch(text); //
        }
    }
}
