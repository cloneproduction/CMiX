﻿using CMiX.MVVM.Interfaces;
using System;

namespace CMiX.MVVM.Models
{
    [Serializable]
    public class BeatModifierModel : IModel
    {
        public BeatModifierModel()
        {
            ChanceToHit = new SliderModel { Amount = 1.0 };
            Multiplier = 1.0;
        }

        public bool Enabled { get; set; }
        public double Multiplier { get; set; }
        public SliderModel ChanceToHit { get; set; }
    }
}