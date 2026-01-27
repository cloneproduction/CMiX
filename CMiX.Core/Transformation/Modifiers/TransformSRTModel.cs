// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public record TransformSRTModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Uniform { get; set; } = new(1.0f);
        public DirectionXYZModel DirectionXYZ { get; set; } = new(false, false, false);
        public TranslateModel Translate { get; set; } = new();
        public ScaleModel Scale { get; set; } = new();
        public RotationModel Rotation { get; set; } = new();
        public GenericValueModel<bool> Visible { get; set; } = new(true);
        public GenericValueModel<ModifierMode> Mode { get; set; } = new(ModifierMode.ToSpread);
    }
}
