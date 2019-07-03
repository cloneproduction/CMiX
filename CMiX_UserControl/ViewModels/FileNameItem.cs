﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using CMiX.Models;
using CMiX.Services;
using Memento;

namespace CMiX.ViewModels 
{
    public class FileNameItem : ViewModel
    {
        public FileNameItem(string messageaddress, ObservableCollection<OSCMessenger> oscmessengers, Mementor mementor)
            : base (oscmessengers, mementor)
        {
            MessageAddress = messageaddress + "Selected";
            Mementor = mementor;
        }

        private string _filename;
        public string FileName
        {
            get => _filename;
            set => SetAndNotify(ref _filename, value);
        }

        private bool _fileisselected;
        public bool FileIsSelected
        {
            get => _fileisselected;
            set
            {
                SetAndNotify(ref _fileisselected, value);
                if (FileIsSelected)
                {
                    SendMessages(MessageAddress, FileName);
                }
            }
        }

        public void UpdateMessageAddress(string messageaddress)
        {
            MessageAddress = messageaddress + "Selected";
        }

        #region COPY/PASTE
        public void Copy(FileNameItemModel filenameitemmodel)
        {
            filenameitemmodel.MessageAddress = MessageAddress;
            filenameitemmodel.FileIsSelected = FileIsSelected;
            filenameitemmodel.FileName = FileName;
        }

        public void Paste(FileNameItemModel filenameitemmodel)
        {

            DisabledMessages();

            MessageAddress = filenameitemmodel.MessageAddress;
            FileIsSelected = filenameitemmodel.FileIsSelected;
            FileName = filenameitemmodel.FileName;

            EnabledMessages();
        }
        #endregion
    }
}