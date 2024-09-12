// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Compositing
{
    public class ColorPalette : IPrefab
    {
        public ColorPalette(PrefabService prefabService,
                            PrefabManager colorManager)
        {
            PrefabService = prefabService;
        }


        public PrefabManager ColorManager { get; set; }
        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }
    }
}
