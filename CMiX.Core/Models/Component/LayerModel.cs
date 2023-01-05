// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Models.Component
{
    public class LayerModel : IPrefabModel
    {
        public LayerModel()
        {
            ID = Guid.NewGuid();
            Name = "Layer";

            Visibility = new ToggleButtonModel();
            IsMask = new ToggleButtonModel();
            Opacity = new SliderModel(1.0f);

            BackgroundColor = new ColorSelectorModel("#ffff00ff");
            AmbientOcclusion = new AmbientOcclusionModel();

            BlendModeModel = new ComboBoxModel<BlendModeEnum>(BlendModeEnum.Normal);
            MaskChannelModel = new ComboBoxModel<MaskChannel>(MaskChannel.Alpha);
            MaskModeModel = new ComboBoxModel<MaskMode>(MaskMode.AllBelow);

            TextureModifierManager = new ModifierManagerModel();

            ModelEntityManager = new PrefabManagerModel();
            CameraEntityManager = new PrefabManagerModel();
            LightEntityManager = new PrefabManagerModel();
        }

        public LayerModel(Guid id) : this()
        {
            ID = id;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public SliderModel Opacity { get; set; }
        public ToggleButtonModel Visibility { get; set; }

        public ModifierManagerModel TextureModifierManager { get; set; }
        public ColorSelectorModel BackgroundColor { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }

        public ComboBoxModel<BlendModeEnum> BlendModeModel { get; internal set; }
        public ComboBoxModel<MaskMode> MaskModeModel { get; internal set; }
        public ComboBoxModel<MaskChannel> MaskChannelModel { get; internal set; }

        public PrefabManagerModel ModelEntityManager { get; internal set; }
        public PrefabManagerModel CameraEntityManager { get; internal set; }
        public PrefabManagerModel LightEntityManager { get; internal set; }
        public ToggleButtonModel IsMask { get; internal set; }
    }
}
