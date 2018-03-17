using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PInvoke;

namespace UsnJournal
{
    [Serializable]
    public class FileNameAndParentFrn : IEquatable<FileNameAndParentFrn>
    {
        #region enum

        public enum FileSystemObjectType
        {
            DIRECTORY = 1,
            FILE = 2
        }

        #endregion

        #region Properties

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private UInt64 _frn;
        public UInt64 Frn
        {
            get { return _frn; }
        }

        private UInt64 _parentFrn;
        public UInt64 ParentFrn
        {
            get { return _parentFrn; }
            set { _parentFrn = value; }
        }

        private FileSystemObjectType _fsType;
        public FileSystemObjectType FileSystemType
        {
            get { return _fsType; }
        }

        #endregion

        #region Constructor(s)
        /*
        public FileNameAndParentFrn(string name, UInt64 frn, UInt64 parentFrn)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _name = name;
            }
            else
            {
                throw new ArgumentException("Invalid argument: null or Length = zero", "name");
            }
            _frn = frn;
            _parentFrn = parentFrn;
        }   // end FileNameAndParentFrn() closing bracket
        */

        public FileNameAndParentFrn(Win32Api.UsnEntry usnEntry)
        {
            if (!string.IsNullOrEmpty(usnEntry.Name))
            {
                _name = usnEntry.Name;
            }
            else
            {
                throw new ArgumentException("Invalid argument: null or Length = zero", "name");
            }
            _frn = usnEntry.FileReferenceNumber;
            _parentFrn = usnEntry.ParentFileReferenceNumber;

        }   // end FileNameAndParentFrn() closing bracket

        #endregion

        #region IEquatable<FileNameAndParentFrn> Members

        public bool Equals(FileNameAndParentFrn other)
        {
            bool retval = false;
            if ((object)other != null)
            {
                if (_frn == other._frn)
                    retval = true;
            }
            return retval;
        }

        public override bool Equals(object obj)
        {
            bool retval = false;
            if (obj == null) return false;

            if (obj is FileNameAndParentFrn)
            {
                retval = Equals((FileNameAndParentFrn)obj);
            }
            else
            {
                throw new InvalidCastException("Argument is not a 'FileNameAndParentFrn' type object");
            }
            return retval;
        }

        public override int GetHashCode()
        {
            return this._frn.GetHashCode();
        }

        public static bool operator ==(FileNameAndParentFrn fnapfrn1, FileNameAndParentFrn fnapfrn2)
        {
            return fnapfrn1.Equals(fnapfrn2);
        }

        public static bool operator !=(FileNameAndParentFrn fnapfrn1, FileNameAndParentFrn fnapfrn2)
        {
            return (!fnapfrn1.Equals(fnapfrn2));
        }

        #endregion
    }
}
