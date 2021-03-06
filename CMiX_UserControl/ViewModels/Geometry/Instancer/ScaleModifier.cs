﻿using System;
using System.Windows;
using System.Collections.ObjectModel;
using CMiX.MVVM.ViewModels;
using CMiX.MVVM.Models;
using Memento;

namespace CMiX.ViewModels
{
    [Serializable]
    public class ScaleModifier : ViewModel
    {
        public ScaleModifier(string messageaddress, ObservableCollection<OSCValidation> oscvalidation, Mementor mementor, Beat beat)
            : base(oscvalidation, mementor)
        {
            MessageAddress = String.Format("{0}{1}/", messageaddress, nameof(ScaleModifier));

            Scale = new AnimParameter(MessageAddress + nameof(Scale), oscvalidation, mementor, beat, true);
            ScaleX = new AnimParameter(MessageAddress + nameof(ScaleX), oscvalidation, mementor, beat, false);
            ScaleY = new AnimParameter(MessageAddress + nameof(ScaleY), oscvalidation, mementor, beat, false);
            ScaleZ = new AnimParameter(MessageAddress + nameof(ScaleZ), oscvalidation, mementor, beat, false);
        }

        #region PROPERTIES
        public AnimParameter Scale { get; set; }
        public AnimParameter ScaleX { get; set; }
        public AnimParameter ScaleY { get; set; }
        public AnimParameter ScaleZ { get; set; }
        #endregion

        #region METHODS
        public void UpdateMessageAddress(string messageaddress)
        {
            MessageAddress = messageaddress;

            Scale.UpdateMessageAddress(String.Format("{0}{1}/", messageaddress, nameof(Scale)));
            ScaleX.UpdateMessageAddress(String.Format("{0}{1}/", messageaddress, nameof(ScaleX)));
            ScaleY.UpdateMessageAddress(String.Format("{0}{1}/", messageaddress, nameof(ScaleY)));
            ScaleZ.UpdateMessageAddress(String.Format("{0}{1}/", messageaddress, nameof(ScaleZ)));
        }
        #endregion

        #region COPY/PASTE/RESET
        public void CopyGeometry()
        {
            ScaleModifierModel Scalemodifiermodel = new ScaleModifierModel();
            this.Copy(Scalemodifiermodel);
            IDataObject data = new DataObject();
            data.SetData("ScaleModifierModel", Scalemodifiermodel, false);
            Clipboard.SetDataObject(data);
        }

        public void PasteGeometry()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent("ScaleModifierModel"))
            {
                Mementor.BeginBatch();
                DisabledMessages();

                var Scalemodifiermodel = data.GetData("ScaleModifierModel") as ScaleModifierModel;
                var messageaddress = MessageAddress;
                this.Paste(Scalemodifiermodel);
                UpdateMessageAddress(messageaddress);
                this.Copy(Scalemodifiermodel);

                EnabledMessages();
                Mementor.EndBatch();

                QueueObjects(Scalemodifiermodel);
                SendQueues();
            }
        }

        public void ResetGeometry()
        {
            ScaleModifierModel Scalemodifiermodel = new ScaleModifierModel();
            this.Reset();
            this.Copy(Scalemodifiermodel);
            QueueObjects(Scalemodifiermodel);
            SendQueues();
        }

        public void Copy(ScaleModifierModel Scalemodifiermodel)
        {
            Scalemodifiermodel.MessageAddress = MessageAddress;
            Scale.Copy(Scalemodifiermodel.Scale);
            ScaleX.Copy(Scalemodifiermodel.ScaleX);
            ScaleY.Copy(Scalemodifiermodel.ScaleY);
            ScaleZ.Copy(Scalemodifiermodel.ScaleZ);
        }

        public void Paste(ScaleModifierModel Scalemodifiermodel)
        {
            DisabledMessages();

            MessageAddress = Scalemodifiermodel.MessageAddress;

            Scale.Paste(Scalemodifiermodel.Scale);
            ScaleX.Paste(Scalemodifiermodel.ScaleX);
            ScaleY.Paste(Scalemodifiermodel.ScaleY);
            ScaleZ.Paste(Scalemodifiermodel.ScaleZ);

            EnabledMessages();
        }

        public void Reset()
        {
            DisabledMessages();

            Scale.Reset();
            ScaleX.Reset();
            ScaleY.Reset();
            ScaleZ.Reset();

            EnabledMessages();

            ScaleModifierModel Scalemodifiermodel = new ScaleModifierModel();
            this.Copy(Scalemodifiermodel);
            QueueObjects(Scalemodifiermodel);
            SendQueues();
        }
        #endregion
    }
}
