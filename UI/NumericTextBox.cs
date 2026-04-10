using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TETMViewer.UI
{
    public partial class NumericTextBox : TextBox
    {
        public bool CheckIfEmpty { get; set; }
        public NumericType ValueType { get; set; } = NumericType.Double;
        public enum NumericType { Int, Double, }

        private string oldText = string.Empty;

        public NumericTextBox()
        {
            this.KeyPress += NumericTextBox_KeyPress;
            this.TextChanged += NumericTextBox_TextChanged;
        }

        private void NumericTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (CheckIfEmpty && string.IsNullOrEmpty(this.Text) || !TryValidate(this.Text))
            {
                this.Text = oldText;
                return;
            }

            oldText = this.Text;
        }

        private void NumericTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (ValueType == NumericType.Int && e.KeyChar == '.')
            {
                e.Handled = true;
                return;
            }

            e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == '.');
        }

        private bool TryValidate(string text)
        {
            switch (ValueType)
            {
                case NumericType.Int:
                    return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);

                case NumericType.Double:
                    return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out _);

                default:
                    return false;
            }
        }
    }
}
