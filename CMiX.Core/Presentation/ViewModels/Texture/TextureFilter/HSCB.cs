// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class HSCB : ObservableObject, ITextureFilter
    {
        public HSCB(HSCBModel HSCBModel)
        {
            this.ID = HSCBModel.ID;

            Name = HSCBModel.Name;
            Visible = new ToggleButton(HSCBModel.Visible);
            Hue = new Slider(nameof(Hue), HSCBModel.HueModel);
            Saturation = new Slider(nameof(Saturation), HSCBModel.SaturationModel);
            Contrast = new Slider(nameof(Contrast), HSCBModel.ConstrastModel);
            Brightness = new Slider(nameof(Brightness), HSCBModel.BrightnessModel);
            Enabled = HSCBModel.Enabled;
            IsExpanded = true;
        }

        public Guid ID { get; set; }

        public ToggleButton Visible { get; set; }
        public Slider Hue { get; set; }
        public Slider Saturation { get; set; }
        public Slider Contrast { get; set; }
        public Slider Brightness { get; set; }
        public TextureFilterName Name { get; set; }
        public Slider Control { get; set; }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public IModel GetModel()
        {
            HSCBModel HSCBModel = new HSCBModel();

            HSCBModel.ID = ID;
            HSCBModel.Name = Name;
            HSCBModel.Enabled = this.Enabled;

            HSCBModel.Visible = (ToggleButtonModel)Visible.GetModel();
            HSCBModel.HueModel = (SliderModel)Hue.GetModel();
            HSCBModel.SaturationModel = (SliderModel)Saturation.GetModel();
            HSCBModel.ConstrastModel = (SliderModel)Contrast.GetModel();
            HSCBModel.BrightnessModel = (SliderModel)Brightness.GetModel();

            return HSCBModel;
        }

        public void SetViewModel(IModel model)
        {
            HSCBModel HSCBModel = model as HSCBModel;
            ID = HSCBModel.ID;
            Name = HSCBModel.Name;
            Enabled = HSCBModel.Enabled;

            Visible.SetViewModel(HSCBModel.Visible);
            Hue.SetViewModel(HSCBModel.HueModel);
            Saturation.SetViewModel(HSCBModel.SaturationModel);
            Contrast.SetViewModel(HSCBModel.ConstrastModel);
            Brightness.SetViewModel(HSCBModel.BrightnessModel);
        }

        public void Dispose()
        {
            
        }
    }
}
