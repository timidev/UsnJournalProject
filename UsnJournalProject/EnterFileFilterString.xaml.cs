// EnterFileFilterString.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UsnJournalProject
{
    /// <summary>
    /// Interaction logic for EnterFileFilterString.xaml
    /// </summary>
    public partial class EnterFileFilterString : Window
    {
        private string _filter = string.Empty;
        public string FileFilter
        {
            get { return _filter; }
        }

        public EnterFileFilterString(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void _cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void _ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _filter = _fileFilterTb.Text;
        }

        private void _fileFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (0 == string.Compare(tb.Text, "*"))
            {
                tb.Text = string.Empty;
            }
        }
    }
}