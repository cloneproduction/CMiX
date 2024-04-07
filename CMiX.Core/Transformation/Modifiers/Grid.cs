// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class Grid : ObservableObject, IControl, IModifier, ISpreadableModifier
    {
        public Grid(GenericValue<bool> visible,
                    Vector3 width,
                    Vector3 phase,
                    Integer3 count,
                    ModifierModeSelector modifierModeSelector)
        {
            Visible = visible;
            Width = width;
            Phase = phase;
            Count = count;
            ModifierModeSelector = modifierModeSelector;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Vector3 Width { get; set; }
        public Vector3 Phase { get; set; }
        public Integer3 Count { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;
    }
}
