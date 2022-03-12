// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Stepper : ObservableObject, IControl, IAnimMode
    {
        public Stepper(StepperModel stepperModel)
        {
            this.ID = stepperModel.ID;
            StepCount = 2;
            nextStep = 0.0;
        }


        public Guid ID { get; set; }

        private double nextStep;

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                SetProperty(ref _width, value);

            }
        }

        private int _stepCount;
        public int StepCount
        {
            get => _stepCount;
            set
            {
                if (value <= 1)
                    value = 1;
                SetProperty(ref _stepCount, value);

            }
        }

        public void UpdateOnBeatTick(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {
            //if (beatModifier.CheckHitOnBeatTick())
            //    nextStep += 1.0;

            //if (nextStep >= StepCount)
            //    nextStep = 0.0;
        }

        private double stepDistance = 0.0;
        private double position = 0.0;

        public void UpdateOnGameLoop(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {
            stepDistance = range.Width / (doubleToAnimate.Length - 1);
            position = 0.0 - (range.Width / 2);

            for (int i = 0; i < doubleToAnimate.Length; i++)
            {
                doubleToAnimate[i] = position + MathUtils.Map(nextStep, 0, StepCount - 1, 0.0 - range.Width / 2.0, 0.0 + range.Width / 2.0);

                position += stepDistance;
            }
        }


        public void SetViewModel(IModel model)
        {
            StepperModel stepperModel = model as StepperModel;
            this.ID = stepperModel.ID;
            this.StepCount = stepperModel.StepCount;
        }

        public IModel GetModel()
        {
            StepperModel model = new StepperModel();
            model.ID = this.ID;
            model.StepCount = this.StepCount;
            return model;
        }
    }
}
