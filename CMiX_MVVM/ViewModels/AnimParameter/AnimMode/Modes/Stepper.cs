﻿using CMiX.MVVM.Models;
using CMiX.MVVM.Tools;
using CMiX.MVVM.Services;
using CMiX.MVVM.ViewModels.Mediator;
using CMiX.MVVM.ViewModels.MessageService.Messages;
using CMiX.MVVM.Interfaces;

namespace CMiX.MVVM.ViewModels
{
    public class Stepper : Sender, IAnimMode
    {
        public Stepper(string name, IMessageProcessor parentSender) : base (name, parentSender)
        {
            StepCount = 2;
            nextStep = 0.0;
        }


        private double nextStep;

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                SetAndNotify(ref _width, value);
                this.MessageDispatcher.NotifyOut(new MessageUpdateViewModel(this.GetAddress(), this.GetModel()));
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
                SetAndNotify(ref _stepCount, value);
                this.MessageDispatcher.NotifyOut(new MessageUpdateViewModel(this.GetAddress(), this.GetModel()));
            }
        }

        public void UpdateOnBeatTick(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {
            if(beatModifier.CheckHitOnBeatTick())
                nextStep += 1.0;

            if (nextStep >= StepCount)
                nextStep = 0.0;
        }

        private double stepDistance = 0.0;
        private double position = 0.0;

        public void UpdateOnGameLoop(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {
            stepDistance = range.Width / (doubleToAnimate.Length - 1);
            position = 0.0 - (range.Width / 2);

            for (int i = 0; i < doubleToAnimate.Length; i++)
            {
                doubleToAnimate[i] = position + Utils.Map(nextStep, 0, StepCount - 1, 0.0 - range.Width / 2.0, 0.0 + range.Width / 2.0);

                position += stepDistance;
            }
        }

        public override void SetViewModel(IModel model)
        {
            StepperModel stepperModel = model as StepperModel;
            this.StepCount = stepperModel.StepCount;
        }

        public override IModel GetModel()
        {
            StepperModel model = new StepperModel();
            model.StepCount = this.StepCount;
            return model;
        }
    }
}