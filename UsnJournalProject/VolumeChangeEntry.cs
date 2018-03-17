using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace ConsoleUSNJournalProject
{
    public class UsnChangeEntry
    {
        #region enums

        public enum FolderOrFile
        {
            FOLDER = 0,
            FILE = 1
        }

        public enum NewChangedDeleted
        {
            NEW = 0,
            CHANGED = 1,
            DELETED = 2
        }

        #endregion

        #region properties

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
        }

        public string FullyQualifiedPath
        {
            get { return System.IO.Path.Combine(_path, _name); }
        }

        private FolderOrFile _fileType;
        public FolderOrFile FileType
        {
            get { return _fileType; }
        }

        private NewChangedDeleted _changeType;
        public NewChangedDeleted ChangeType
        {
            get { return _changeType; }
        }

        #endregion

        #region private member variables
        #endregion

        #region constructor(s)
        #endregion

        #region public member functions
        #endregion

        #region private member functions
        #endregion
    }
}
