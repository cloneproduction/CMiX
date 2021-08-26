// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class None : ObservableObject, IControl, IAnimMode
    {
        public None(AnimParameter animParameter, NoneModel noneModel)
        {

        }

        public Guid ID { get; set; }
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get => _IsEnabled;
            set => SetProperty(ref _IsEnabled, value);
        }


        public void UpdateOnBeatTick(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {

        }

        public void UpdateOnGameLoop(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {

        }



        public void SetViewModel(IModel model)
        {
            throw new System.NotImplementedException();
        }

        public IModel GetModel()
        {
            throw new System.NotImplementedException();
        }
    }
}
