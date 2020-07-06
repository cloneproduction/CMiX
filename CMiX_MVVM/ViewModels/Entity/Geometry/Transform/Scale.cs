﻿using System.Windows;
using CMiX.MVVM.Models;
using CMiX.MVVM.Services;

namespace CMiX.MVVM.ViewModels
{
    public class Scale : Sendable
    {
        public Scale() 
        {
            Uniform = new Slider(nameof(Uniform), this);
            Uniform.Amount = 1.0;

            X = new Slider(nameof(X), this);
            X.Amount = 1.0;

            Y = new Slider(nameof(Y), this);
            Y.Amount = 1.0;

            Z = new Slider(nameof(Z), this);
            Z.Amount = 1.0;

            IsUniform = true;
        }

        public Scale(Sendable parentSendable) : this()
        {
            SubscribeToEvent(parentSendable);
        }

        public Slider X { get; set; }
        public Slider Y { get; set; }
        public Slider Z { get; set; }
        public Slider Uniform { get; set; }

        private bool _isUniform;
        public bool IsUniform
        {
            get => _isUniform;
            set => SetAndNotify(ref _isUniform, value);
        }

        public override void OnParentReceiveChange(object sender, ModelEventArgs e)
        {
            if (e.ParentMessageAddress + this.GetMessageAddress() == e.MessageAddress)
                this.SetViewModel(e.Model as ScaleModel);
            else
                OnReceiveChange(e.Model, e.MessageAddress, e.ParentMessageAddress + this.GetMessageAddress());
        }
    }
}
