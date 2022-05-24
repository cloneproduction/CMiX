// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Pixelate : ObservableObject, ITextureFilter
    {
        public Pixelate(PixelateModel pixelateModel)
        {
            ID = pixelateModel.ID;
            Visible = new ToggleButton(pixelateModel.Visible);
            Name = pixelateModel.Name;
            Control = new Slider(nameof(Control), pixelateModel.Control);
            FactorX = new Slider(nameof(FactorX), pixelateModel.FactorX);
            FactorY = new Slider(nameof(FactorY), pixelateModel.FactorY);

            IsExpanded = true;
        }

        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }
        public Guid ID { get; set; }

        public Slider FactorX { get; set; }
        public Slider FactorY { get; set; }

        public Slider Control { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public IModel GetModel()
        {
            PixelateModel pixelateModel = new PixelateModel();

            pixelateModel.ID = ID;
            pixelateModel.Name = Name;
            pixelateModel.Visible = (ToggleButtonModel)Visible.GetModel();
            pixelateModel.Control = (SliderModel)Control.GetModel();
            pixelateModel.FactorX = (SliderModel)FactorX.GetModel();
            pixelateModel.FactorY = (SliderModel)FactorY.GetModel();

            return pixelateModel;
        }

        public void SetViewModel(IModel model)
        {
            PixelateModel pixelateModel = model as PixelateModel;

            ID = pixelateModel.ID;
            Name = pixelateModel.Name;
            Visible.SetViewModel(pixelateModel.Visible);
            Control.SetViewModel(pixelateModel.Control);
            FactorX.SetViewModel(pixelateModel.FactorX);
            FactorY.SetViewModel(pixelateModel.FactorY);
        }

        public void Dispose()
        {

        }
    }
}
