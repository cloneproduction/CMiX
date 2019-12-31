﻿using System;
using System.Windows;
using System.Collections.ObjectModel;
using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;
using Memento;
using CMiX.MVVM.Services;

namespace CMiX.ViewModels
{
    public class Transform : ViewModel, ISendable, IUndoable
    {
        public Transform(string messageAddress, Messenger messenger, Mementor mementor)
        {
            MessageAddress = $"{messageAddress}{nameof(Transform)}/";
            Translate = new Translate(MessageAddress, messenger, mementor);
            Scale = new Scale(MessageAddress, messenger, mementor);
            Rotation = new Rotation(MessageAddress, messenger, mementor);

            Is3D = false;
        }

        #region PROPERTY
        public Translate Translate { get; }
        public Scale Scale { get; }
        public Rotation Rotation { get; }

        private bool _is3D;
        public bool Is3D
        {
            get => _is3D;
            set
            {
                if (Mementor != null)
                    Mementor.PropertyChange(this, "Is3D");
                SetAndNotify(ref _is3D, value);
                //SendMessages(MessageAddress + nameof(Is3D), Is3D.ToString());
            }
        }

        public string MessageAddress { get; set; }
        public Messenger Messenger { get; set; }
        public Mementor Mementor { get; set; }
        #endregion

        #region METHODS
        public void UpdateMessageAddress(string messageaddress)
        {
            MessageAddress = messageaddress;

            Translate.UpdateMessageAddress(messageaddress);
            Scale.UpdateMessageAddress(messageaddress);
            Rotation.UpdateMessageAddress(messageaddress);
        }
        #endregion

        #region COPY/PASTE/RESET
        public void CopyGeometry()
        {
            TransformModel transformmodel = new TransformModel();
            this.Copy(transformmodel);
            IDataObject data = new DataObject();
            data.SetData("TransformModel", transformmodel, false);
            Clipboard.SetDataObject(data);
        }

        public void PasteGeometry()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent("TransformModel"))
            {
                Mementor.BeginBatch();
                Messenger.Disable();;

                var transformmodel = data.GetData("TransformModel") as TransformModel;
                var messageaddress = MessageAddress;
                this.Paste(transformmodel);
                UpdateMessageAddress(messageaddress);
                this.Copy(transformmodel);

                Messenger.Enable();
                Mementor.EndBatch();
                //this.SendMessages(nameof(TransformModel), transformmodel);
            }
        }

        public void ResetGeometry()
        {
            TransformModel transformmodel = new TransformModel();
            this.Reset();
            this.Copy(transformmodel);
            //this.SendMessages(nameof(TransformModel), transformmodel);
        }

        public void Copy(TransformModel transformmodel)
        {
            transformmodel.MessageAddress = MessageAddress;
            Translate.Copy(transformmodel.Translate);
            Scale.Copy(transformmodel.Scale);
            Rotation.Copy(transformmodel.Rotation);
            transformmodel.Is3D = Is3D;
        }

        public void Paste(TransformModel transformmodel)
        {
            Messenger.Disable();;

            MessageAddress = transformmodel.MessageAddress;
            Translate.Paste(transformmodel.Translate);
            Scale.Paste(transformmodel.Scale);
            Rotation.Paste(transformmodel.Rotation);
            Is3D = transformmodel.Is3D;

            Messenger.Enable();
        }

        public void Reset()
        {
            Messenger.Disable();;
            //Mementor.BeginBatch();

            Translate.Reset();
            Scale.Reset();
            Rotation.Reset();
            Is3D = false;
            //Mementor.EndBatch();
            Messenger.Enable();

            TransformModel transformmodel = new TransformModel();
            this.Copy(transformmodel);
            //this.SendMessages(nameof(TransformModel), transformmodel);
            //QueueObjects(transformmodel);
            //SendQueues();
        }
        #endregion
    }
}