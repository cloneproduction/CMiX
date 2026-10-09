// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            TextureSelector = textureSelector;
            Offset = offset;
            OffsetScale = offsetScale;

            Bindables = new List<ModulatableValue<float>>
            {
                offset.X, offset.Y,
                offsetScale.X, offsetScale.Y
            };
        }

        public PrefabManager TextureSelector { get; set; }
        public ModulatableVector2 Offset { get; }
        public ModulatableVector2 OffsetScale { get; }

        public override IControlModel ToModel()
        {
            var model = new DisplaceModel
            {
                TextureSelector = (PrefabManagerModel)TextureSelector.ToModel(),
                Offset = (ModulatableVector2Model)Offset.ToModel(),
                OffsetScale = (ModulatableVector2Model)OffsetScale.ToModel()
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
        public override void Dispose()
        {
            base.Dispose();
            DisposeAll(TextureSelector);
        }
    }
}
