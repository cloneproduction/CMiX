// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class LinearXYZ : ObservableObject, IModifier, ISpreadable
    {
        public LinearXYZ(BooleanValue visible, ModifierModeSelector modifierModeSelector, GenericValue<TransformType> transformType, FloatValue width, FloatValue phase, DirectionXYZ directionXYZ)
        {
            Visible = visible;
            ModifierModeSelector = modifierModeSelector;
            TransformTypeSelector = transformType;
            Width = width;
            Phase = phase;
            DirectionXYZ = directionXYZ;
        }


        public Guid ID { get; set; } = Guid.NewGuid();

        public BooleanValue Visible { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public FloatValue Width { get; set; }
        public FloatValue Phase { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;
    }
}
