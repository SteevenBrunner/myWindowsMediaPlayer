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
    /// Logique d'interaction pour SelectDialog.xaml
    /// </summary>
    public partial class SelectDialog : Window
    {

        public SelectDialog(string title, string label, List<string> items)
        {
            InitializeComponent();

            ListValues = items;

            this.Title = title;
            this.Label.Text = label + " :";
            this.Items.ItemsSource = ListValues;
        }

        public List<string> ListValues
        {
            get;
        }

        public string Input
        {
            get; set;
        }


        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            Input = ((Button)sender).Tag as string;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
