// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Inverter : ObservableObject, IControl
    {
        public Inverter(string name, InverterModel inverterModel)
        {
            this.ID = inverterModel.ID;
            Invert = new Slider(nameof(Invert), inverterModel.Invert);
            InvertMode = new ComboBox<TextureInvertMode>(inverterModel.InvertMode);
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }
        public Slider Invert { get; set; }
        public ComboBox<TextureInvertMode> InvertMode { get; set; }


        public void SetViewModel(IModel model)
        {
            InverterModel inverterModel = model as InverterModel;
            this.ID = inverterModel.ID;
            this.Invert.SetViewModel(inverterModel.Invert);
            this.InvertMode.SetViewModel(inverterModel.InvertMode);
        }

        public IModel GetModel()
        {
            InverterModel model = new InverterModel();
            model.ID = this.ID;
            model.Invert = (SliderModel)this.Invert.GetModel();
            model.InvertMode = (ComboBoxModel<TextureInvertMode>)this.InvertMode.GetModel();
            return model;
        }
    }
}
