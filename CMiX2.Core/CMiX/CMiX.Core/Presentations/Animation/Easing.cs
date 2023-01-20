// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
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
            IsEnabled = new BooleanValue(easingModel.IsEnabled);
            Mode = new GenericValue<EasingMode>(easingModel.Mode);
            Function = new GenericValue<EasingFunction>(easingModel.Function);
            IsActive = true;
        }


        public Guid ID { get; set; }

        public BooleanValue IsEnabled { get; set; }
        public GenericValue<EasingFunction> Function { get; set; }
        public GenericValue<EasingMode> Mode { get; set; }


        public void SetViewModel(IModel model)
        {
            EasingModel easingModel = model as EasingModel;
            this.ID = easingModel.ID;
            this.IsEnabled.SetViewModel(easingModel.IsEnabled);
            this.Function.SetViewModel(easingModel.Function);
            this.Mode.SetViewModel(easingModel.Mode);
        }

        public IModel GetModel()
        {
            EasingModel model = new EasingModel();
            model.ID = this.ID;
            model.IsEnabled = (BooleanValueModel)IsEnabled.GetModel();
            model.Function = (GenericValueModel<EasingFunction>)this.Function.GetModel();
            model.Mode = (GenericValueModel<EasingMode>)this.Mode.GetModel();
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
