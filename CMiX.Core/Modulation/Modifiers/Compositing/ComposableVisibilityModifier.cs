// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Shows or hides a Layer or a Composition, in addition to its own visibility toggle. Named apart from VisibilityModifier, which acts on entities.
    [ModifierLabel("Visibility")]
    [ModifierPanel(typeof(Layer))]
    [ModifierPanel(typeof(Composition))]
    public partial class ComposableVisibilityModifier : Modifier
    {
        public ComposableVisibilityModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ControlRepository controlRepository,
                            ModulatableValue<bool> visible)
            : base(prefabService, modulatorManager, controlRepository, new IModulatorBindable[] { visible })
        {
            Visible = visible;
        }

        public ModulatableValue<bool> Visible { get; }

        public override IControlModel ToModel()
        {
            var model = new ComposableVisibilityModifierModel
            {
                Visible = (ModulatableValueModel<bool>)Visible.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ComposableVisibilityModifierModel)model;
            LoadBaseModel(m);
            Visible.FromModel(m.Visible);
            ResolveNestedBindables();
        }
    }
}
