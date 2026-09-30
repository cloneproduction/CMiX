// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Ascii : TextureFilterBase
    {
        public Ascii(PrefabService prefabService,
                     GenericValue<float> control,
                     GenericValue<bool> grayscale,
                     ModulatableValue<float> gridSize,
                     ModulatableValue<float> characterSizeX,
                     ModulatableValue<float> characterSizeY,
                     Blend blend,
                     PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Grayscale = grayscale;

            Bindables = new List<ModulatableValue<float>> { gridSize, characterSizeX, characterSizeY };

            gridSize.Label = "Grid Size";
            characterSizeX.Label = "X";
            characterSizeY.Label = "Y";
            gridSize.SetDefault(0.66f);
            characterSizeX.SetDefault(16.0f);
            characterSizeY.SetDefault(16.0f);
        }

        public ModulatableValue<float> GridSize => Bindables[0];
        public ModulatableValue<float> X => Bindables[1];
        public ModulatableValue<float> Y => Bindables[2];
        public GenericValue<bool> Grayscale { get; set; }

        public override IControlModel ToModel()
        {
            var model = new AsciiModel
            {
                Grayscale = (GenericValueModel<bool>)Grayscale.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (AsciiModel)model;
            LoadBaseModel(m);
            Grayscale.FromModel(m.Grayscale);
        }
    }
}
