// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Modifiers.Transform
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
