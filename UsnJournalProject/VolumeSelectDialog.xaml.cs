// VolumeSelectDialog.xaml.cs
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

using System.IO;
using System.Windows.Controls.Primitives;

namespace UsnJournalProject
{
    /// <summary>
    /// Interaction logic for VolumeSelectDialog.xaml
    /// </summary>
    public partial class VolumeSelectDialog : Window
    {
        private DriveInfo _driveInfo = null;
        public DriveInfo Volume
        {
            get { return _driveInfo; }
        }

        public VolumeSelectDialog(Window owner)
        {
            this.Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            DriveInfo[] volumes = DriveInfo.GetDrives();
            ListBoxItem lbItem;
            foreach (DriveInfo di in volumes)
            {
                if (di.IsReady && 0 == string.Compare(di.DriveFormat, "ntfs", true))
                {
                    lbItem = new ListBoxItem();
                    lbItem.Content = di.Name;
                    lbItem.Tag = di;
                    drivesLb.Items.Add(lbItem);
                }
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            HandleSelection();
        }

        private void drivesLb_Selectionchanged(object sender, SelectionChangedEventArgs e)
        {
            selectionerrorTb.Text = string.Empty;
        }

        private void drivesLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HandleSelection();
        }

        private void HandleSelection()
        {
            if (drivesLb.SelectedItem != null)
            {
                ListBoxItem lbItem = drivesLb.SelectedItem as ListBoxItem;
                _driveInfo = lbItem.Tag as DriveInfo;
                DialogResult = true;
            }
            else
            {
                selectionerrorTb.Text = "No volume selected";
            }
        }
    }
}