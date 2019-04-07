﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Collections.Specialized;
using System.ComponentModel;
using CMiX.Services;
using CMiX.Models;
using GongSolutions.Wpf.DragDrop;
using Memento;

namespace CMiX.ViewModels
{
    public class FileSelector : ViewModel, IDropTarget
    {
        #region CONSTRUCTORS
        public FileSelector(string layername, ObservableCollection<OSCMessenger> messengers, Mementor mementor)
            : this
            (
                filemask: new List<string>(),
                selectionmode: String.Empty,
                messageaddress: String.Format("{0}/", layername),
                messengers: messengers,
                mementor: mementor
            )
        { }

        public FileSelector
            (
                String selectionmode,
                List<string> filemask,
                ObservableCollection<OSCMessenger> messengers,
                string messageaddress,
                Mementor mementor
            )
            : base(messengers)
        {
            SelectionMode = selectionmode;
            FileMask = filemask;
            MessageAddress = messageaddress;
            Messengers = messengers ?? throw new ArgumentNullException(nameof(messengers));
            ClearSelectedCommand = new RelayCommand(p => ClearSelected());
            ClearUnselectedCommand = new RelayCommand(p => ClearUnselected());
            ClearAllCommand = new RelayCommand(p => ClearAll());
            DeleteItemCommand = new RelayCommand(p => DeleteItem(p));
            MouseDownCommand = new RelayCommand(p => MouseDown());
            MouseUpCommand = new RelayCommand(p => MouseUp());
            FilePaths = new ObservableCollection<FileNameItem>();
            FilePaths.CollectionChanged += ContentCollectionChanged;
            Mementor = mementor;
        }

        private void MouseDown()
        {
            if (!Mementor.IsInBatch) 
                Mementor.BeginBatch();
        }

        private void MouseUp()
        {
            if (Mementor.IsInBatch) 
               Mementor.EndBatch();
        }
        #endregion

        #region PROPERTIES
        [OSC]
        public ObservableCollection<FileNameItem> FilePaths { get; set; }

        public List<string> FileMask { get; set; }
        public string SelectionMode { get; set; }

        public ICommand ClearSelectedCommand { get; }
        public ICommand ClearUnselectedCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand MouseDownCommand { get; }
        public ICommand MouseUpCommand { get; }

        #endregion

        #region METHODS

        private void ClearAll()
        {
            Mementor.PropertyChange(this, "FilePaths");
            FilePaths.Clear();
        }

        private void ClearUnselected()
        {
            Mementor.Batch(() =>
            {
                for (int i = FilePaths.Count - 1; i >= 0; i--)
                {
                    if (!FilePaths[i].FileIsSelected)
                    {
                        Mementor.ElementRemove(FilePaths, FilePaths[i]);
                        FilePaths.Remove(FilePaths[i]);
                    }

                }
            });
        }

        private void ClearSelected()
        {
            Mementor.Batch(() =>
            {
                for (int i = FilePaths.Count - 1; i >= 0; i--)
                {
                    if (FilePaths[i].FileIsSelected)
                    {
                        Mementor.ElementRemove(FilePaths, FilePaths[i]);
                        FilePaths.Remove(FilePaths[i]);
                    }
                }
            });
        }

        private void DeleteItem(object filenameitem)
        {
            FileNameItem fni = filenameitem as FileNameItem;
            Mementor.ElementRemove(FilePaths, fni);
            FilePaths.Remove(fni);
        }
        #endregion

        #region DRAG/DROP

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            var dataObject = dropInfo.Data as IDataObject;
            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }

            if (dropInfo.Data.GetType() == typeof(FileNameItem))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;

            if (dataObject != null && dataObject.ContainsFileDropList())
            {

                var filedrop = dataObject.GetFileDropList();
                Mementor.Batch(() => {
                    foreach (string str in filedrop)
                    {
                        foreach (string fm in FileMask)
                        {
                            if (System.IO.Path.GetExtension(str).ToUpperInvariant() == fm)
                            {
                                FileNameItem lbfn = new FileNameItem(Mementor);
                                lbfn.FileName = str;
                                lbfn.FileIsSelected = false;
                                FilePaths.Add(lbfn);
                                Mementor.ElementAdd(FilePaths, lbfn);
                            }
                        }
                    }
                });

            }
        }
        #endregion

        #region COPY/PASTE
        public void Copy(FileSelectorDTO fileselectordto)
        {
            List<FileNameItemDTO> fileNameItemDTOs = new List<FileNameItemDTO>();
            foreach (var item in FilePaths)
            {
                var filenameitemdto = new FileNameItemDTO();
                filenameitemdto.FileIsSelected = item.FileIsSelected;
                filenameitemdto.FileName = item.FileName;
                fileNameItemDTOs.Add(filenameitemdto);
            }
            fileselectordto.FilePaths = fileNameItemDTOs;
        }

        public void Paste(FileSelectorDTO fileselectordto)
        {

            if(fileselectordto.FilePaths != null) // NOT SURE THIS IS USEFULL ...
            {
                DisabledMessages();
                Mementor.Batch(() =>
                {
                    FilePaths.Clear();
                    foreach (var item in fileselectordto.FilePaths)
                    {
                        var filenameitem = new FileNameItem(Mementor);
                        filenameitem.FileIsSelected = item.FileIsSelected;
                        filenameitem.FileName = item.FileName;
                        Mementor.ElementAdd(FilePaths, filenameitem);
                        FilePaths.Add(filenameitem);
                    }
                });
                EnabledMessages();
            }
        }
        #endregion

        #region COLLECTIONCHANGED
        public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (FileNameItem item in e.OldItems)
                {
                    item.PropertyChanged -= EntityViewModelPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FileNameItem item in e.NewItems)
                {
                    item.PropertyChanged += EntityViewModelPropertyChanged;
                }
            }

            List<string> filename = new List<string>();
            foreach (FileNameItem lb in FilePaths)
            {
                if (lb.FileIsSelected == true)
                {
                    filename.Add(lb.FileName);
                }
            }
            SendMessages(MessageAddress + nameof(FilePaths), filename.ToArray());
        }

        public void EntityViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            List<string> filename = new List<string>();
            foreach (FileNameItem lb in FilePaths)
            {
                if (lb.FileIsSelected == true)
                {
                    filename.Add(lb.FileName);
                }
            }
            SendMessages(MessageAddress + nameof(FilePaths), filename.ToArray());
        }
        #endregion
    }
}