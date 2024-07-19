// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class AsciiModel : IPrefabModel
    {
        public AsciiModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Control = new GenericValueModel<float>(1.0f);
            GridSize = new GenericValueModel<float>(0.66f);
            CharacterSize = new Vector2Model(16.0f, 16.0f);
            Grayscale = new GenericValueModel<bool>(false);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> GridSize { get; set; }
        public Vector2Model CharacterSize { get; set; }
        public GenericValueModel<bool> Grayscale { get; set; }
        public GenericValueModel<float> Control { get; set; }
    }
}
