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

            Visibility = new BooleanValueModel();
            IsMask = new BooleanValueModel();
            Opacity = new FloatValueModel(1.0f);

            BackgroundColor = new ColorSelectorModel("#ffff00ff");
            AmbientOcclusion = new AmbientOcclusionModel();

            BlendModeModel = new GenericValueModel<BlendModeEnum>(BlendModeEnum.Normal);
            MaskChannelModel = new GenericValueModel<MaskChannel>(MaskChannel.Alpha);
            MaskModeModel = new GenericValueModel<MaskMode>(MaskMode.AllBelow);

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

        public FloatValueModel Opacity { get; set; }
        public BooleanValueModel Visibility { get; set; }

        public ModifierManagerModel TextureModifierManager { get; set; }
        public ColorSelectorModel BackgroundColor { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }

        public GenericValueModel<BlendModeEnum> BlendModeModel { get; internal set; }
        public GenericValueModel<MaskMode> MaskModeModel { get; internal set; }
        public GenericValueModel<MaskChannel> MaskChannelModel { get; internal set; }

        public PrefabManagerModel ModelEntityManager { get; internal set; }
        public PrefabManagerModel CameraEntityManager { get; internal set; }
        public PrefabManagerModel LightEntityManager { get; internal set; }
        public BooleanValueModel IsMask { get; internal set; }
    }
}
