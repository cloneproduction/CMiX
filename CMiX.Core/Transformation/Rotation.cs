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
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class Rotation : ObservableObject, IControl, IModifier
    {
        public Rotation(PrefabService prefabService, Vector3 xyz)
        {
            PrefabService = prefabService;
            XYZ = xyz;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Vector3 XYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public IControlModel ToModel() => new RotationModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            XYZ = (Vector3Model)XYZ.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (RotationModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            XYZ.FromModel(m.XYZ);
        }
    }
}
