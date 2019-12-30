﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;
using CMiX.MVVM.Commands;
using Memento;
using GongSolutions.Wpf.DragDrop;
using CMiX.MVVM.Services;

namespace CMiX.ViewModels
{
    public class Composition : ViewModel, ISendable, IUndoable, IDropTarget
    {

        #region CONSTRUCTORS
        public Composition(ObservableCollection<Server> servers, Assets assets)
        {
            Name = string.Empty;
            MessageAddress = "/Composition";
            MessageService = new MessageService();

            EntityFactory = new EntityFactory();
            CreateServerValidation(servers);

            Assets = assets;
            Layers = new ObservableCollection<Layer>();
            Mementor = new Mementor();

            Transition = new Slider("/Transition", MessageService, Mementor);
            //ContentFolderName = string.Empty;
            MasterBeat = new MasterBeat(MessageService);
            Camera = new Camera(MessageService, MasterBeat, Mementor);

            string messageAddress = CreateLayerMessageAddress();
            CreateLayer(messageAddress, LayerID);

            ReloadCompositionCommand = new RelayCommand(p => ReloadComposition(p));
            ReloadAllOSCCommand = new RelayCommand(p => ReloadAllOSC());
            ResetAllOSCCommand = new RelayCommand(p => ResetAllOSC());
            AddLayerCommand = new RelayCommand(p => AddLayer());
            DeleteLayerCommand = new RelayCommand(p => DeleteLayer());
            DuplicateLayerCommand = new RelayCommand(p => DuplicateLayer());
            CopyLayerCommand = new RelayCommand(p => CopyLayer());
            PasteLayerCommand = new RelayCommand(p => PasteLayer());
            ResetLayerCommand = new RelayCommand(p => ResetLayer());

            AddEntityCommand = new RelayCommand(p => AddEntity(p));
        }
        #endregion

        #region PROPERTIES

        #region COMMANDS
        public ICommand ReloadCompositionCommand { get; }
        public ICommand ResetAllOSCCommand { get; }
        public ICommand ReloadAllOSCCommand { get; }

        public ICommand AddLayerCommand { get; }
        public ICommand CopyLayerCommand { get; }
        public ICommand PasteLayerCommand { get; }
        public ICommand ResetLayerCommand { get; }

        public ICommand SaveCompositionCommand { get; }
        public ICommand OpenCompositionCommand { get; }
        public ICommand DeleteLayerCommand { get; }
        public ICommand DuplicateLayerCommand { get; }

        public ICommand AddEntityCommand { get; set; }

        #endregion

