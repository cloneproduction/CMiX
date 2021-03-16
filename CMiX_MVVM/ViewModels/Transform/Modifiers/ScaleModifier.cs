﻿using CMiX.MVVM.Interfaces;
using CMiX.MVVM.ViewModels.Beat;
using System;

namespace CMiX.MVVM.ViewModels
{
    public class ScaleModifier : MessageCommunicator, IModifier
    {
        public ScaleModifier(string name, MessageCommunicator parentSender, Scale scale, MasterBeat beat) : base(name, parentSender)
        {
            IsUniform = false;
            //X = new AnimParameter(nameof(X), this, scale.X.Amount, beat);
            //Y = new AnimParameter(nameof(Y), this, scale.Y.Amount, beat);
            //Z = new AnimParameter(nameof(Z), this, scale.Z.Amount, beat);
        }

        public override void SetViewModel(IModel model)
        {
            throw new NotImplementedException();
        }

        public override IModel GetModel()
        {
            throw new NotImplementedException();
        }

        public AnimParameter X { get; set; }
        public AnimParameter Y { get; set; }
        public AnimParameter Z { get; set; }

        private bool _isUniform;
        public bool IsUniform
        {
            get => _isUniform;
            set => SetAndNotify(ref _isUniform, value);
        }
    }
}