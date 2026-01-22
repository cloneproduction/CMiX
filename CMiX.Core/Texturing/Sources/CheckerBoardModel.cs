// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Sources
{
    public class CheckerBoardModel : IControlModel, IPrefabModel
    {
        public CheckerBoardModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            FilterManager = new PrefabManagerModel();
            Resolution = new Integer2Model(1024, 1024);
            Transform2D = new Transform2DModel();
            CellCount = new Vector2Model(8.0f, 8.0f);
            ColorA = new GenericValueModel<string>("#FFFFFF");
            ColorB = new GenericValueModel<string>("#000000");
        }

        public Guid ID { get; set; }
        public Integer2Model Resolution { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public Transform2DModel Transform2D { get; set; }
        public Vector2Model CellCount { get; set; }
        public GenericValueModel<string> ColorA { get; set; }
        public GenericValueModel<string> ColorB { get; set; }
        public PrefabManagerModel FilterManager { get; set; }
    }
}
