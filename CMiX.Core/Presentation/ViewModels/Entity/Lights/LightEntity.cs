// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels
{
    public class LightEntity : ObservableObject, IEntity, IPrefab
    {
        public LightEntity(LightEntityModel lightEntityModel, CompositionService compositionService)
        {
            ID = lightEntityModel.ID;
            Name = this.GetType().Name;
            IsRenaming = false;
            CompositionService = compositionService;

            LightColor = new ColorSelector(lightEntityModel.LightColor);
            Position = new VectorXYZ(lightEntityModel.Position);
            Target = new VectorXYZ(lightEntityModel.Target);
            Radius = new Slider(nameof(Radius), lightEntityModel.Radius);
            Angle = new Slider(nameof(Angle), lightEntityModel.Angle);
            Softness = new Slider(nameof(Softness), lightEntityModel.Softness);
            Intensity = new Slider(nameof(Intensity), lightEntityModel.Intensity);

            LightTypeSelector = new ComboBox<LightType>(lightEntityModel.LightTypeSelector);
            Visibility = new ToggleButton(lightEntityModel.Visibility);

            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);
        }

        public ICommand OpenColorSelectorCommand { get; set; }

        public Guid ID { get; set; }

        public CompositionService CompositionService { get; set; }
        public ComboBox<LightType> LightTypeSelector { get; set; }
        public ColorSelector LightColor { get; set; }
        public VectorXYZ Position { get; set; }
        public VectorXYZ Target { get; set; }
        public Slider Radius { get; set; }
        public Slider Angle { get; set; }
        public Slider Softness { get; set; }
        public Slider Intensity { get; set; }
        public ToggleButton Visibility { get; set; }


        public void OpenColorSelector()
        {
            CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.LightColor);
        }


        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        public IModel GetModel()
        {
            LightEntityModel lightEntityModel = new LightEntityModel();

            lightEntityModel.Name = Name;
            lightEntityModel.ID = ID;

            lightEntityModel.LightColor = (ColorSelectorModel)LightColor.GetModel();
            lightEntityModel.Position = (VectorXYZModel)Position.GetModel();
            lightEntityModel.Target = (VectorXYZModel)Target.GetModel();
            lightEntityModel.Radius = (SliderModel)Radius.GetModel();
            lightEntityModel.Angle = (SliderModel)Angle.GetModel();
            lightEntityModel.Softness = (SliderModel)Softness.GetModel();
            lightEntityModel.Intensity = (SliderModel)Intensity.GetModel();
            lightEntityModel.LightTypeSelector = (ComboBoxModel<LightType>)LightTypeSelector.GetModel();
            lightEntityModel.Visibility = (ToggleButtonModel)Visibility.GetModel();

            return lightEntityModel;
        }


        public void SetViewModel(IModel model)
        {
            LightEntityModel lightEntityModel = model as LightEntityModel;

            Name = lightEntityModel.Name;
            ID = lightEntityModel.ID;

            LightColor.SetViewModel(lightEntityModel.LightColor);
            Position.SetViewModel(lightEntityModel.Position);
            Target.SetViewModel(lightEntityModel.Target);
            Radius.SetViewModel(lightEntityModel.Radius);
            Angle.SetViewModel(lightEntityModel.Angle);
            Softness.SetViewModel(lightEntityModel.Softness);
            Intensity.SetViewModel(lightEntityModel.Intensity);
            LightTypeSelector.SetViewModel(lightEntityModel.LightTypeSelector);
            Visibility.SetViewModel(lightEntityModel.Visibility);
        }

        public void Dispose()
        {
            
        }
    }
}
