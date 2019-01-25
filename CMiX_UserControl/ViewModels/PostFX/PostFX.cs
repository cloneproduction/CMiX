﻿using System;
using CMiX.Services;
using CMiX.Models;
using System.Windows;
using System.Windows.Input;
using GuiLabs.Undo;

namespace CMiX.ViewModels
{
    public class PostFX : ViewModel, IMessengerData
    {
        #region CONSTRUCTORS
        public PostFX(string layername, IMessenger messenger, ActionManager actionmanager)
            : this
            (
                actionmanager: actionmanager,
                feedback: new Slider(String.Format("{0}/{1}/{2}", layername, nameof(PostFX), "Feedback"), messenger, actionmanager),
                blur: new Slider(String.Format("{0}/{1}/{2}", layername, nameof(PostFX), "Blur"), messenger, actionmanager),
                transforms: ((PostFXTransforms)0).ToString(), 
                view: ((PostFXView)0).ToString(),
                messageaddress: String.Format("{0}/{1}/", layername, nameof(PostFX)),
                messenger: messenger,
                messageEnabled: true
            )
        { }

        public PostFX
            (
                ActionManager actionmanager,
                Slider feedback,
                Slider blur,
                string transforms,
                string view,
                IMessenger messenger,
                string messageaddress,
                bool messageEnabled
            )
            : base (actionmanager)
        {
            Feedback = feedback;
            Blur = blur;
            Transforms = transforms;
            View = view;
            Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            MessageAddress = messageaddress;
            MessageEnabled = messageEnabled;
            CopySelfCommand = new RelayCommand(p => CopySelf());
            PasteSelfCommand = new RelayCommand(p => PasteSelf());
            ResetSelfCommand = new RelayCommand(p => ResetSelf());
        }
        #endregion

        #region PROPERTIES
        public string MessageAddress { get; set; }

        public bool MessageEnabled { get; set; }

        public IMessenger Messenger { get; }

        public ICommand CopySelfCommand { get; }
        public ICommand PasteSelfCommand { get; }
        public ICommand ResetSelfCommand { get; }

        public Slider Feedback { get; }
        public Slider Blur { get; }

        private string _transforms;
        [OSC]
        public string Transforms
        {
            get => _transforms;
            set
            {
                SetAndNotify(ref _transforms, value);
                if(MessageEnabled)
                    Messenger.SendMessage(MessageAddress + nameof(Transforms), Transforms);
            }
        }

        private string _view;
        [OSC]
        public string View
        {
            get => _view;
            set
            {
                SetAndNotify(ref _view, value);
                if(MessageEnabled)
                    Messenger.SendMessage(MessageAddress + nameof(View), View);
            }
        }
        #endregion

        #region COPY/PASTE/RESET
        public void Copy(PostFXDTO postFXdto)
        {
            Feedback.Copy(postFXdto.Feedback);
            Blur.Copy(postFXdto.Blur);
            postFXdto.Transforms = Transforms;
            postFXdto.View = View;
        }

        public void Paste(PostFXDTO postFXdto)
        {
            MessageEnabled = false;
            Feedback.Paste(postFXdto.Feedback);
            Blur.Paste(postFXdto.Blur);
            Transforms = postFXdto.Transforms;
            View = postFXdto.View;
            MessageEnabled = true;
        }

        public void CopySelf()
        {
            PostFXDTO postFXdto = new PostFXDTO();
            this.Copy(postFXdto);
            IDataObject data = new DataObject();
            data.SetData("PostFX", postFXdto, false);
            Clipboard.SetDataObject(data);
        }

        public void PasteSelf()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent("PostFX"))
            {
                var postFXdto = (PostFXDTO)data.GetData("PostFX") as PostFXDTO;
                this.Paste(postFXdto);

                Messenger.QueueObject(this);
                Messenger.SendQueue();
            }
        }

        public void ResetSelf()
        {
            PostFXDTO postFXdto = new PostFXDTO();
            this.Paste(postFXdto);
        }
        #endregion
    }
}