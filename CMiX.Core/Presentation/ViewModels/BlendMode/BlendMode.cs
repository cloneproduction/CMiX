// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class BlendMode : ObservableObject, IControl
    {
        public BlendMode(BlendModeModel blendModeModel)
        {
            this.ID = blendModeModel.ID;
            Mode = blendModeModel.Mode;
        }

        public Communicator Communicator { get; set; }
        public Guid ID { get; set; }

        private string _mode;
        public string Mode
        {
            get { return _mode; }
            set
            {
                SetProperty(ref _mode, value);
                Communicator?.SendMessage(new MessageUpdateViewModel(this));
                Console.WriteLine("BlendModel is " + Mode);
            }
        }

        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            throw new NotImplementedException();
        }

        public void SetViewModel(IModel model)
        {
            BlendModeModel blendModeModel = model as BlendModeModel;
            this.Mode = blendModeModel.Mode;
        }

        public IModel GetModel()
        {
            BlendModeModel model = new BlendModeModel();
            model.ID = this.ID;
            model.Mode = this.Mode;
            return model;
        }
    }
}
