// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Text.Modifiers
{
    // [ModifierPanel] removed - superseded by Modulation.CharWriterModifier. No longer addable via
    // the picker; kept so already-saved Project data referencing CharWriter still loads.
    public partial class CharWriter : BeatModifiableModifierBase, IModifier
    {
        public CharWriter(PrefabService prefabService,
                          PrefabManager beatModifierManager)
            : base(prefabService, beatModifierManager)
        {
        }

        public override IControlModel ToModel()
        {
            var model = new CharWriterModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CharWriterModel)model;
            LoadBaseModel(m);
        }
    }
}
