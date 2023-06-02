// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public class LayerModel : IPrefabModel
    {
        public LayerModel()
        {
            ID = Guid.NewGuid();
            Name = new StringValueModel("Layer");
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
            Visibility = new BooleanValueModel(false);
            IsMask = new BooleanValueModel(false);
            Opacity = new FloatValueModel(1.0f);
            BackgroundColor = new ColorSelectorModel("#ffff00ff");
            AmbientOcclusion = new AmbientOcclusionModel();
            BlendMode = new GenericValueModel<BlendModeEnum>(BlendModeEnum.Normal);
            MaskChannel = new GenericValueModel<MaskChannel>(Texturing.MaskChannel.Alpha);
            MaskMode = new GenericValueModel<MaskMode>(Texturing.MaskMode.AllBelow);
            ModifierManager = new ModifierManagerModel();
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
        public ModifierManagerModel ModifierManager { get; set; }
        public ColorSelectorModel BackgroundColor { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }
        public GenericValueModel<BlendModeEnum> BlendMode { get; internal set; }
        public GenericValueModel<MaskMode> MaskMode { get; internal set; }
        public GenericValueModel<MaskChannel> MaskChannel { get; internal set; }
        public PrefabManagerModel ModelEntityManager { get; internal set; }
        public PrefabManagerModel CameraEntityManager { get; internal set; }
        public PrefabManagerModel LightEntityManager { get; internal set; }
        public BooleanValueModel IsMask { get; internal set; }
        public BooleanValueModel IsRenaming { get; internal set; }
        public BooleanValueModel IsSelected { get; internal set; }
    }
}
