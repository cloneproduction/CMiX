// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class GridModel : IModifierModel
    {
        public GridModel()
        {
            ID = Guid.NewGuid();
            Count = new Integer3Model(1, 1, 1);
            Width = new Vector3Model(0, 0, 0);
            Phase = new Vector3Model(0, 0, 0);
            ModifierModeSelector = new ModifierModeSelectorModel();
            Visible = new GenericValueModel<bool>(true);
        }

        public Guid ID { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
        public Vector3Model Width { get; set; }
        public Vector3Model Phase { get; set; }
        public Integer3Model Count { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
    }
}
