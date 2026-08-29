// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    // [ModifierPanel] removed - superseded by Modulation.TranslateModifier, which does everything
    // this does plus an optional modulator stack. No longer independently addable via the picker;
    // kept as TransformSRTModifier's internal building block and so already-saved Project data
    // referencing Translate still loads.
    public partial class Translate : ObservableObject, IModifier
    {
        public Translate(PrefabService prefabService, Vector3 xyz)
        {
            PrefabService = prefabService;
            XYZ = xyz;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Vector3 XYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new TranslateModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            XYZ = (Vector3Model)XYZ.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TranslateModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            XYZ.FromModel(m.XYZ);
        }
    }
}
