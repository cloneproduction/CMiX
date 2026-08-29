// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    // [ModifierPanel] removed - superseded by Modulation.VisibilityModifier. No longer addable via
    // the picker; kept so already-saved Project data referencing RandomVisibility still loads.
    public partial class RandomVisibility : BeatModifiableModifierBase, IModifier
    {
        public RandomVisibility(GenericValue<float> control,
                                PrefabService prefabService,
                                PrefabManager beatModifierManager)
            : base(prefabService, beatModifierManager)
        {
            Control = control;
        }

        public GenericValue<float> Control { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomVisibilityModel
            {
                Control = (GenericValueModel<float>)Control.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomVisibilityModel)model;
            LoadBaseModel(m);
            Control.FromModel(m.Control);
        }
    }
}
