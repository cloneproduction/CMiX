using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class Playlist : ObservableObject, IControl, IDropTarget
    {
        public Playlist(PlaylistModel playlistModel)
        {
            this.ID = playlistModel.ID;
            this.Name = playlistModel.Name;
            this.IsRenaming = false;
            Compositions = new ObservableCollection<Composition>();

            RenameCommand = new RelayCommand(Rename);
        }

        public ICommand RenameCommand { get; set; }
        public Guid ID { get; set; }
        public ObservableCollection<Composition> Compositions { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }


        public void Rename()
        {
            this.IsRenaming = true;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            IDataObject dataObject = dropInfo.Data as IDataObject;

            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            DataObject dataObject = dropInfo.Data as DataObject;
            if (dataObject != null)
            {
                if (dataObject.ContainsFileDropList())
                {
                    var filedrop = dataObject.GetFileDropList();
                    foreach (string str in filedrop)
                    {
                        if (Path.GetExtension(str).ToUpperInvariant() == ".COMPMIX")
                        {
                            //byte[] data = File.ReadAllBytes(str);
                            //CompositionModel compositionmodel = Serializer.Deserialize<CompositionModel>(data);
                            //Compositions.Add(compositionmodel);
                        }
                    }
                }
            }
        }


        public IControlModel GetModel()
        {
            PlaylistModel playlistModel = new PlaylistModel();
            playlistModel.ID = this.ID;
            playlistModel.Name = this.Name;
            return playlistModel;
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}
