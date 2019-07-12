﻿//using CMiX.Services;
using CMiX.MVVM.Models;
using CMiX.MVVM.ViewModels;
using System;

namespace CMiX.MVVM.Models
{
    [Serializable]
    public class GeometryTranslateModel : Model
    {
        public GeometryTranslateModel()
        {

        }

        //[OSC]
        public GeometryTranslateMode Mode { get; set; }
    }
}