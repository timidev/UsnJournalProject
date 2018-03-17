// UsnEntryDetail.xaml.cs
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
using PInvoke;
using UsnJournal;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace UsnJournalProject
{
    /// <summary>
    /// Interaction logic for UsnEntryDetail.xaml
    /// </summary>
    public partial class UsnEntryDetail : Window
    {
        public enum EntryDetail
        {
            Folder = 0,
            File = 1,
            UsnEntry = 2
        }

        public UsnEntryDetail(Window owner)
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            Owner = owner;
            Visibility = Visibility.Hidden;
        }

        public void ChangeDisplay(double top, double left, Win32Api.UsnEntry usnEntry, UsnEntryDetail.EntryDetail entryDetail)
        {
            Top = top;
            Left = left;

            MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
            NtfsUsnJournal usnJournal = mainWin.Journal;
            StringBuilder sb = new StringBuilder();
            if (usnEntry.IsFolder)
            {
                sb.AppendFormat("Directory: {0}", usnEntry.Name);
            }
            else if (usnEntry.IsFile)
            {
                sb.AppendFormat("File: {0}", usnEntry.Name);
            }
            _nameLbl.Content = sb.ToString();
            sb = new StringBuilder();
            string path;
            NtfsUsnJournal.UsnJournalReturnCode usnRtnCode = usnJournal.GetPathFromFileReference(usnEntry.ParentFileReferenceNumber, out path);
            if (usnRtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS && 0 != string.Compare(path, "Unavailable", true))
            {
                sb.AppendFormat("  Path:    {0}{1}\\", usnJournal.VolumeName.TrimEnd('\\'), path);
            }
            else
            {
                sb.AppendFormat("  Path:    {0}", path);
            }
            _pathLbl.Content = sb.ToString();
            sb = new StringBuilder();
            sb.AppendFormat("  File Ref No: {0}", usnEntry.FileReferenceNumber);
            sb.AppendFormat("\n  Parent FRN   {0}", usnEntry.ParentFileReferenceNumber);

            if (entryDetail == EntryDetail.UsnEntry)
            {
                sb.AppendFormat("\n  Length:  {0}", usnEntry.RecordLength);
                sb.AppendFormat("\n  USN:     {0}", usnEntry.USN);
                AddReasonData(sb, usnEntry);
            }
            if (usnEntry.IsFile)
            {
                string fullPath = System.IO.Path.Combine(path, usnEntry.Name);
                if (File.Exists(fullPath))
                {
                    FileInfo fi = new FileInfo(fullPath);
                    sb.AppendFormat("\n  File Length:   {0} (bytes)", fi.Length);
                    sb.AppendFormat("\n  Creation Time: {0} - {1}", fi.CreationTime.ToShortDateString(), fi.CreationTime.ToShortTimeString());
                    sb.AppendFormat("\n  Last Modify:   {0} - {1}", fi.LastWriteTime.ToShortDateString(), fi.LastWriteTime.ToShortTimeString());
                    sb.AppendFormat("\n  Last Access:   {0} - {1}", fi.LastAccessTime.ToShortDateString(), fi.LastAccessTime.ToShortTimeString());
                }
            }
            _entryDetailLbl.Content = sb.ToString();
            Visibility = Visibility.Visible;
        }

        private void AddReasonData(StringBuilder sb, Win32Api.UsnEntry usnEntry)
        {
            sb.AppendFormat("\n  Reason Codes:");
            uint value = usnEntry.Reason & Win32Api.USN_REASON_DATA_OVERWRITE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -DATA OVERWRITE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_DATA_EXTEND;
            if (0 != value)
            {
                sb.AppendFormat("\n     -DATA EXTEND");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_DATA_TRUNCATION;
            if (0 != value)
            {
                sb.AppendFormat("\n     -DATA TRUNCATION");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_NAMED_DATA_OVERWRITE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -NAMED DATA OVERWRITE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_NAMED_DATA_EXTEND;
            if (0 != value)
            {
                sb.AppendFormat("\n     -NAMED DATA EXTEND");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_NAMED_DATA_TRUNCATION;
            if (0 != value)
            {
                sb.AppendFormat("\n     -NAMED DATA TRUNCATION");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_FILE_CREATE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -FILE CREATE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_FILE_DELETE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -FILE DELETE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_EA_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -EA CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_SECURITY_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -SECURITY CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_RENAME_OLD_NAME;
            if (0 != value)
            {
                sb.AppendFormat("\n     -RENAME OLD NAME");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_RENAME_NEW_NAME;
            if (0 != value)
            {
                sb.AppendFormat("\n     -RENAME NEW NAME");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_INDEXABLE_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -INDEXABLE CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_BASIC_INFO_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -BASIC INFO CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_HARD_LINK_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -HARD LINK CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_COMPRESSION_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -COMPRESSION CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_ENCRYPTION_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -ENCRYPTION CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_OBJECT_ID_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -OBJECT ID CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_REPARSE_POINT_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -REPARSE POINT CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_STREAM_CHANGE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -STREAM CHANGE");
            }
            value = usnEntry.Reason & Win32Api.USN_REASON_CLOSE;
            if (0 != value)
            {
                sb.AppendFormat("\n     -CLOSE");
            }
        }

        private void _nameLbl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string name = (string)_nameLbl.Content;
            string path = (string)_pathLbl.Content;
            if (name.StartsWith("Directory: "))
            {
                name = name.Replace("File: ", "").Trim();
                MessageBox.Show(string.Format("Entry '{0}' is a 'Directory'", name));
            }
            else if (name.StartsWith("File: "))
            {
                name = name.Replace("File: ", "").Trim();
                path = path.Replace("  Path: ", "").Trim();
                if (!path.Contains("Unavailable"))
                {
                    string fullPath = System.IO.Path.Combine(path, name);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            Process.Start(fullPath);
                        }
                        catch (Exception excptn)
                        {
                            MessageBox.Show(excptn.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("File '{0}' not found", fullPath));
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("File '{0}' path unavailable", name));
                }
            }
        }
    }
}