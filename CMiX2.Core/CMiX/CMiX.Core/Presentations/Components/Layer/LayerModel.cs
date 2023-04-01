// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.PostFX;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.Modifiers;

namespace CMiX.Core.Presentations.Components
{
    public class LayerModel : IPrefabModel
    {
        public LayerModel()
        {
            ID = Guid.NewGuid();
            Name = new StringValueModel("Layer");
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);

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
        public StringValueModel Name { get; set; }

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
        public BooleanValueModel IsRenaming { get; internal set; }
        public BooleanValueModel IsSelected { get; internal set; }
    }
}
