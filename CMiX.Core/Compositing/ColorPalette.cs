// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Compositing
{
    public class ColorPalette : IPrefab, IModifiable
    {
        public ColorPalette(PrefabService prefabService,
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
        public PrefabManager ColorManager { get; set; }
        public PrefabManager ModifierManager { get; set; }

        public GenericValue<ResamplingMethod> Resample { get; set; }
    }
}
