// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    // [ModifierPanel] removed - superseded by Modulation.ScaleModifier, which does everything this
    // does plus an optional modulator stack (TransformSRTModifier itself now composes
    // ScaleModifier directly, not this class). No longer independently addable via the picker or
    // used internally by anything - kept standalone in case it's needed again.
    public partial class Scale : ObservableObject, IModifier
    {
        public Scale(GenericValue<float> uniform, 
                     Vector3 xyz, 
                     PrefabService prefabService)
        {
            Uniform = uniform;
            XYZ = xyz;
            PrefabService = prefabService;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public Vector3 XYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public IControlModel ToModel() => new ScaleModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Uniform = (GenericValueModel<float>)Uniform.ToModel(),
            XYZ = (Vector3Model)XYZ.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ScaleModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Uniform.FromModel(m.Uniform);
            XYZ.FromModel(m.XYZ);
        }
    }
}
