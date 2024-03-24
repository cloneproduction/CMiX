// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public class LayerModel : IControlModel, IPrefabModel
    {
        public LayerModel()
        {
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;

            Name = new GenericValueModel<string>("Layer");
            IsSelected = new GenericValueModel<bool>(false);
            IsRenaming = new GenericValueModel<bool>(false);
            Visibility = new GenericValueModel<bool>(false);

            IsMask = new GenericValueModel<bool>(false);
            Opacity = new GenericValueModel<float>(1.0f);
            BackgroundColor = new GenericValueModel<string>("#ff333333");
            AmbientOcclusion = new AmbientOcclusionModel();
            BlendMode = new GenericValueModel<BlendModeEnum>(Texturing.BlendModeEnum.Normal);
            MaskChannel = new GenericValueModel<MaskChannel>(Texturing.MaskChannel.Alpha);
            MaskMode = new GenericValueModel<MaskMode>(Texturing.MaskMode.AllBelow);
            Invert = new GenericValueModel<bool>(false);
            ModifierManager = new PrefabManagerModel();
            ModelEntityManager = new PrefabManagerModel();
            LayerModifierManager = new PrefabManagerModel();
        }


        public Guid ID { get; set; }

        public PrefabServiceModel PrefabService { get; set; }

        public GenericValueModel<string> Name { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }


        public GenericValueModel<bool> Invert { get; set; }
        public GenericValueModel<bool> IsMask { get; set; }
        public GenericValueModel<MaskMode> MaskMode { get; set; }

        public PrefabManagerModel ModifierManager { get; set; }
        public PrefabManagerModel LayerModifierManager { get; set; }
        public GenericValueModel<string> BackgroundColor { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }
        public GenericValueModel<BlendModeEnum> BlendMode { get; set; }
  
        public GenericValueModel<MaskChannel> MaskChannel { get; set; }
        public PrefabManagerModel ModelEntityManager { get; set; }

        public GenericValueModel<float> Opacity { get; set; }
    }
}
