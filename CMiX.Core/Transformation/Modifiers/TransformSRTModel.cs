// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;

namespace CMiX.Core.Transformation
{
    public class TransformSRTModel : IPrefabModel, IModifierModel
    {
        public TransformSRTModel()
        {
            ID = Guid.NewGuid();
            Translate = new TranslateModel();
            Scale = new ScaleModel();
            Rotation = new RotationModel();
            Visible = new BooleanValueModel(true);
            Uniform = new FloatValueModel(1.0f);
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public Guid ID { get; set; }
        public FloatValueModel Uniform { get; set; }
        public TranslateModel Translate { get; set; }
        public ScaleModel Scale { get; set; }
        public RotationModel Rotation { get; set; }
        public BooleanValueModel Visible { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; internal set; }
    }
}
