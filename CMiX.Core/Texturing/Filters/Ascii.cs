// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Ascii : TextureFilterBase
    {
        public Ascii(PrefabService prefabService,
                     GenericValue<float> control,
                     GenericValue<bool> grayscale,
                     GenericValue<float> gridSize,
                     Vector2 characterSize,
                     Blend blend)
            : base(prefabService, control, blend)
        {
            Grayscale = grayscale;
            GridSize = gridSize;
            CharacterSize = characterSize;
        }

        public GenericValue<float> GridSize { get; set; }
        public Vector2 CharacterSize { get; set; }
        public GenericValue<bool> Grayscale { get; set; }

        public override IControlModel ToModel()
        {
            var model = new AsciiModel
            {
                GridSize = (GenericValueModel<float>)GridSize.ToModel(),
                CharacterSize = (Vector2Model)CharacterSize.ToModel(),
                Grayscale = (GenericValueModel<bool>)Grayscale.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (AsciiModel)model;
            LoadBaseModel(m);
            GridSize.FromModel(m.GridSize);
            CharacterSize.FromModel(m.CharacterSize);
            Grayscale.FromModel(m.Grayscale);
        }
    }
}
