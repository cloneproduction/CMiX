// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Observer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class AnimParameter : ObservableObject, IControl, IObserver
    {
        public AnimParameter(string name, double[] defaultParameter, MasterBeat beat, AnimParameterModel animParameterModel)
        {
            Period = new double[0];
            Range = new Range(animParameterModel.RangeModel);
            Easing = new Easing(animParameterModel.EasingModel);
            Width = new Slider(nameof(Width), animParameterModel.Width);
            BeatModifier = new BeatModifier(beat, animParameterModel.BeatModifierModel);
            Parameters = defaultParameter;
            Name = name;
            SelectedModeType = ModeType.None;
        }



        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public IRange Range { get; set; }
        public Slider Width { get; set; }

        public double[] Parameters { get; set; }
        public double[] Period { get; set; }
        public double DefaultValue { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get => _IsEnabled;
            set => SetProperty(ref _IsEnabled, value);
        }

        private ModeType _selectedModeType;
        public ModeType SelectedModeType
        {
            get => _selectedModeType;
            set
            {
                SetProperty(ref _selectedModeType, value);
                SetAnimMode();
            }
        }

        private IAnimMode _animMode;
        public IAnimMode AnimMode
        {
            get => _animMode;
            set
            {
                SetProperty(ref _animMode, value);
            }
        }

        public Communicator Communicator { get; set; }
        public System.Guid ID { get; set; }

        private void SetAnimMode()
        {
            ParametersToDefault();
            //if (this.AnimMode != null)
            //    this.AnimMode.Dispose();
            this.AnimMode = ModesFactory.CreateMode(SelectedModeType, this);
        }

        public void AnimateOnBeatTick()
        {
            var index = BeatModifier.BeatIndex;
            if (index >= 0 && index < Period.Length)
                this.AnimMode.UpdateOnBeatTick(this.Parameters, Period[BeatModifier.BeatIndex], Range, this.Easing, this.BeatModifier);
        }

        public void AnimateOnGameLoop(double[] parameters)
        {
            this.AnimMode.UpdateOnGameLoop(parameters, Period[BeatModifier.BeatIndex], Range, this.Easing, this.BeatModifier);
        }

        public void Update(int count)
        {
            //this.Parameters = new double[count];
            ParametersToDefault();
            AnimateOnBeatTick();
        }

        private void ParametersToDefault()
        {
            for (int i = 0; i < Parameters.Length; i++)
            {
                this.Parameters[i] = DefaultValue;
            }
        }

        public void SetViewModel(IModel model)
        {
            AnimParameterModel animParameterModel = model as AnimParameterModel;
            this.SelectedModeType = animParameterModel.SelectedModeType;
            this.Name = animParameterModel.Name;
            this.IsEnabled = animParameterModel.IsEnabled;
            this.Width.SetViewModel(animParameterModel.Width);
            this.Easing.SetViewModel(animParameterModel.EasingModel);
            this.BeatModifier.SetViewModel(animParameterModel.BeatModifierModel);
            this.AnimMode.SetViewModel(animParameterModel.AnimModeModel);
        }

        public IModel GetModel()
        {
            AnimParameterModel model = new AnimParameterModel();

            model.IsEnabled = this.IsEnabled;
            model.Name = this.Name;
            model.SelectedModeType = this.SelectedModeType;
            model.Width = (SliderModel)this.Width.GetModel();
            model.EasingModel = (EasingModel)this.Easing.GetModel();
            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.AnimModeModel = (AnimModeModel)this.AnimMode.GetModel();

            return model;
        }

        public void SetCommunicator(Communicator communicator)
        {
            throw new System.NotImplementedException();
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            throw new System.NotImplementedException();
        }
    }
}
