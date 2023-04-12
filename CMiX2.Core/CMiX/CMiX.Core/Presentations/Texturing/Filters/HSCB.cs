// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class HSCB : ObservableObject, ITextureFilter
    {
        public HSCB(HSCBModel HSCBModel, CompositionService compositionService)
        {
            this.ID = HSCBModel.ID;
            Name = HSCBModel.Name;
            Visible = new BooleanValue(HSCBModel.Visible, compositionService);
            Hue = new FloatValue(HSCBModel.Hue, compositionService);
            Saturation = new FloatValue(HSCBModel.Saturation, compositionService);
            Contrast = new FloatValue(HSCBModel.Contrast, compositionService);
            Brightness = new FloatValue(HSCBModel.Brightness, compositionService);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public FloatValue Hue { get; set; }
        public FloatValue Saturation { get; set; }
        public FloatValue Contrast { get; set; }
        public FloatValue Brightness { get; set; }
        public TextureFilterName Name { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool enabled;

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            
        }
    }
}
