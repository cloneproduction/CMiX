﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;
using CMiX.MVVM.Resources;

using GongSolutions.Wpf.DragDrop;
using Memento;


namespace CMiX.ViewModels
{
    public class FileSelector : ViewModel, IDropTarget, IDragSource
    {
        #region CONSTRUCTORS
        public FileSelector(string messageaddress, string selectionmode, List<string> filemask, ObservableCollection<OSCValidation> oscvalidation, Mementor mementor) 
            : base (oscvalidation, mementor)
        {
            MessageAddress = String.Format("{0}{1}/", messageaddress, nameof(FileSelector));

            SelectionMode = selectionmode;
            FileMask = filemask;
            FilePaths = new ObservableCollection<FileNameItem>();
            FolderPath = string.Empty;

            ClearSelectedCommand = new RelayCommand(p => ClearSelected());
            ClearUnselectedCommand = new RelayCommand(p => ClearUnselected());
            ClearAllCommand = new RelayCommand(p => ClearAll());
            DeleteItemCommand = new RelayCommand(p => DeleteItem(p));
        }
        #endregion

        #region PROPERTIES
        public ObservableCollection<FileNameItem> FilePaths { get; set; }

        public List<string> FileMask { get; set; }
        public string SelectionMode { get; set; }

        public ICommand ClearSelectedCommand { get; }
        public ICommand ClearUnselectedCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand DeleteItemCommand { get; }

        private FileNameItem selectedfilenameitem;
        public FileNameItem SelectedFileNameItem
        {
            get { return selectedfilenameitem; }
            set
            {
                SetAndNotify(ref selectedfilenameitem, value);
                if (Mementor != null)
                    Mementor.PropertyChange(this, nameof(SelectedFileNameItem));
            }
        }

        private string _folderpath;
        public string FolderPath
        {
            get => _folderpath;
            set
            {
                SetAndNotify(ref _folderpath, value);
                UpdateFileNameItemFolderName();
            }
        }
        #endregion

        #region METHODS
        public void UpdateMessageAddress(string messageaddress)
        {
            MessageAddress = messageaddress;
            foreach (var item in FilePaths)
            {
                item.UpdateMessageAddress(messageaddress);
            }
        }

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
        public void UpdateFileNameItemFolderName()
        {
            foreach (var filename in FilePaths)
            {
                filename.FolderPath = FolderPath;
            }
        }
        #region DRAG/DROP

        public void DragOver(IDropInfo dropInfo)
        {
            
            var dataObject = dropInfo.Data as IDataObject;

            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }

            if (dropInfo.Data.GetType() == typeof(FileNameItem))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }

            if(dropInfo.Data is Item)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }

            if (Mementor.IsInBatch)
            {
                Mementor.EndBatch();
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if(dataObject != null)
            {
                if (dataObject.ContainsFileDropList())
                {
                    Mementor.BeginBatch();
                    var filedrop = dataObject.GetFileDropList();

                    foreach (string str in filedrop)
                    {
                        foreach (string fm in FileMask)
                        {
                            if (System.IO.Path.GetExtension(str).ToUpperInvariant() == fm)
                            {
                                FileNameItem lbfn = new FileNameItem(FolderPath, MessageAddress, OSCValidation, Mementor) { FileIsSelected = false, FileName = str };
                                FilePaths.Add(lbfn);
                                Mementor.ElementAdd(FilePaths, lbfn);
                            }
                        }
                    }
                    Mementor.EndBatch();
                }
            }

            if (dropInfo.DragInfo != null)
            {
                if (dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget)
                {
                    if(dropInfo.Data is FileNameItem)
                    {
                        FileNameItem filenameitem = dropInfo.Data as FileNameItem;
                        foreach (string fm in FileMask)
                        {
                            if (System.IO.Path.GetExtension(filenameitem.FileName).ToUpperInvariant() == fm)
                            {
                                FileNameItem newfilenameitem = filenameitem.Clone() as FileNameItem;
                                newfilenameitem.FileIsSelected = true;
                                newfilenameitem.UpdateMessageAddress(MessageAddress);
                                SelectedFileNameItem = newfilenameitem;
                                FilePaths.Insert(dropInfo.InsertIndex, newfilenameitem);
                                Mementor.ElementAdd(FilePaths, newfilenameitem);
                            }
                        }
                    }
                }
            }
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            FileNameItem filenameitem = (FileNameItem)dragInfo.SourceItem;
            dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            dragInfo.Data = filenameitem;
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem.GetType() == typeof(FileNameItem))
            {
                return true;
            }
            return false;
        }

        public void Dropped(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo != null)
            {
                if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget && dropInfo.Data.GetType() == typeof(FileNameItem))
                {
                    FileNameItem filenameitem = dropInfo.Data as FileNameItem;
                    FileNameItem newfilenameitem = filenameitem.Clone() as FileNameItem;

                    newfilenameitem.UpdateMessageAddress(MessageAddress);
                    FilePaths.Insert(dropInfo.InsertIndex, newfilenameitem);
                    FilePaths.Remove(filenameitem);
                    newfilenameitem.FileIsSelected = true;
                    SelectedFileNameItem = newfilenameitem;
                    Mementor.ElementAdd(FilePaths, newfilenameitem);
                }
            }
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            //throw new NotImplementedException();
        }

        public void DragCancelled()
        {
            //throw new NotImplementedException();
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region COPY/PASTE/RESET

        public void Reset()
        {
            DisabledMessages();

            Mementor.PropertyChange(this, "FilePaths");
            FilePaths.Clear();

            EnabledMessages();
        }

        public void Copy(FileSelectorModel fileselectormodel)
        {
            fileselectormodel.MessageAddress = MessageAddress;
            fileselectormodel.FolderPath = FolderPath;
            List<FileNameItemModel> FileNameItemModelList = new List<FileNameItemModel>();
            foreach (var item in FilePaths)
            {
                var filenameitemmodel = new FileNameItemModel();
                
                item.Copy(filenameitemmodel);
                
                FileNameItemModelList.Add(filenameitemmodel);
            }
            fileselectormodel.FilePaths = FileNameItemModelList;
        }

        public void Paste(FileSelectorModel fileselectormodel)
        {
            DisabledMessages();

            MessageAddress = fileselectormodel.MessageAddress;
            FolderPath = fileselectormodel.FolderPath;

            FilePaths.Clear();

            foreach (var item in fileselectormodel.FilePaths)
            {
                FileNameItem filenameitem = new FileNameItem(FolderPath, MessageAddress, OSCValidation, Mementor);
                filenameitem.Paste(item);
                //filenameitem.UpdateMessageAddress(MessageAddress);
                FilePaths.Add(filenameitem);
            }

            EnabledMessages();
        }
        #endregion
    }
}
 