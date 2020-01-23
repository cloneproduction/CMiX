﻿using CMiX.MVVM.Commands;
using CMiX.MVVM.Models;
using CMiX.MVVM.Services;
using CMiX.MVVM.ViewModels;
using GongSolutions.Wpf.DragDrop;
using Memento;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CMiX.Studio.ViewModels
{
    public class LayerEditor : ViewModel, ILayerEditor, IDropTarget
    {
        public LayerEditor(ObservableCollection<Layer> layers, MessageService messageService, string messageAddress, MasterBeat masterBeat, Assets assets, Mementor mementor)
        {
            Mementor = mementor;
            LayerManager = new LayerManager(messageService);
            Layers = layers;
            
            Assets = assets;
            MasterBeat = masterBeat;

            MessageAddress = messageAddress;
            MessageService = messageService;

            DeleteSelectedLayerCommand = new RelayCommand(p => DeleteSelectedLayer());
            DuplicateSelectedLayerCommand = new RelayCommand(p => DuplicateSelectedLayer());
            DuplicateSelectedLayerLinkCommand = new RelayCommand(p => DuplicateSelectedLayerLink(p));
            RenameSelectedLayerCommand = new RelayCommand(p => RenameSelectedLayer());
            AddLayerCommand = new RelayCommand(p => AddLayer());
            CopyLayerCommand = new RelayCommand(p => CopyLayer());
            PasteLayerCommand = new RelayCommand(p => PasteLayer());
            ResetLayerCommand = new RelayCommand(p => ResetLayer());
            SelectLayerCommand = new RelayCommand(p => SelectLayer(p));
        }

        public ICommand AddLayerCommand { get; }
        public ICommand DeleteSelectedLayerCommand { get; }
        public ICommand DuplicateSelectedLayerCommand { get; }
        public ICommand DuplicateSelectedLayerLinkCommand { get; }

        public ICommand RenameSelectedLayerCommand { get; }
        public ICommand CopyLayerCommand { get; }
        public ICommand PasteLayerCommand { get; }
        public ICommand ResetLayerCommand { get; }
        public ICommand SelectLayerCommand { get; }

        public LayerManager LayerManager { get; set; }
        public ObservableCollection<Layer> Layers { get; set; }

        private Layer _selectedLayer;
        public Layer SelectedLayer
        {
            get => _selectedLayer;
            set => SetAndNotify(ref _selectedLayer, value);
        }

        public MasterBeat MasterBeat { get; set; }
        public string MessageAddress { get; set; }
        public MessageService MessageService { get; set; }
        public Mementor Mementor { get; set; }
        public Assets Assets { get; set; }

        public void SelectLayer(object obj)
        {
            if(obj is Layer)
            {
                var layer = obj as Layer;
                SelectedLayer = layer;
                layer.IsSelected = true;
            }
        }

        #region COPY/PASTE/RESET LAYER
        private void CopyLayer()
        {
            LayerModel layerModel = SelectedLayer.GetModel();
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
                var selectedlayername = SelectedLayer.Name;
                var selectedLayerID = SelectedLayer.ID;

                var layerModel = data.GetData(nameof(LayerModel)) as LayerModel;

                SelectedLayer.Name = selectedlayername;
                SelectedLayer.ID = selectedLayerID;
                SelectedLayer.SetViewModel(layerModel);


                //SelectedLayer.CopyModel(layerModel);

                MessageService.SendMessages(MessageAddress, MessageCommand.VIEWMODEL_UPDATE, null, layerModel);
                Mementor.EndBatch();
            }
        }

        public void ResetLayer()
        {
            SelectedLayer.Reset();
            LayerModel layerModel = SelectedLayer.GetModel();
            MessageService.SendMessages(MessageAddress, MessageCommand.VIEWMODEL_UPDATE, null, layerModel);
        }
        #endregion

        #region ADD/DUPLICATE/DELETE LAYERS
        public void RenameSelectedLayer()
        {
            if(SelectedLayer != null)
                SelectedLayer.IsRenaming = true;
        }

        public void AddLayer()
        {
            //LayerManager.CreateLayer(this);
        }

        private void DuplicateSelectedLayer()
        {
            //LayerManager.DuplicateLayer(this);
        }

        private void DuplicateSelectedLayerLink(object obj)
        {
            if (obj is Composition)
            {
                var composition = obj as Composition;
                LayerManager.DuplicateLayerLink(composition);
            }
        }

        private void DeleteSelectedLayer()
        {
            //LayerManager.DeleteLayer(this);
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
            Mementor.BeginBatch();
            if (dropInfo.DragInfo != null)
            {
                int sourceindex = dropInfo.DragInfo.SourceIndex;
                int insertindex = dropInfo.InsertIndex;
                
                if (sourceindex != insertindex)
                {
                    if (insertindex >= Layers.Count - 1)
                        insertindex -= 1;

                    Layers.Move(sourceindex, insertindex);
                    Mementor.ElementIndexChange(Layers, Layers[insertindex], sourceindex);
                    SelectedLayer = Layers[insertindex];

                    int[] moveIndex = new int[2] { sourceindex, insertindex };
                    MessageService.SendMessages(MessageAddress, MessageCommand.LAYER_MOVE, null, moveIndex); 
                }
            }
            Mementor.EndBatch();
        }
        #endregion

        public List<string> GetLayerNames()
        {
            var layerNames = new List<string>();
            foreach (var layer in Layers)
            {
                layerNames.Add(layer.Name);
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

        public LayerEditorModel GetModel()
        {
            LayerEditorModel layerEditorModel = new LayerEditorModel();
            if(SelectedLayer != null)
                layerEditorModel.SelectedLayerModel = SelectedLayer.GetModel();

            foreach (var layer in Layers)
            {
                var layerModel = layer.GetModel();
                layerEditorModel.LayerModels.Add(layerModel);
            }
            return layerEditorModel;
        }

        public void SetViewModel(LayerEditorModel layerEditorModel)
        {
            if(SelectedLayer != null)
                SelectedLayer.SetViewModel(layerEditorModel.SelectedLayerModel);
            
            Layers.Clear();
            foreach (var layerModel in layerEditorModel.LayerModels)
            {
                //LayerManager.CreateLayer(this).GetModel();
            }
        }
    }
}
