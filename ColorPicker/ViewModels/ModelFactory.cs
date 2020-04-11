﻿using CMiX.MVVM.Models;
using CMiX.MVVM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.ColorPicker.ViewModels
{
    public static class ModelFactory
    {
        public static ColorPickerModel GetModel(this ColorPicker instance)
        {
            ColorPickerModel colorPickerModel = new ColorPickerModel();
            colorPickerModel.SelectedColor = Utils.ColorToHexString(instance.SelectedColor);
            return colorPickerModel;
        }
    }
}
