// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CMiX.Core.Presentation.ViewModels
{
    public class LFO : ObservableObject, IControl, IAnimMode
    {
        public LFO(LFOModel lfoModel)
        {

        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }


        public void SetCommunicator(Communicator communicator)
        {
            throw new NotImplementedException();
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            throw new NotImplementedException();
        }


        public void UpdateOnBeatTick(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {

        }

        public void UpdateOnGameLoop(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {
            double periodOffset = 1.0 / doubleToAnimate.Length;
            double offset = 0.0;
            double val = 0.0;

            for (int i = 0; i < doubleToAnimate.Length; i++)
            {
                val = period + offset;
                if (val < 0.0)
                    val = 1.0 - (0.0 - val) % (1.0 - 0.0);
                else
                    val = 0.0 + (val - 0.0) % (1.0 - 0.0);

                doubleToAnimate[i] = MathUtils.Map(Easings.Interpolate((float)val, easing.SelectedEasing), 0.0, 1.0, 0.0 - range.Width / 2, 0.0 + range.Width / 2);

                offset += periodOffset;
            }
        }

        public IModel GetModel()
        {
            LFOModel model = new LFOModel();

            return model;
        }

        public void SetViewModel(IModel model)
        {
            LFOModel LFOModel = model as LFOModel;
        }
    }
}
