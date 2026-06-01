// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Ascii : ObservableObject, IPrefab, ITextureFilter
    {
        public Ascii(PrefabService prefabService,
                     GenericValue<float> control,
                     GenericValue<bool> grayscale,
                     GenericValue<float> gridSize,
                     Vector2 characterSize,
                     Blend blend)
        {
            PrefabService = prefabService;
            Control = control;
            Grayscale = grayscale;
            GridSize = gridSize;
            CharacterSize = characterSize;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<float> GridSize { get; set; }
        public Vector2 CharacterSize { get; set; }
        public GenericValue<bool> Grayscale { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new AsciiModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            GridSize = (GenericValueModel<float>)GridSize.ToModel(),
            CharacterSize = (Vector2Model)CharacterSize.ToModel(),
            Grayscale = (GenericValueModel<bool>)Grayscale.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (AsciiModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            GridSize.FromModel(m.GridSize);
            CharacterSize.FromModel(m.CharacterSize);
            Grayscale.FromModel(m.Grayscale);
            Control.FromModel(m.Control);
            Blend.FromModel(m.Blend);
        }
    }
}
