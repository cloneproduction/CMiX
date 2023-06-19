// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class LinearXYZ : ObservableObject, IControl, ITransformModifier, IDisposable, ISpreadable
    {
        public LinearXYZ(LinearXYZModel linearXYZModel)
        {
            ID = linearXYZModel.ID;
            Mode = new GenericValue<ModifierMode>(linearXYZModel.Mode);
            Visible = new BooleanValue(linearXYZModel.Visible);
            Width = new FloatValue(linearXYZModel.Width);
            Phase = new FloatValue(linearXYZModel.Phase);
            Counter = new IntegerValue(linearXYZModel.Counter);
            TransformTypeSelector = new GenericValue<TransformType>(linearXYZModel.TransformTypeSelector);
            DirectionXYZ = new DirectionXYZ(linearXYZModel.DirectionXYZ);
            BeatModifier = new BeatModifier(linearXYZModel.BeatModifier);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public IntegerValue Counter { get; set; }
        public FloatValue Width { get; set; }
        public FloatValue Phase { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }


        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
