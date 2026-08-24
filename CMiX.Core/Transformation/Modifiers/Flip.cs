// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class Flip : BeatModifiableModifierBase, IModifier
    {
        public Flip(PrefabService prefabService,
                    PrefabManager beatModifierManager,
                    DirectionXYZ directionXYZ)
            : base(prefabService, beatModifierManager)
        {
            DirectionXYZ = directionXYZ;
        }

        public DirectionXYZ DirectionXYZ { get; set; }

        public override IControlModel ToModel()
        {
            var model = new FlipModel { DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel() };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (FlipModel)model;
            LoadBaseModel(m);
            DirectionXYZ.FromModel(m.DirectionXYZ);
        }
    }
}
