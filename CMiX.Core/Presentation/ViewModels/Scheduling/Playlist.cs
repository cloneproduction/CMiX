using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduler;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class Playlist : ObservableObject, IControl, IDropTarget
    {
        public Playlist(PlaylistModel playlistModel)
        {
            this.ID = playlistModel.ID;
            this.Name = playlistModel.Name;
            Compositions = new ObservableCollection<Composition>();
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public Guid ID { get; set; }
        public ObservableCollection<Composition> Compositions { get; set; }
        public Communicator Communicator { get; set; }


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


        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public void SetViewModel(IModel model)
        {
            PlaylistModel playlistModel = model as PlaylistModel;
            this.ID = playlistModel.ID;
            this.Name = playlistModel.Name;
            //foreach (var composition in playlistModel.Compositions)
            //{
            //    Compositions.Add(composition);
            //}
        }

        public IModel GetModel()
        {
            PlaylistModel playlistModel = new PlaylistModel();
            playlistModel.ID = this.ID;
            playlistModel.Name = this.Name;
            return playlistModel;
        }
    }
}
