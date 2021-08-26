// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Media.Media3D;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Randomized : ObservableObject, IControl, IAnimMode
    {
        public Randomized(AnimParameter parentSender, RandomizedModel randomizedModel)
        {
            Random = new Random();
            oldRandom = GetNewRandoms(parentSender.Parameters.Length);
            newRandom = GetNewRandoms(parentSender.Parameters.Length);
        }


        public Guid ID { get; set; }
        private Random Random { get; set; }
        public int Count { get; set; }
        public Vector3D[] Location { get; set; }
        public Vector3D[] Scale { get; set; }
        public Vector3D[] Rotation { get; set; }

        private double[] oldRandom;
        private double[] newRandom;


        private double[] GetNewRandoms(int count)
        {
            var rands = new double[count];

            for (int i = 0; i < count; i++)
                rands[i] = Random.NextDouble();

            return rands;
        }

        public void UpdateOnBeatTick(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {
            oldRandom = newRandom;
            if (beatModifier.CheckHitOnBeatTick())
                newRandom = GetNewRandoms(doubleToAnimate.Length);
        }

        public void UpdateOnGameLoop(double[] doubleToAnimate, double period, IRange range, Easing easing, BeatModifier beatModifier)
        {
            bool ease = easing.IsEnabled;

            for (int i = 0; i < doubleToAnimate.Length; i++)
            {
                if (ease)
                {
                    double eased = Easings.Interpolate((float)period, easing.SelectedEasing);
                    double lerped = MathUtils.Lerp(oldRandom[i], newRandom[i], eased);
                    doubleToAnimate[i] = MathUtils.Map(lerped, 0.0, 1.0, 0.0 - range.Width / 2, 0.0 + range.Width / 2);
                }
                else
                {
                    doubleToAnimate[i] = MathUtils.Map(newRandom[i], 0.0, 1.0, 0.0 - range.Width / 2, 0.0 + range.Width / 2);
                }
            }
        }

        public void SetViewModel(IModel model)
        {
            throw new NotImplementedException();
        }

        public IModel GetModel()
        {
            RandomizedModel model = new RandomizedModel();
            return model;
        }
    }
}
