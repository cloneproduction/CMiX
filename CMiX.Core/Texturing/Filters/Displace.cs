// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Displace : TextureFilterBase, IDisposable
    {
        public Displace(PrefabService prefabService,
                        PrefabManager textureSelector,
                        Vector2 offset,
                        Vector2 offsetScale,
                        GenericValue<float> control,
                        Blend blend)
            : base(prefabService, control, blend)
        {
            Offset = offset;
            OffsetScale = offsetScale;
            TextureSelector = textureSelector;
        }

        public PrefabManager TextureSelector { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 OffsetScale { get; set; }

        public override IControlModel ToModel()
        {
            var model = new DisplaceModel
            {
                TextureSelector = (PrefabManagerModel)TextureSelector.ToModel(),
                Offset = (Vector2Model)Offset.ToModel(),
                OffsetScale = (Vector2Model)OffsetScale.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (DisplaceModel)model;
            LoadBaseModel(m);
            Offset.FromModel(m.Offset);
            OffsetScale.FromModel(m.OffsetScale);

            LoadManager(TextureSelector, m.TextureSelector);
        }

        // The texture this filter displaces with is reachable through this selector alone, so a
        // filter torn down without disposing it leaves its repository and its deleter
        // registrations behind.
        public void Dispose()
        {
            TextureSelector.Dispose();
        }
    }
}
