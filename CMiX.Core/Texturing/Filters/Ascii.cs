// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public class Ascii : ObservableObject, ITextureModifier, IPrefab
    {
        public Ascii(PrefabService prefabService,
                     GenericValue<float> control,
                     GenericValue<bool> grayscale,
                     GenericValue<float> gridSize,
                     Vector2 characterSize)
        {
            PrefabService = prefabService;
            Control = control;
            Grayscale = grayscale;
            GridSize = gridSize;
            CharacterSize = characterSize;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<float> GridSize { get; set; }
        public Vector2 CharacterSize { get; set; }
        public GenericValue<bool> Grayscale { get; set; }
    }
}
