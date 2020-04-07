﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;
using Memento;
using CMiX.MVVM.Commands;
using CMiX.MVVM.Services;
using CMiX.MVVM.Resources;

namespace CMiX.Studio.ViewModels
{
    public class Scene : Component, IGetSet<SceneModel>
    {
        #region CONSTRUCTORS
        public Scene(Beat beat, string messageAddress, MessageService messageService, Mementor mementor)
        {
            Enabled = true;
            Beat = beat;
            Mementor = mementor;
            Name = "Scene";
            MessageAddress = $"{messageAddress}{nameof(Scene)}/";
            MessageService = messageService;
            Components = new ObservableCollection<Component>();

            BeatModifier = new BeatModifier(MessageAddress, Beat, messageService, mementor);
            PostFX = new PostFX(MessageAddress, messageService, mementor);

            CopyContentCommand = new RelayCommand(p => CopyContent());
            PasteContentCommand = new RelayCommand(p => PasteContent());
            ResetContentCommand = new RelayCommand(p => ResetContent());
        }
        #endregion

        #region PROPERTIES
        public ICommand DeleteEntityCommand { get; }
        public ICommand CopyContentCommand { get; }
        public ICommand PasteContentCommand { get; }
        public ICommand ResetContentCommand { get; }


        public BeatModifier BeatModifier { get; }
        public PostFX PostFX { get; }
        #endregion

        #region COPY/PASTE
        public SceneModel GetModel()
        {
            SceneModel sceneModel = new SceneModel();
            sceneModel.Enabled = Enabled;
            sceneModel.BeatModifierModel = BeatModifier.GetModel();
            sceneModel.PostFXModel = PostFX.GetModel();

            foreach (IGetSet<EntityModel> item in Components)
                sceneModel.ComponentModels.Add(item.GetModel());

            return sceneModel;
        }

        public void SetViewModel(SceneModel sceneModel)
        {
            MessageService.Disable();

            Enabled = sceneModel.Enabled;
            BeatModifier.SetViewModel(sceneModel.BeatModifierModel);
            PostFX.SetViewModel(sceneModel.PostFXModel);

            Components.Clear();
            foreach (EntityModel componentModel in sceneModel.ComponentModels)
            {
                Entity entity = new Entity(0, this.Beat, this.MessageAddress, this.MessageService, this.Mementor);
                entity.SetViewModel(componentModel);
                this.AddComponent(entity);
            }

            MessageService.Enable();
        }

        public void Reset()
        {
            MessageService.Disable();

            this.Enabled = true;
            this.BeatModifier.Reset();
            this.PostFX.Reset();

            MessageService.Enable();
        }

        #region COPYPASTE CONTENT
        public void CopyContent()
        {
            SceneModel contentmodel = GetModel();
            IDataObject data = new DataObject();
            data.SetData("ContentModel", contentmodel, false);
            Clipboard.SetDataObject(data);
        }

        public void PasteContent()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent("ContentModel"))
            {
                this.Mementor.BeginBatch();
                MessageService.Disable();

                var contentModel = data.GetData("ContentModel") as SceneModel;
                var contentmessageaddress = MessageAddress;
                this.SetViewModel(contentModel);

                MessageService.Enable();
                this.Mementor.EndBatch();

                MessageService.SendMessages(MessageAddress, MessageCommand.VIEWMODEL_UPDATE, null, contentModel);
            }
        }

        public void ResetContent()
        {
            this.Reset();
            MessageService.SendMessages(MessageAddress, MessageCommand.VIEWMODEL_UPDATE, null, GetModel());
        }
        #endregion

        #endregion
    }
}