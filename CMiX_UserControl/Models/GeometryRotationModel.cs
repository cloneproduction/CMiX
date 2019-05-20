﻿using CMiX.ViewModels;
using System;

namespace CMiX.Models
{
    [Serializable]
    public class GeometryRotationModel
    {
        public GeometryRotationMode RotationModeDTO { get; set; }
        public bool RotationX { get; set; }
        public bool RotationY { get; set; }
        public bool RotationZ { get; set; }
    }
}