// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Colors.Modifiers
{
    public partial class ColorPalette : ObservableObject, IPrefab, ISpreadableModifier
    {
        public ColorPalette(PrefabService prefabService,
                            ModifierModeSelector modifierModeSelector,
                            PrefabManager colorManager,
                            PrefabManager modifierManager,
                            GenericValue<ResamplingMethod> resample)
        {
            PrefabService = prefabService;
            ColorManager = colorManager;
            ModifierManager = modifierManager;
            Resample = resample;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public PrefabManager ColorManager { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public GenericValue<ResamplingMethod> Resample { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
