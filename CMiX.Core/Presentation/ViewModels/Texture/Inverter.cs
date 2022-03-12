// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Inverter : ObservableObject, IControl
    {
        public Inverter(string name, InvertModel inverterModel)
        {
            this.ID = inverterModel.ID;
            Invert = new Slider(nameof(Invert), inverterModel.Factor);
            InvertMode = new ComboBox<TextureInvertMode>(inverterModel.InvertMode);
        }


        public Guid ID { get; set; }
        public Slider Invert { get; set; }
        public ComboBox<TextureInvertMode> InvertMode { get; set; }


        public void SetViewModel(IModel model)
        {
            InvertModel inverterModel = model as InvertModel;
            this.ID = inverterModel.ID;
            this.Invert.SetViewModel(inverterModel.Factor);
            this.InvertMode.SetViewModel(inverterModel.InvertMode);
        }

        public IModel GetModel()
        {
            InvertModel model = new InvertModel();
            model.ID = this.ID;
            model.Factor = (SliderModel)this.Invert.GetModel();
            model.InvertMode = (ComboBoxModel<TextureInvertMode>)this.InvertMode.GetModel();
            return model;
        }
    }
}
