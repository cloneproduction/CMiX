// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Colors.Modifiers
{
    public partial class Colorize : ObservableObject, IPrefab
    {
        public Colorize(PrefabService prefabService,
                        PrefabManager colorPaletteManager)
        {
            PrefabService = prefabService;
            ColorPaletteManager = colorPaletteManager;
        }

        public PrefabService PrefabService { get; set; }
        public PrefabManager ColorPaletteManager { get; set; }
        public Guid ID { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
