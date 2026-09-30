// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Displace : TextureFilterBase
    {
        public Displace(PrefabService prefabService,
                        PrefabManager textureSelector,
                        ModulatableVector2 offset,
                        ModulatableVector2 offsetScale,
                        GenericValue<float> control,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            TextureSelector = textureSelector;
            Offset = offset;
            OffsetScale = offsetScale;

            Bindables = new List<ModulatableValue<float>>
            {
                offset.X, offset.Y,
                offsetScale.X, offsetScale.Y
            };

            offset.X.SetDefault(0.5f);
            offset.Y.SetDefault(0.5f);
            offsetScale.X.SetDefault(0.1f);
            offsetScale.Y.SetDefault(0.1f);
        }

        public PrefabManager TextureSelector { get; set; }
        public ModulatableVector2 Offset { get; }
        public ModulatableVector2 OffsetScale { get; }

        public override IControlModel ToModel()
        {
            var model = new DisplaceModel
            {
                TextureSelector = (PrefabManagerModel)TextureSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (DisplaceModel)model;
            LoadBaseModel(m);

            LoadManager(TextureSelector, m.TextureSelector);
        }

        // The texture this filter displaces with is reachable through this selector alone, so a
        // filter torn down without disposing it leaves its repository and its deleter
        // registrations behind.
        public override void Dispose()
        {
            base.Dispose();
            DisposeAll(TextureSelector);
        }
    }
}