        public EntityFactory EntityFactory { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public Camera Camera { get; set; }
        public Slider Transition { get; set; }

        public ObservableCollection<Layer> Layers { get; }

        private int _LayerID = 0;
        public int LayerID
        {
            get { return _LayerID; }
            set { _LayerID = value; }
        }

        private int _layerNameID = 0;
        public int LayerNameID
        {
            get { return _layerNameID; }
            set { _layerNameID = value; }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        private ObservableCollection<Entity> _entities;
        public ObservableCollection<Entity> Entities
        {
            get { return _entities; }
            set { _entities = value; }
        }

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public Assets Assets { get; set; }

        private Layer _selectedlayer;
        public Layer SelectedLayer
        {
            get => _selectedlayer;
            set => SetAndNotify(ref _selectedlayer, value);
        }
        public string MessageAddress { get; set; }
        public MessageService MessageService { get; set; }
        public Mementor Mementor { get; set; }
        #endregion

        #region PUBLIC METHODS
        public  void AddEntity(object layerControl)
        {
            Console.WriteLine("AddEntity From Composition");
            if (layerControl != null)
            {
                var lc = layerControl as IEntityContext;
                BeatModifier bm = new BeatModifier("testMessageAddress", this.MasterBeat, MessageService, Mementor);
                var entity = EntityFactory.CreateEntity(bm, "testMessageAddress", MessageService, Mementor);
                lc.Entities.Add(entity);
                Console.WriteLine("Added Entity from Composition");
            }
        }

        public Layer CreateLayer(string messageAddress, int layerID)
        {
            Layer layer = new Layer(MasterBeat, messageAddress, MessageService, Mementor)
            {
                ID = layerID,
                DisplayName = "Layer " + layerID
            };
            Layers.Add(layer);
            SelectedLayer = layer;

            return layer;
        }

        public List<string> GetLayerNames()
        {
            var layerNames = new List<string>();
            foreach (var layer in Layers)
	        {
                layerNames.Add(layer.LayerName);
	        }
            layerNames.Sort();
            return layerNames;
        }

        public List<int> GetLayerID()
        {
            var LayerID = new List<int>();
            foreach (var layer in Layers)
            {
                LayerID.Add(layer.ID);
            }
            return LayerID;
        }

        //public void UpdateLayerContentFolder(Layer layer)
        //{
        //    //layer.Content.Texture.FileSelector.FolderPath = ContentFolderName;
        //    //layer.Content.Geometry.FileSelector.FolderPath = ContentFolderName;
        //    //layer.Content.Geometry.GeometryFX.FileSelector.FolderPath = ContentFolderName;
        //    layer.Mask.Texture.FileSelector.FolderPath = ContentFolderName;
        //    layer.Mask.Geometry.FileSelector.FolderPath = ContentFolderName;
        //}
        #endregion

        #region PRIVATE METHODS
        private string CreateLayerMessageAddress()
        {
            return "/Layer" + LayerNameID.ToString() + "/";
        }

        private void CreateServerValidation(ObservableCollection<Server> servers)
        {
            foreach (var server in servers)
            {
                MessageService.ServerValidation.Add(new ServerValidation(server));
            }
        }

        private void ReloadComposition(object messenger)
        {
            CompositionModel compositionModel = new CompositionModel();
            this.Copy(compositionModel);
            MessageService.SendMessages(MessageAddress, MessageCommand.VIEWMODEL_UPDATE, null, compositionModel);
            Console.WriteLine("Composition Reloaded");
        }

        private void ReloadAllOSC()
        {
            foreach (var serverValidation in MessageService.ServerValidation)
                ReloadComposition(serverValidation.Server);
        }

        private void ResetAllOSC()
        {
            //foreach (var serverValidation in ServerValidation)
                //serverValidation.Server.Send("/CompositionReloaded", true);
        }
        #endregion

        #region COPY/PASTE/RESET LAYER
        private void CopyLayer()
        {
            LayerModel layerModel = new LayerModel();
            SelectedLayer.Copy(layerModel);
            IDataObject data = new DataObject();
            data.SetData(nameof(LayerModel), layerModel, false);
            Clipboard.SetDataObject(data);
        }

        private void PasteLayer()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(nameof(LayerModel)))
            {
                Mementor.BeginBatch();

                var selectedlayermessageaddress = SelectedLayer.MessageAddress;
                var selectedlayername = SelectedLayer.LayerName;
                var selectedname = SelectedLayer.DisplayName;
                var selectedLayerID = SelectedLayer.ID;

                var layerModel = data.GetData(nameof(LayerModel)) as LayerModel;

                SelectedLayer.Paste(layerModel);
                SelectedLayer.UpdateMessageAddress(selectedlayermessageaddress);
                SelectedLayer.LayerName = selectedlayername;
                SelectedLayer.DisplayName = selectedname;
                SelectedLayer.ID = selectedLayerID;
                SelectedLayer.Copy(layerModel);

                MessageService.SendMessages(MessageAddress, MessageCommand.VIEWMODEL_UPDATE, null, layerModel);
                Mementor.EndBatch();
            }
        }

        public void ResetLayer()
        {
            SelectedLayer.Reset();
            LayerModel layerModel = new LayerModel();
            SelectedLayer.Copy(layerModel);
            //SendMessages("LayerModel", layerModel);
        }
        #endregion

        #region ADD/DUPLICATE/DELETE LAYERS

        public void AddLayer()
        {
            Mementor.BeginBatch();
            MessageService.DisabledMessages();

            LayerID++;
            LayerNameID++;
            
            string messageAddress = CreateLayerMessageAddress();
            var layer = CreateLayer(messageAddress, LayerID);

            Mementor.ElementAdd(Layers, layer);
            MessageService.EnabledMessages();

            LayerModel layerModel = new LayerModel();
            layer.Copy(layerModel);

            MessageService.SendMessages(MessageAddress, MessageCommand.LAYER_ADD, null, layerModel);

            Mementor.EndBatch();
        }



        private void DuplicateLayer()
        {
            if(SelectedLayer != null)
            {
                Mementor.BeginBatch();
                MessageService.DisabledMessages();

                var lyr = SelectedLayer;
                LayerModel layermodel = new LayerModel();
                lyr.Copy(layermodel);

                LayerID++;
                LayerNameID++;
                var messageAddress = CreateLayerMessageAddress();

                Layer newlayer = CreateLayer(messageAddress, LayerID);
                newlayer.Paste(layermodel);
                newlayer.LayerName = messageAddress;
                newlayer.DisplayName += " - Copy";
                newlayer.ID = LayerID;
                newlayer.UpdateMessageAddress(messageAddress);
                newlayer.Enabled = false;
                newlayer.Copy(layermodel);

                SelectedLayer = newlayer;
                Mementor.ElementAdd(Layers, newlayer);

                int oldIndex = Layers.IndexOf(SelectedLayer);
                int newIndex = Layers.IndexOf(lyr) + 1;
                Layers.Move(oldIndex, newIndex);
                int[] movedIndex = new int[2] { oldIndex, newIndex };

                MessageService.EnabledMessages();
                Mementor.EndBatch();

                MessageService.SendMessages(MessageAddress, MessageCommand.LAYER_DUPLICATE, movedIndex, layermodel);
            }
        }

