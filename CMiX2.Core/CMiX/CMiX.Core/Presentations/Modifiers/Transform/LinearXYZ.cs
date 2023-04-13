// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public partial class LinearXYZ : ObservableObject, IControl, ITransformModifier, IDisposable
    {
        public LinearXYZ(LinearXYZModel linearXYZModel, CompositionService compositionService)
        {
            ID = linearXYZModel.ID;
            Name = linearXYZModel.Name;
            Mode = new GenericValue<ModifierMode>(linearXYZModel.Mode, compositionService);
            Visible = new BooleanValue(linearXYZModel.Visible, compositionService);
            Width = new FloatValue(linearXYZModel.Width, compositionService);
            Phase = new FloatValue(linearXYZModel.Phase, compositionService);
            Counter = new IntegerValue(linearXYZModel.CounterModel, compositionService);
            TransformTypeSelector = new GenericValue<TransformType>(linearXYZModel.TransformTypeSelector, compositionService);
            DirectionXYZ = new DirectionXYZ(linearXYZModel.DirectionXYZModel, compositionService);
            BeatModifier = new BeatModifier(linearXYZModel.BeatModifierModel, compositionService);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public TransformModifierNames Name { get; set; }
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
