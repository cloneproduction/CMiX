﻿using CMiX.MVVM.Models;

namespace CMiX.MVVM.ViewModels
{
    public static class AnimParameterModelFactory
    {
        public static AnimParameterModel GetModel(this AnimParameter instance)
        {
            AnimParameterModel model = new AnimParameterModel();

            //model.IsEnabled = instance.IsEnabled;
            //model.Name = instance.Name;
            //model.SelectedModeType = instance.SelectedModeType;
            //model.Width = instance.Width.GetModel();
            //model.EasingModel = instance.Easing.GetModel();
            //model.BeatModifierModel = instance.BeatModifier.GetModel();
            //model.AnimModeModel = instance.AnimMode.GetModel();

            return model;
        }

        public static void SetViewModel(this AnimParameter instance, AnimParameterModel model)
        {
            instance.SelectedModeType = model.SelectedModeType;
            instance.Name = model.Name;
            instance.IsEnabled = model.IsEnabled;
            instance.Width.SetViewModel(model.Width);
            instance.Easing.SetViewModel(model.EasingModel);
            instance.BeatModifier.SetViewModel(model.BeatModifierModel);
            instance.AnimMode.SetViewModel(model.AnimModeModel);
        }
    }
}