        private void DeleteLayer()
        {
            if(SelectedLayer != null)
            {
                Mementor.BeginBatch();
                MessageService.DisabledMessages();

                Layer removedlayer = SelectedLayer as Layer;
                int removedlayerindex = Layers.IndexOf(removedlayer);
                UpdateLayersIDOnDelete(removedlayer);
                Mementor.ElementRemove(Layers, removedlayer);
                Layers.Remove(removedlayer);

                if (Layers.Count > 0)
                {
                    if(removedlayerindex > 0)
                        SelectedLayer = Layers[removedlayerindex - 1];
                    else
                        SelectedLayer = Layers[0];
                }
                else
                {
                    LayerNameID = 0;
                }

                MessageService.EnabledMessages();
                Mementor.EndBatch();

                MessageService.SendMessages(MessageAddress, MessageCommand.LAYER_DELETE, null, removedlayerindex);
            }
        }

        public void UpdateLayersIDOnDelete(Layer deletedlayer)
        {
            foreach (var item in Layers)
            {
                if (item.ID > deletedlayer.ID)
                    item.ID--;
            }
            LayerID--;
        }
        #endregion

        #region COPY/PASTE/LOAD/SAVE/OPEN COMPOSITIONS

        public void Copy(CompositionModel compositionmodel)
        {
            compositionmodel.MessageAddress = MessageAddress;
            compositionmodel.Name = Name;
            //compositionmodel.ContentFolderName = ContentFolderName;

            compositionmodel.LayerID = GetLayerID();
            compositionmodel.LayerNames = GetLayerNames();
            compositionmodel.layerID = LayerID;
            compositionmodel.layerNameID = LayerNameID;

            foreach (Layer lyr in Layers)
            {
                LayerModel layermodel = new LayerModel();
                lyr.Copy(layermodel);
                compositionmodel.LayersModel.Add(layermodel);
            }

            SelectedLayer.Copy(compositionmodel.SelectedLayer);
            MasterBeat.Copy(compositionmodel.MasterBeatModel);
            Camera.Copy(compositionmodel.CameraModel);
            Transition.Copy(compositionmodel.TransitionModel);
        }

        public void Paste(CompositionModel compositionmodel)
        {
            MessageService.DisabledMessages();

            MessageAddress = compositionmodel.MessageAddress;
            Name = compositionmodel.Name;
            //ContentFolderName = compositionmodel.ContentFolderName;

            LayerID = compositionmodel.layerID;
            LayerNameID = compositionmodel.layerNameID;

            Layers.Clear();
            foreach (LayerModel layermodel in compositionmodel.LayersModel)
            {
                Layer layer = new Layer(MasterBeat, CreateLayerMessageAddress(), MessageService, Mementor);
                layer.Paste(layermodel);
                Layers.Add(layer);
            }

            SelectedLayer.Paste(compositionmodel.SelectedLayer);
            MasterBeat.Paste(compositionmodel.MasterBeatModel);
            Camera.Paste(compositionmodel.CameraModel);
            Transition.Paste(compositionmodel.TransitionModel);

            MessageService.EnabledMessages();
        }
        #endregion

        #region DRAG DROP
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data.GetType() == typeof(Layer) && Layers.Count > 1)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }
            else if (dropInfo.Data != null && dropInfo.TargetItem != null)
                SelectedLayer = dropInfo.TargetItem as Layer;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo != null)
            {
                int sourceindex = dropInfo.DragInfo.SourceIndex;
                int insertindex = dropInfo.InsertIndex;

                if(sourceindex != insertindex)
                {
                    Mementor.BeginBatch();

                    if (insertindex >= Layers.Count - 1)
                    {
                        insertindex -= 1;
                    }

                    Layers.Move(sourceindex, insertindex);
                    Mementor.ElementIndexChange(Layers, Layers[insertindex], sourceindex);
                    SelectedLayer = Layers[insertindex];

                    int[] moveIndex = new int[2] { sourceindex, insertindex };

                    MessageService.SendMessages(MessageAddress, MessageCommand.LAYER_MOVE, null, moveIndex);

                    Mementor.EndBatch();
                }
            }
        }
        #endregion
    }
}