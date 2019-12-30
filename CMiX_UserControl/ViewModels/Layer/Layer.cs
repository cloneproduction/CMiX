﻿using System;
using System.Collections.ObjectModel;
using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;
using Memento;
using CMiX.MVVM;
using CMiX.MVVM.Services;

namespace CMiX.ViewModels
{
    public class Layer : ViewModel, ISendable, IUndoable
    {
        #region CONSTRUCTORS
        public Layer(MasterBeat masterBeat, string messageAddress,  MessageService messageService, Mementor mementor) 
        {
            MessageAddress =  messageAddress;
            Enabled = false;
            LayerName = messageAddress;           

            BlendMode = new BlendMode(masterBeat, MessageAddress, messageService, mementor);
            Fade = new Slider(MessageAddress + nameof(Fade), messageService, mementor);

            Content = new Content(masterBeat, MessageAddress, messageService, mementor);
            Mask = new Mask(masterBeat, MessageAddress, messageService, mementor);
            LayerControls = new ObservableCollection<ViewModel>();
            LayerControls.Add(Content);
            LayerControls.Add(Mask);

            PostFX = new PostFX(MessageAddress, messageService, mementor);
        }
        #endregion

        #region PROPERTIES
        private int _selectedLayerControlIndex;
        public int SelectedLayerControlIndex
        {
            get { return _selectedLayerControlIndex; }
            set { _selectedLayerControlIndex = value; }
        }

        private ObservableCollection<ViewModel> _layerControls;
        public ObservableCollection<ViewModel> LayerControls
        {
            get => _layerControls;
            set => SetAndNotify(ref _layerControls, value);
        }

        private string _layername;
        public string LayerName
        {
            get => _layername;
            set => SetAndNotify(ref _layername, value);
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetAndNotify(ref _displayName, value);
        }

        private int _id;
        public int ID
        {
            get => _id;
            set => SetAndNotify(ref _id, value);
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetAndNotify(ref _enabled, value);
        }

        private bool _out;
        public bool Out
        {
            get => _out;
            set
            {
                if (Mementor != null)
                    Mementor.PropertyChange(this, nameof(Out));
                SetAndNotify(ref _out, value);
                //if (Out)
                //SendMessages(MessageAddress + nameof(Out), Out);
            }
        }

        private ViewModel _selectedEntityContext;
        public ViewModel SelectedEntityContext
        {
            get => _selectedEntityContext;
            set
            {
                SetAndNotify(ref _selectedEntityContext, value);
                Console.WriteLine("SelectedEntityContext " + value.GetType().ToString());
            }

        }


        public Slider Fade { get; }
        public Content Content { get; }
        public Mask Mask { get; }
        public PostFX PostFX { get; }
        public BlendMode BlendMode { get; }
        public string MessageAddress { get; set; }
        public MessageService MessageService { get; set; }
        public Mementor Mementor { get; set; }
        #endregion

        #region METHODS
        public void UpdateMessageAddress(string messageaddress)
        {
            MessageAddress = messageaddress;

            BlendMode.UpdateMessageAddress($"{MessageAddress}{nameof(BlendMode)}/");
            Fade.UpdateMessageAddress($"{MessageAddress}{nameof(Fade)}/");
            Content.UpdateMessageAddress($"{MessageAddress}{nameof(Content)}/");
            Mask.UpdateMessageAddress($"{MessageAddress}{nameof(Mask)}/");
            PostFX.UpdateMessageAddress($"{MessageAddress}{nameof(PostFX)}/");
        }

        public void AssignEntityToLayerControl(Entity entity)
        {
            var currentLayerControl = LayerControls[SelectedLayerControlIndex];

            if(currentLayerControl != null)
            {
                if(currentLayerControl is IEntityContext)
                {
                    var c = currentLayerControl as IEntityContext;
                    c.Entities.Add(entity);
                    Console.WriteLine("entity added to " + c.GetType().ToString());
                }
            }
            foreach (var layerControl in LayerControls)
            {
                if(layerControl is IEntityContext)
                {

                }
            }
        }
        #endregion

        #region COPY/PASTE/RESET
        public void Copy(LayerModel layermodel)
        {
            layermodel.MessageAddress = MessageAddress;
            layermodel.LayerName = LayerName;
            layermodel.DisplayName = DisplayName;
            layermodel.ID = ID;

            Fade.Copy(layermodel.Fade);
            BlendMode.Copy(layermodel.BlendMode);
            Content.Copy(layermodel.ContentModel);
            Mask.Copy(layermodel.MaskModel);
            PostFX.Copy(layermodel.PostFXModel);
        }

        public void Paste(LayerModel layerModel)
        {
            MessageService.DisabledMessages();

            MessageAddress = layerModel.MessageAddress;
            LayerName = layerModel.LayerName;
            DisplayName = layerModel.DisplayName;
            Out = layerModel.Out;
            ID = layerModel.ID;

            Fade.Paste(layerModel.Fade);
            BlendMode.Paste(layerModel.BlendMode);
            Content.Paste(layerModel.ContentModel);
            Mask.Paste(layerModel.MaskModel);
            PostFX.Paste(layerModel.PostFXModel);

            MessageService.EnabledMessages();
        }

        public void Reset()
        {
            Enabled = true;

            BlendMode.Reset();
            Fade.Reset();
            Content.Reset();
            Mask.Reset();
            PostFX.Reset();
        }
        #endregion
    }
}