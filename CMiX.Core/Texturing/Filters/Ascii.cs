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
                     GenericValue<bool> grayscale,
                     ModulatableValue<float> gridSize,
                     ModulatableValue<float> characterSizeX,
                     ModulatableValue<float> characterSizeY,
                     Blend blend,
                     PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Grayscale = grayscale;

            Bindables = new List<ModulatableValue<float>> { gridSize, characterSizeX, characterSizeY };
        }

        public ModulatableValue<float> GridSize => Bindables[0];
        public ModulatableValue<float> X => Bindables[1];
        public ModulatableValue<float> Y => Bindables[2];
        public GenericValue<bool> Grayscale { get; set; }

        public override IControlModel ToModel()
        {
            var model = new AsciiModel
            {
                Grayscale = (GenericValueModel<bool>)Grayscale.ToModel(),
                GridSize = (ModulatableValueModel<float>)GridSize.ToModel(),
                X = (ModulatableValueModel<float>)X.ToModel(),
                Y = (ModulatableValueModel<float>)Y.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (AsciiModel)model;
            LoadBaseModel(m);
            GridSize.FromModel(m.GridSize);
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Grayscale.FromModel(m.Grayscale);
        }
    }
}
