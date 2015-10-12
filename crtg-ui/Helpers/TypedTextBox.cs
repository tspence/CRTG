using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRTG.UI.Helpers
{
    #region Typed text box with color changes and hints
    public class TypedTextBox : TextBox
    {
        private Type _valuetype;

        public TypedTextBox(Type t)
            : base()
        {
            _valuetype = t;
            this.TextChanged += new EventHandler(TypedTextBox_TextChanged);
            this.KeyDown += new KeyEventHandler(TypedTextBox_KeyDown);
        }

        void TypedTextBox_TextChanged(object sender, EventArgs e)
        {
            bool good = true;

            // If the underlying value of this class isn't a string, make sure the user types valid text
            if (_valuetype != typeof(string)) {
                try {
                    object o = Convert.ChangeType(this.Text.Trim(), _valuetype);
                } catch {
                    good = false;
                }
            }

            // Change the color to represent the status
            if (good) {
                if (Enabled) {
                    this.BackColor = SystemColors.Window;
                } else {
                    this.BackColor = TextBox.DefaultBackColor;
                }
            } else {
                this.BackColor = Color.FromArgb(251, 206, 177);
            }
        }


        /// <summary>
        /// Ensure that Ctrl+A works to select everything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TypedTextBox_KeyDown(object sender, KeyEventArgs k)
        {
            if (k.Control && k.KeyCode == Keys.A) {
                var tb = sender as TextBox;
                if (tb != null) {
                    tb.SelectAll();
                }
            }
        }
    }
    #endregion
}
