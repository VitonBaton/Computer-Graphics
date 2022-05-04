using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectWPF.Encoding;

namespace ProjectWPF
{
    public partial class LzwCoding : UserControl
    {
        public LzwCoding()
        {
            InitializeComponent();
        }

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var originalText = OriginalText.Text;
                if (string.IsNullOrEmpty(originalText))
                {
                    throw new ArgumentNullException("Нечего кодировать");
                }

                var dictionary = LzwEncoder.GenerateStartDictionary(originalText).ToList();

                var code = LzwEncoder.Encode(originalText, dictionary);

                BaseDictionary.Text = dictionary.Aggregate((res, str) => res + " " + str);

                CodeText.Text = code;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = CodeText.Text;
                if (string.IsNullOrEmpty(code))
                {
                    throw new ArgumentNullException("Нечего раскодировать");
                }

                var dictionary = BaseDictionary.Text.Split(' ');

                if (string.IsNullOrEmpty(BaseDictionary.Text))
                {
                    throw new ArgumentNullException("Нет словаря");
                }

                var decodedText = LzwEncoder.Decode(code, dictionary);
                OriginalText.Text = decodedText;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}