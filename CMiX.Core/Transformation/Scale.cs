// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class Scale : ObservableObject, ISpreadableModifier, IPrefab
    {
        public Scale(ModifierModeSelector modifierModeSelector, 
                     GenericValue<float> uniform, 
                     Vector3 xyz, 
                     PrefabService prefabService)
        {
            ModifierModeSelector = modifierModeSelector;
            Uniform = uniform;
            XYZ = xyz;
            PrefabService = prefabService;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public Vector3 XYZ { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
