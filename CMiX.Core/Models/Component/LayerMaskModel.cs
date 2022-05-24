// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models.Component
{
    public class LayerMaskModel : IComponentModel
    {
        public LayerMaskModel()
        {
            ID = Guid.NewGuid();
            BlendMode = new BlendModeModel();
            Opacity = new SliderModel();
            Opacity.Amount = 1.0f;
            VisibilityModel = new ToggleButtonModel(true);
            ComponentModels = new ObservableCollection<IComponentModel>();
            ModifierManager = new ModifierManagerModel();
            ColorSelectorModel = new ColorSelectorModel();
            Camera = new CameraModel();
            AmbientOcclusion = new AmbientOcclusionModel();
            Invert = new ToggleButtonModel(true);

            MaskChannelSelector = new ComboBoxModel<MaskChannel>(MaskChannel.Luma);
        }

        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsVisible { get; set; }


        public ToggleButtonModel VisibilityModel { get; set; }
        public BlendModeModel BlendMode { get; set; }
        public SliderModel Opacity { get; set; }
        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public ColorSelectorModel ColorSelectorModel { get; set; }
        public CameraModel Camera { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }
        public ComboBoxModel<MaskChannel> MaskChannelSelector { get; set; }
        public ToggleButtonModel Invert { get; internal set; }
    }
}
