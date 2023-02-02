// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

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
            Position = new Vector3(lightEntityModel.Position);
            Target = new Vector3(lightEntityModel.Target);
            Radius = new FloatValue(lightEntityModel.Radius);
            Angle = new FloatValue(lightEntityModel.Angle);
            Softness = new FloatValue(lightEntityModel.Softness);
            Intensity = new FloatValue(lightEntityModel.Intensity);

            LightTypeSelector = new GenericValue<LightType>(lightEntityModel.LightTypeSelector);
            Visibility = new BooleanValue(lightEntityModel.Visibility);
        }

        public ICommand OpenColorSelectorCommand { get; set; }

        public Guid ID { get; set; }

        public CompositionService CompositionService { get; set; }
        public GenericValue<LightType> LightTypeSelector { get; set; }
        public ColorSelector LightColor { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public FloatValue Radius { get; set; }
        public FloatValue Angle { get; set; }
        public FloatValue Softness { get; set; }
        public FloatValue Intensity { get; set; }
        public BooleanValue Visibility { get; set; }



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
            lightEntityModel.Position = (Vector3Model)Position.GetModel();
            lightEntityModel.Target = (Vector3Model)Target.GetModel();
            lightEntityModel.Radius = (FloatValueModel)Radius.GetModel();
            lightEntityModel.Angle = (FloatValueModel)Angle.GetModel();
            lightEntityModel.Softness = (FloatValueModel)Softness.GetModel();
            lightEntityModel.Intensity = (FloatValueModel)Intensity.GetModel();
            lightEntityModel.LightTypeSelector = (GenericValueModel<LightType>)LightTypeSelector.GetModel();
            lightEntityModel.Visibility = (BooleanValueModel)Visibility.GetModel();

            return lightEntityModel;
        }

        public void Dispose()
        {
            
        }
    }
}
