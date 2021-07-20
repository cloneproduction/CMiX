// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Easing : ObservableObject, IControl
    {
        public Easing(EasingModel easingModel)
        {
            this.ID = easingModel.ID;
            EasingMode = EasingMode.EaseIn;
            EasingFunction = EasingFunction.None;
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }


        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                SetProperty(ref _isEnabled, value);

            }
        }

        private EasingFunction _easingFunction;
        public EasingFunction EasingFunction
        {
            get => _easingFunction;
            set
            {
                SetProperty(ref _easingFunction, value);
                SetEasing();
            }
        }

        private EasingMode _easingMode;
        public EasingMode EasingMode
        {
            get => _easingMode;
            set
            {
                SetProperty(ref _easingMode, value);
                SetEasing();
            }
        }

        private Easings.Functions _selectedEasing;
        public Easings.Functions SelectedEasing
        {
            get => _selectedEasing;
            set => _selectedEasing = value;
        }


        private void SetEasing()
        {
            Easings.Functions myStatus;

            if (EasingFunction == EasingFunction.None)
                Enum.TryParse(EasingFunction.ToString(), out myStatus);
            else
                Enum.TryParse(EasingFunction.ToString() + EasingMode.ToString(), out myStatus);

            SelectedEasing = myStatus;

        }

        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public void SetViewModel(IModel model)
        {
            EasingModel easingModel = model as EasingModel;
            this.ID = easingModel.ID;
            this.IsEnabled = easingModel.IsEnabled;
            this.EasingFunction = easingModel.EasingFunction;
            this.EasingMode = easingModel.EasingMode;
            this.SelectedEasing = easingModel.SelectedEasing;
        }

        public IModel GetModel()
        {
            EasingModel model = new EasingModel();
            model.ID = this.ID;
            model.IsEnabled = this.IsEnabled;
            model.EasingFunction = this.EasingFunction;
            model.EasingMode = this.EasingMode;
            model.SelectedEasing = this.SelectedEasing;
            return model;
        }
    }
}
