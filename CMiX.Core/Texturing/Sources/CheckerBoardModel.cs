// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Sources
{
    public record CheckerBoardModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2Model Resolution { get; set; } = new(1024, 1024);
        public PrefabServiceModel PrefabService { get; set; } = new();
        public Transform2DModel Transform2D { get; set; } = new();
        public Vector2Model CellCount { get; set; } = new(8.0f, 8.0f);
        public GenericValueModel<string> ColorA { get; set; } = new("#FFFFFF");
        public GenericValueModel<string> ColorB { get; set; } = new("#000000");
        public PrefabManagerModel TextureModifierManager { get; set; } = new();
    }
}
