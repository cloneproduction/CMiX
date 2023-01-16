// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Easing : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public Easing(EasingModel easingModel)
        {
            this.ID = easingModel.ID;
            IsEnabled = easingModel.IsEnabled;

            EasingMode = EasingMode.In;
            EasingFunction = EasingFunction.Linear;
            IsActive = true;
        }


        public Guid ID { get; set; }


        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                SetProperty(ref _isEnabled, value);
                ControlMessenger.Send(this);
            }
        }

        private EasingFunction _easingFunction;
        public EasingFunction EasingFunction
        {
            get => _easingFunction;
            set
            {
                SetProperty(ref _easingFunction, value);
                ControlMessenger.Send(this);
            }
        }

        private EasingMode _easingMode;
        public EasingMode EasingMode
        {
            get => _easingMode;
            set
            {
                SetProperty(ref _easingMode, value);
                ControlMessenger.Send(this);
            }
        }


        public void SetViewModel(IModel model)
        {
            EasingModel easingModel = model as EasingModel;
            this.ID = easingModel.ID;
            this.IsEnabled = easingModel.IsEnabled;
            this.EasingFunction = easingModel.EasingFunction;
            this.EasingMode = easingModel.EasingMode;
        }

        public IModel GetModel()
        {
            EasingModel model = new EasingModel();
            model.ID = this.ID;
            model.IsEnabled = this.IsEnabled;
            model.EasingFunction = this.EasingFunction;
            model.EasingMode = this.EasingMode;
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
