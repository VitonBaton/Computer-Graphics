using System;
using System.Windows;

namespace ProjectWPF
{
    public partial class GraphicParamsDialog : Window
    {
        public short SelectedStartAngle
        {
            get
            {
                if (!StartAngle.Value.HasValue)
                {
                    throw new ArgumentNullException();
                }
                return StartAngle.Value.Value;
            }
        }
        
        public short SelectedEndAngle
        {
            get
            {
                if (!EndAngle.Value.HasValue)
                {
                    throw new ArgumentNullException();
                }
                return EndAngle.Value.Value;
            }
        }
        
        public double SelectedShift
        {
            get
            {
                if (!Shift.Value.HasValue)
                {
                    throw new ArgumentNullException();
                }
                return Shift.Value.Value;
            }
        }

        public GraphicParamsDialog()
        {
            InitializeComponent();
        }
        
        public GraphicParamsDialog(GraphicParamsDialog dialog) : this()
        {
            this.Shift.Value = dialog.Shift.Value;
            this.StartAngle.Value = dialog.StartAngle.Value;
            this.EndAngle.Value = dialog.EndAngle.Value;
        }
        
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void EndAngle_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((short)e.NewValue <= StartAngle.Value.Value)
            {
                EndAngle.Value = (short)e.OldValue;
            }
        }
    }
}