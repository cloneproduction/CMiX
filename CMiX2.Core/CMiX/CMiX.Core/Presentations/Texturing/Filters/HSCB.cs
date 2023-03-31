// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class HSCB : ObservableObject, ITextureFilter
    {
        public HSCB(HSCBModel HSCBModel)
        {
            this.ID = HSCBModel.ID;

            Name = HSCBModel.Name;
            Visible = new BooleanValue(HSCBModel.Visible);
            Hue = new FloatValue(HSCBModel.HueModel);
            Saturation = new FloatValue(HSCBModel.SaturationModel);
            Contrast = new FloatValue(HSCBModel.ConstrastModel);
            Brightness = new FloatValue(HSCBModel.BrightnessModel);
            IsExpanded = true;
        }

        public Guid ID { get; set; }

        public BooleanValue Visible { get; set; }
        public FloatValue Hue { get; set; }
        public FloatValue Saturation { get; set; }
        public FloatValue Contrast { get; set; }
        public FloatValue Brightness { get; set; }
        public TextureFilterName Name { get; set; }
        public FloatValue Control { get; set; }


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


        public void Dispose()
        {
            
        }
    }
}
