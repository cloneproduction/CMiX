// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Colors.Modifiers
{
    public class ColorizeModel : IControlModel
    {
        public ColorizeModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            ColorPaletteManager = new PrefabManagerModel();
        }
        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel ColorPaletteManager { get; set; }
    }
}
