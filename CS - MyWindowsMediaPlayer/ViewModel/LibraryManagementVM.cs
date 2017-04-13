using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace MyWindowsMediaPlayer.ViewModel
{
    class LibraryManagementVM : ViewModelBase
    {
        #region Attributes
        private ICommand _addFolderCommand = null;
        private ICommand _removeFolderCommand = null;

        private Library _library = null;
        #endregion

        #region Properties
        public Library Library
        {
            get { return (_library); }
        }
        public ICollectionView Folders
        {
            get { return (new ListCollectionView(_library.Folders)); }
        }

        public ICommand AddFolderCommand
        {
            get
            {
                if (_addFolderCommand == null)
                    _addFolderCommand = new DelegateCommand(OnAddFolder, CanAddFolder);

                return (_addFolderCommand);
            }
        }
        public ICommand RemoveFolderCommand
        {
            get
            {
                if (_removeFolderCommand == null)
                    _removeFolderCommand = new DelegateCommand(OnRemoveFolder, CanRemoveFolder);

                return (_removeFolderCommand);
            }
        }
        #endregion

        #region Commands
        public void OnAddFolder(object arg)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.SelectedPath))
            {
                _library.AddFolder(new Uri(dialog.SelectedPath));
                NotifyPropertyChanged("Folders");
                NotifyPropertyChanged("Library");
            }
        }

        public bool CanAddFolder(object arg)
        {
            return (true);
        }

        public void OnRemoveFolder(object arg)
        {
            string folder = ((Uri)arg).ToString();

            _library.RemoveFolder(new Uri(folder));
            NotifyPropertyChanged("Folders");
            NotifyPropertyChanged("Library");
        }

        public bool CanRemoveFolder(object arg)
        {
            return (true);
        }
        #endregion

        #region Ctor / Dtor
        public LibraryManagementVM(Library library)
        {
            _library = library;
        }
        #endregion
    }
}
