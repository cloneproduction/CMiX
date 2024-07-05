// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class CircularSpread : ObservableObject, IPrefab, ISpreadableModifier
    {
        public CircularSpread(PrefabService prefabService, 
                              ModifierModeSelector modifierModeSelector, 
                              Vector2 width, 
                              GenericValue<float> phase)
        {
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            Width = width;
            Phase = phase;
        }


        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Vector2 Width { get; set; }
        public GenericValue<float> Phase { get; set; }
        public GenericValue<float> Factor { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;
    }
}
