// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Beat;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public class LinearXYZModel : IModifierModel
    {
        public LinearXYZModel()
        {
            ID = Guid.NewGuid();
            Name = TransformModifierNames.LinearXYZ;

            Visible = new BooleanValueModel(true);
            BeatModifierModel = new BeatModifierModel();
            CounterModel = new IntegerValueModel(1);
            Width = new FloatValueModel();
            Phase = new FloatValueModel();
            DirectionXYZModel = new DirectionXYZModel();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.PerInstance);
            Mode.Value = ModifierMode.ToSpread;
            TransformTypeSelector = new GenericValueModel<TransformType>(TransformType.Translate);
        }

        public TransformModifierNames Name { get; set; }
        public Guid ID { get; set; }

        public BooleanValueModel Visible { get; set; }
        public BeatModifierModel BeatModifierModel { get; set; }
        public IntegerValueModel CounterModel { get; set; }
        public FloatValueModel Width { get; set; }
        public DirectionXYZModel DirectionXYZModel { get; set; }
        public FloatValueModel Phase { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; internal set; }
        public GenericValueModel<TransformType> TransformTypeSelector { get; internal set; }
    }
}
