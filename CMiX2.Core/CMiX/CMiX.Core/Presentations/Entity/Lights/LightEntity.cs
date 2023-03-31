// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Entities.Lights;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class LightEntity : ObservableObject, IEntity, IPrefab
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


        [ObservableProperty]
        private bool isRenaming;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isSelected;


        public void Dispose()
        {
            
        }
    }
}
