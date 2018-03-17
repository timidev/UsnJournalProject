// UpdateUsnStateDialog.xaml.cs
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
    /// Interaction logic for UpdateUsnStateDialog.xaml
    /// </summary>
    public partial class UpdateUsnStateDialog : Window
    {
        public UpdateUsnStateDialog(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void _ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void _cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}