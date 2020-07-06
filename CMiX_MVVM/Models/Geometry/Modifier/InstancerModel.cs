﻿using ColorMine.ColorSpaces;
using System;

namespace CMiX.MVVM.Models
{
    [Serializable]
    public class InstancerModel
    {
        public InstancerModel()
        {
            Transform = new TransformModel();
            Counter = new CounterModel();
            TranslateModifierModel = new XYZModifierModel();
            ScaleModifierModel = new XYZModifierModel();
            RotationModifierModel = new XYZModifierModel();
        }

        public TransformModel Transform { get; set; }
        public CounterModel Counter { get; set; }
        public XYZModifierModel TranslateModifierModel { get; set; }
        public XYZModifierModel ScaleModifierModel { get; set; }
        public XYZModifierModel RotationModifierModel { get; set; }
        public bool NoAspectRatio { get; set; }
    }
}