// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public record EntityModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<string> Name { get; set; } = new("Entity");
        public MeshModel Mesh { get; set; } = new();
        public TransformSRTModel TransformSRT { get; set; } = new();
        public MaterialModel Material { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
        public PrefabManagerModel MaterialManager { get; set; } = new();
        public PrefabManagerModel ColorPaletteManager { get; set; } = new();
        public GenericValueModel<bool> IsSelected { get; set; } = new(false);
        public GenericValueModel<bool> IsRenaming { get; set; } = new(false);
        public GenericValueModel<bool> Visibility { get; set; } = new(false);
    }
}
