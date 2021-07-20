// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Mask : ObservableObject, IControl
    {
        public Mask(MaskModel maskModel)
        {
            MaskType = maskModel.MaskType;
            MaskControlType = maskModel.MaskControlType;
            // Enabled = maskModel.Enabled;
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }


        private bool _IsMask;
        public bool IsMask
        {
            get => _IsMask;
            set
            {
                SetProperty(ref _IsMask, value);

            }
        }

        private string _masktype;
        public string MaskType
        {
            get => _masktype;
            set
            {
                SetProperty(ref _masktype, value);

            }
        }

        private string _maskcontroltype;
        public string MaskControlType
        {
            get => _maskcontroltype;
            set
            {
                SetProperty(ref _maskcontroltype, value);

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
            MaskModel maskModel = model as MaskModel;
            this.ID = maskModel.ID;
            this.IsMask = maskModel.IsMask;
            this.MaskType = maskModel.MaskType;
            this.MaskControlType = maskModel.MaskControlType;
        }

        public IModel GetModel()
        {
            MaskModel maskModel = new MaskModel();

            maskModel.ID = this.ID;
            maskModel.IsMask = this.IsMask;
            maskModel.MaskType = this.MaskType;
            maskModel.MaskControlType = this.MaskControlType;

            return maskModel;
        }
    }
}
