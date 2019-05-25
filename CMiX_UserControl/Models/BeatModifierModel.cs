﻿using CMiX.Services;
using System;

namespace CMiX.Models
{
    [Serializable]
    public class BeatModifierModel
    {
        public BeatModifierModel()
        {
            ChanceToHit = new SliderModel { Amount = 1.0 };
            Multiplier = 1.0;
        }

        public string MessageAddress { get; set; }

        [OSC]
        public double Multiplier { get; set; }

        public SliderModel ChanceToHit { get; set; }
    }
}