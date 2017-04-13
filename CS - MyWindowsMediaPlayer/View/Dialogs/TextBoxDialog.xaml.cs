using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyWindowsMediaPlayer.View.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour TextBoxDialog.xaml
    /// </summary>
    public partial class TextBoxDialog : Window
    {
        public TextBoxDialog(string title, string label)
        {
            InitializeComponent();

            this.Title = title;
            this.Label.Text = label + " :";
        }

        public string Input
        {
            get { return InputTextBox.Text; }
            set { InputTextBox.Text = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
