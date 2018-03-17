// Mainwindow.xaml.cs
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Threading;
using System.Windows.Controls.Primitives;
using UsnJournal;
using PInvoke;
using System.Diagnostics;

namespace UsnJournalProject
{
    /// <summary>
    /// Interaction logic for Mainwindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        private NtfsUsnJournal _usnJournal = null;
        public NtfsUsnJournal Journal
        {
            get { return _usnJournal; }
        }

        #endregion

        #region private member variables

        private Win32Api.USN_JOURNAL_DATA _usnCurrentJournalState;

        private UsnEntryDetail _usnEntryDetail;
        private double _lbItemX;
        private double _lbItemY;

        private UsnEntryDetail.EntryDetail _entryDetail;

        #endregion

        #region delegates

        private delegate void FillListBoxDelegate(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> usnEntries, Win32Api.USN_JOURNAL_DATA newUsnState);
        private delegate void FillListBoxWithFilesDelagate(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> fileList);
        private delegate void FillListBoxWithFoldersDelegate(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> folders);

        #endregion

        #region constructor(s)

        public MainWindow()
        {
            InitializeComponent();
            _usnCurrentJournalState = new Win32Api.USN_JOURNAL_DATA();
        }

        #endregion

        #region event handler functions

        private void SelectVolume_Click(object sender, RoutedEventArgs e)
        {
            _usnEntryDetail.Visibility = Visibility.Hidden;
            resultsLb.ItemsSource = null;
            resultsLb.Items.Clear();

            VolumeSelectDialog selectVolumeDlg = new VolumeSelectDialog(this);

            bool? rtn = selectVolumeDlg.ShowDialog();
            if (rtn != null && rtn.Value)
            {
                DriveInfo volume = selectVolumeDlg.Volume;
                try
                {
                    _usnJournal = new NtfsUsnJournal(volume);
                    FunctionElapsedTime.Content = string.Format("{0} elapsed time {1}(ms) - Volume: {2}",
                        "NtfsUsnJournal() constructor", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString(), volume.Name);
                    QueryUsnJournal.IsEnabled = true;
                    CreateUsnJournal.IsEnabled = true;
                    DeleteUsnJournal.IsEnabled = true;
                    SaveUsnState.IsEnabled = true;
                    ViewUsnChanges.IsEnabled = true;
                    ListFiles.IsEnabled = true;
                    ListFolders.IsEnabled = true;
                }
                catch (Exception excptn)
                {
                    if (excptn.Message.Contains("Access is denied"))
                    {
                        ListBoxItem lbItem = new ListBoxItem();
                        lbItem.Content = string.Format("'Access Denied' exception caught attempting to select volume.  \nYou need 'Admin' rights to run this application.");
                        lbItem.Foreground = Brushes.Red;
                        resultsLb.Items.Add(lbItem);
                    }
                    else
                    {
                        ListBoxItem lbItem = new ListBoxItem();
                        lbItem.Content = string.Format("{0} exception caught attempting to select volume. \n{1}", excptn.GetType().ToString(), excptn.Message);
                        lbItem.Foreground = Brushes.Red;
                        resultsLb.Items.Add(lbItem);
                    }
                }
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("Select Volume -- No Volume Selected");
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
        }

        private void QueryUsnJournal_Click(object sender, RoutedEventArgs e)
        {
            _usnEntryDetail.Visibility = Visibility.Hidden;
            resultsLb.ItemsSource = null;
            resultsLb.Items.Clear();
            Win32Api.USN_JOURNAL_DATA journalState = new Win32Api.USN_JOURNAL_DATA();
            NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal.GetUsnJournalState(ref journalState);

            FunctionElapsedTime.Content = string.Format("Query->{0} elapsed time {1}(ms)",
                "GetUsnJournalState()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString());

            if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Foreground = Brushes.Black;
                lbItem.Content = FormatUsnJournalState(journalState);
                resultsLb.Items.Add(lbItem);
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("Query->{0} returned error code: {1}", "GetUsnJournalState()", rtn.ToString());
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
        }

        private void CreateUsnJournal_Click(object sender, RoutedEventArgs e)
        {
            _usnEntryDetail.Visibility = Visibility.Hidden;
            resultsLb.ItemsSource = null;
            resultsLb.Items.Clear();
            NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal.CreateUsnJournal(1000 * 1024, 16 * 1024);

            FunctionElapsedTime.Content = string.Format("Create->{0} elapsed time {1}(ms)",
                "CreateUsnJournal()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString());

            if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Foreground = Brushes.Black;
                lbItem.Content = string.Format("USN Journal Successfully created, CreateUsnJournal() returned: {0}", rtn.ToString());
                resultsLb.Items.Add(lbItem);
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("Create->{0} returned error code: {1}", "GetUsnJournalState()", rtn.ToString());
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
        }

        private void DeleteUsnJournal_Click(object sender, RoutedEventArgs e)
        {
            _usnEntryDetail.Visibility = Visibility.Hidden;
            resultsLb.ItemsSource = null;
            resultsLb.Items.Clear();
            NtfsUsnJournal.UsnJournalReturnCode rtn;
            if (_usnCurrentJournalState.UsnJournalID == 0)
            {
                Win32Api.USN_JOURNAL_DATA journalState = new Win32Api.USN_JOURNAL_DATA();
                rtn = _usnJournal.GetUsnJournalState(ref journalState);
                if (rtn != NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    lbItem.Content = string.Format("Delete->{0} returned error code: {1}", "GetUsnJournalState()", rtn.ToString());
                    lbItem.Foreground = Brushes.Red;
                    resultsLb.Items.Add(lbItem);
                    return;
                }
                else
                {
                    _usnCurrentJournalState = journalState;
                }
            }

            rtn = _usnJournal.DeleteUsnJournal(_usnCurrentJournalState);

            FunctionElapsedTime.Content = string.Format("Delete->{0} elapsed time {1}(ms)",
                "DeleteUsnJournal()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString());

            if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Foreground = Brushes.Black;
                lbItem.Content = string.Format("USN Journal successfully deleted, DeleteUsnJournal() returned: {0}", rtn.ToString()); ;
                resultsLb.Items.Add(lbItem);
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("Delete->{0} returned error code: {1}", "DeleteUsnJournal()", rtn.ToString());
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
        }

        private void SaveUsnState_Click(object sender, RoutedEventArgs e)
        {
            _usnEntryDetail.Visibility = Visibility.Hidden;
            resultsLb.ItemsSource = null;
            resultsLb.Items.Clear();
            Win32Api.USN_JOURNAL_DATA journalState = new Win32Api.USN_JOURNAL_DATA();
            NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal.GetUsnJournalState(ref journalState);

            FunctionElapsedTime.Content = string.Format("Save State->{0} elapsed time {1}(ms)",
                "GetUsnJournalState()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString());

            if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                _usnCurrentJournalState = journalState;
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Foreground = Brushes.Black;
                lbItem.Content = FormatUsnJournalState(journalState);
                resultsLb.Items.Add(lbItem);
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("Save State->{0} returned error code: {1}", "GetUsnJournalState()", rtn.ToString());
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
        }

        private void ViewUsnChanges_Click(object sender, RoutedEventArgs e)
        {
            if (_usnCurrentJournalState.UsnJournalID != 0)
            {
                _usnEntryDetail.Visibility = Visibility.Hidden;
                resultsLb.ItemsSource = null;
                resultsLb.Items.Clear();

                Thread usnJournalThread = new Thread(ViewChangesThreadStart);
                Cursor = Cursors.Wait;
                usnJournalThread.Start();
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("'View Changes'-> Must 'Save State' before calling 'View Changes'");
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
        }

        private void ViewChangesThreadStart()
        {
            uint reasonMask = Win32Api.USN_REASON_DATA_OVERWRITE |
                    Win32Api.USN_REASON_DATA_EXTEND |
                    Win32Api.USN_REASON_NAMED_DATA_OVERWRITE |
                    Win32Api.USN_REASON_NAMED_DATA_TRUNCATION |
                    Win32Api.USN_REASON_FILE_CREATE |
                    Win32Api.USN_REASON_FILE_DELETE |
                    Win32Api.USN_REASON_EA_CHANGE |
                    Win32Api.USN_REASON_SECURITY_CHANGE |
                    Win32Api.USN_REASON_RENAME_OLD_NAME |
                    Win32Api.USN_REASON_RENAME_NEW_NAME |
                    Win32Api.USN_REASON_INDEXABLE_CHANGE |
                    Win32Api.USN_REASON_BASIC_INFO_CHANGE |
                    Win32Api.USN_REASON_HARD_LINK_CHANGE |
                    Win32Api.USN_REASON_COMPRESSION_CHANGE |
                    Win32Api.USN_REASON_ENCRYPTION_CHANGE |
                    Win32Api.USN_REASON_OBJECT_ID_CHANGE |
                    Win32Api.USN_REASON_REPARSE_POINT_CHANGE |
                    Win32Api.USN_REASON_STREAM_CHANGE |
                    Win32Api.USN_REASON_CLOSE;

            Win32Api.USN_JOURNAL_DATA newUsnState;
            List<Win32Api.UsnEntry> usnEntries;
            NtfsUsnJournal.UsnJournalReturnCode rtnCode = _usnJournal.GetUsnJournalEntries(_usnCurrentJournalState, reasonMask, out usnEntries, out newUsnState);
            Dispatcher.Invoke(new FillListBoxDelegate(FillListBoxWithUsnEntries), rtnCode, usnEntries, newUsnState);
        }

        private void FillListBoxWithUsnEntries(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> usnEntries, Win32Api.USN_JOURNAL_DATA newUsnState)
        {
            FunctionElapsedTime.Content = string.Format("'View Changes'->{0} elapsed time {1}(ms)",
                "GetUsnJournalEntries()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString());

            if (rtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                if (usnEntries.Count > 0)
                {
                    _entryDetail = UsnEntryDetail.EntryDetail.UsnEntry;
                    resultsLb.ItemsSource = usnEntries;

                    UpdateUsnStateDialog updateUsnStateDlg = new UpdateUsnStateDialog(this);
                    updateUsnStateDlg.Owner = this;
                    bool? bRtn = updateUsnStateDlg.ShowDialog();
                    if (bRtn != null && bRtn.Value)
                    {
                        _usnCurrentJournalState = newUsnState;
                    }
                }
                else
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    lbItem.Content = string.Format("'View Changes'-> No Journal entries found");
                    lbItem.Foreground = Brushes.Red;
                    resultsLb.Items.Add(lbItem);
                }
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("'View Changes'->{0} returned error code: {1}", "GetUsnJournalEntries()", rtnCode.ToString());
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
            Cursor = Cursors.Arrow;
        }

        private void ListFiles_Click(object sender, RoutedEventArgs e)
        {
            EnterFileFilterString filterStringDlg = new EnterFileFilterString(this);
            bool? bRtn = filterStringDlg.ShowDialog();
            if (bRtn != null && bRtn.Value)
            {
                string fileFilter = filterStringDlg.FileFilter;
                if (!string.IsNullOrEmpty(fileFilter))
                {
                    _usnEntryDetail.Visibility = Visibility.Hidden;
                    resultsLb.ItemsSource = null;
                    resultsLb.Items.Clear();

                    Thread usnJournalThread = new Thread(ListFilesThreadStart);
                    Cursor = Cursors.Wait;
                    usnJournalThread.Start(fileFilter);
                }
                else
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    lbItem.Content = string.Format("'List Files'-> File Filter is Null or Empty");
                    lbItem.Foreground = Brushes.Red;
                    resultsLb.Items.Add(lbItem);
                }
            }
        }

        private void ListFilesThreadStart(object fileFilterObj)
        {
            string fileFilter = (string)fileFilterObj;
            List<Win32Api.UsnEntry> fileList;
            NtfsUsnJournal.UsnJournalReturnCode rtnCode = _usnJournal.GetFilesMatchingFilter(fileFilter, out fileList);
            Dispatcher.Invoke(new FillListBoxWithFilesDelagate(FillListBoxWithFiles), rtnCode, fileList);
        }

        private void FillListBoxWithFiles(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> fileList)
        {
            FunctionElapsedTime.Content = string.Format("'List Files'->{0} elapsed time {1}(ms) {2} files",
                "GetFilesMatchingFilter()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString(), fileList.Count);

            if (rtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                if (fileList.Count > 0)
                {
                    _entryDetail = UsnEntryDetail.EntryDetail.File;
                    resultsLb.ItemsSource = fileList;
                }
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("'List Files'->{0} returned error code: {1}", "GetFilesMatchingFilter()", rtnCode.ToString());
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
            Cursor = Cursors.Arrow;
        }

        private void ListFolders_Click(object sender, RoutedEventArgs e)
        {
            _usnEntryDetail.Visibility = Visibility.Hidden;
            resultsLb.ItemsSource = null;
            resultsLb.Items.Clear();

            Thread usnJournalThread = new Thread(ListFoldersThreadStart);
            Cursor = Cursors.Wait;
            usnJournalThread.Start();
        }

        private void ListFoldersThreadStart(object fileFilterObj)
        {
            List<Win32Api.UsnEntry> folders;
            NtfsUsnJournal.UsnJournalReturnCode rtnCode = _usnJournal.GetNtfsVolumeFolders(out folders);
            Dispatcher.Invoke(new FillListBoxWithFoldersDelegate(FillListBoxWithFolders), rtnCode, folders);
        }

        private void FillListBoxWithFolders(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> folders)
        {
            FunctionElapsedTime.Content = string.Format("'List Folders'->{0} elapsed time {1}(ms) {2} folders",
                "GetNtfsVolumeFolders()", NtfsUsnJournal.ElapsedTime.Milliseconds.ToString(), folders.Count);

            if (rtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                if (folders.Count > 0)
                {
                    _entryDetail = UsnEntryDetail.EntryDetail.File;
                    resultsLb.ItemsSource = folders;
                }
            }
            else
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.Content = string.Format("'List Folders'->{0} returned error code: {1}", "GetNtfsVolumeFolders()", rtnCode.ToString());
                lbItem.Foreground = Brushes.Red;
                resultsLb.Items.Add(lbItem);
            }
            Cursor = Cursors.Arrow;
        }

        private void resultsLb_Selectionchanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedItem != null)
            {
                if (lb.SelectedItem.GetType() == typeof(Win32Api.UsnEntry))
                {
                    Win32Api.UsnEntry item = (Win32Api.UsnEntry)lb.SelectedItem;
                    _usnEntryDetail.ChangeDisplay(_lbItemY, _lbItemX, item, _entryDetail);
                }
            }
        }

        private void resultsLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedItem != null)
            {
                if (lb.SelectedItem.GetType() == typeof(Win32Api.UsnEntry))
                {
                    Win32Api.UsnEntry usnEntry = (Win32Api.UsnEntry)lb.SelectedItem;
                    StringBuilder sb = new StringBuilder();
                    string path;
                    NtfsUsnJournal.UsnJournalReturnCode usnRtnCode = _usnJournal.GetPathFromFileReference(usnEntry.ParentFileReferenceNumber, out path);
                    if (usnRtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS && 0 != string.Compare(path, "Unavailable", true))
                    {
                        if (usnEntry.IsFile)
                        {
                            string fullPath = System.IO.Path.Combine(path, usnEntry.Name);
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
                    }
                }
            }
        }

        private void resultsLb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            Point pt = resultsDock.PointToScreen(new Point(resultsDock.ActualWidth, mousePosition.Y));
            _lbItemX = pt.X;
            _lbItemY = pt.Y;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _usnEntryDetail = new UsnEntryDetail(this);
        }

        #endregion

        #region Formating functions

        private string FormatUsnJournalState(Win32Api.USN_JOURNAL_DATA _usnCurrentJournalState)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Journal ID: {0}", _usnCurrentJournalState.UsnJournalID.ToString("X")));
            sb.AppendLine(string.Format(" First USN: {0}", _usnCurrentJournalState.FirstUsn.ToString("X")));
            sb.AppendLine(string.Format("  Next USN: {0}", _usnCurrentJournalState.NextUsn.ToString("X")));
            sb.AppendLine();
            sb.AppendLine(string.Format("Lowest Valid USN: {0}", _usnCurrentJournalState.LowestValidUsn.ToString("X")));
            sb.AppendLine(string.Format("         Max USN: {0}", _usnCurrentJournalState.MaxUsn.ToString("X")));
            sb.AppendLine(string.Format("        Max Size: {0}", _usnCurrentJournalState.MaximumSize.ToString("X")));
            sb.AppendLine(string.Format("Allocation Delta: {0}", _usnCurrentJournalState.AllocationDelta.ToString("X")));
            return sb.ToString();
        }

        #endregion
    }
}