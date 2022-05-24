// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class LinearXYZ : ObservableObject, IControl, ITransformModifier, IBeatable, IDisposable
    {
        public LinearXYZ(LinearXYZModel linearXYZModel)
        {
            this.ID = linearXYZModel.ID;
            this.Name = linearXYZModel.Name;
            Mode = new ComboBox<ModifierMode>(linearXYZModel.Mode);

            Visible = new ToggleButton(linearXYZModel.Visible);
            Width = new Slider(nameof(Width), linearXYZModel.Width);
            Phase = new Slider(name: nameof(Phase), linearXYZModel.Phase);
            Counter = new Counter(linearXYZModel.CounterModel);
            TransformTypeSelector = new ComboBox<TransformType>(linearXYZModel.TransformTypeSelector);
            DirectionXYZ = new DirectionXYZ(linearXYZModel.DirectionXYZModel);
            BeatModifier = new BeatModifier(linearXYZModel.BeatModifierModel);
        }

        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }

        public Guid ID { get; set; }
        public TransformModifierNames Name { get; set; }
        public ComboBox<ModifierMode> Mode { get; set; }
        public ComboBox<TransformType> TransformTypeSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Counter Counter { get; set; }
        public Slider Width { get; set; }
        public Slider Phase { get; set; }
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


        public IModel GetModel()
        {
            LinearXYZModel linearXYZModel = new LinearXYZModel();

            linearXYZModel.Name = Name;
            linearXYZModel.ID = ID;
            linearXYZModel.Visible = (ToggleButtonModel)Visible.GetModel();

            linearXYZModel.Width = (SliderModel)Width.GetModel();
            linearXYZModel.CounterModel = (CounterModel)Counter.GetModel();
            linearXYZModel.BeatModifierModel = (BeatModifierModel)BeatModifier.GetModel();
            linearXYZModel.DirectionXYZModel = (DirectionXYZModel)DirectionXYZ.GetModel();
            linearXYZModel.Phase = (SliderModel)Phase.GetModel();
            linearXYZModel.Mode =(ComboBoxModel<ModifierMode>)Mode.GetModel();
            linearXYZModel.TransformTypeSelector = (ComboBoxModel<TransformType>)TransformTypeSelector.GetModel();

            return linearXYZModel;
        }

        public void SetViewModel(IModel model)
        {
            LinearXYZModel linearXYZModel = model as LinearXYZModel;

            this.Name = linearXYZModel.Name;
            this.ID = linearXYZModel.ID;

            this.Visible.SetViewModel(linearXYZModel.Visible);
            this.Width.SetViewModel(linearXYZModel.Width);
            this.Counter.SetViewModel(linearXYZModel.CounterModel);
            this.BeatModifier.SetViewModel(linearXYZModel.BeatModifierModel);
            this.DirectionXYZ.SetViewModel(linearXYZModel.DirectionXYZModel);
            this.Phase.SetViewModel(linearXYZModel.Phase);
            this.Mode.SetViewModel(linearXYZModel.Mode);
            this.TransformTypeSelector.SetViewModel(linearXYZModel.TransformTypeSelector);
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            BeatModifier.SetMasterBeat(masterBeat);
        }

        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
