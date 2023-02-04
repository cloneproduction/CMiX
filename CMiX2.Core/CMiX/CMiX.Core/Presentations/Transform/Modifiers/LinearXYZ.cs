// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class LinearXYZ : ObservableObject, IControl, ITransformModifier, IDisposable
    {
        public LinearXYZ(LinearXYZModel linearXYZModel, CompositionService compositionService)
        {
            this.ID = linearXYZModel.ID;
            this.Name = linearXYZModel.Name;
            Mode = new GenericValue<ModifierMode>(linearXYZModel.Mode);

            Visible = new BooleanValue(linearXYZModel.Visible);
            Width = new FloatValue(linearXYZModel.Width);
            Phase = new FloatValue(linearXYZModel.Phase);
            Counter = new IntegerValue(linearXYZModel.CounterModel);
            TransformTypeSelector = new GenericValue<TransformType>(linearXYZModel.TransformTypeSelector);
            DirectionXYZ = new DirectionXYZ(linearXYZModel.DirectionXYZModel);
            BeatModifier = new BeatModifier(linearXYZModel.BeatModifierModel, compositionService);

            IsExpanded = true;
        }

        public BooleanValue Visible { get; set; }

        public Guid ID { get; set; }
        public TransformModifierNames Name { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public IntegerValue Counter { get; set; }
        public FloatValue Width { get; set; }
        public FloatValue Phase { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }


        private ModifierMode _selectedModifierType;
        public ModifierMode SelectedModifierType
        {
            get => _selectedModifierType;
            set => SetProperty(ref _selectedModifierType, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
