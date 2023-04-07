// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Entities.Lights;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class LightEntity : ObservableObject, IEntity, IPrefab
    {
        public LightEntity(LightEntityModel lightEntityModel, CompositionService compositionService)
        {
            ID = lightEntityModel.ID;
            Name = new StringValue(lightEntityModel.Name, compositionService);
            IsRenaming = new BooleanValue(lightEntityModel.IsRenaming, compositionService);
            IsSelected = new BooleanValue(lightEntityModel.IsSelected, compositionService);
            CompositionService = compositionService;

            LightColor = new ColorSelector(lightEntityModel.LightColor, compositionService);
            Position = new Vector3(lightEntityModel.Position, compositionService);
            Target = new Vector3(lightEntityModel.Target, compositionService);
            Radius = new FloatValue(lightEntityModel.Radius, compositionService);
            Angle = new FloatValue(lightEntityModel.Angle, compositionService);
            Softness = new FloatValue(lightEntityModel.Softness, compositionService);
            Intensity = new FloatValue(lightEntityModel.Intensity, compositionService);

            LightTypeSelector = new GenericValue<LightType>(lightEntityModel.LightTypeSelector, compositionService);
            Visibility = new BooleanValue(lightEntityModel.Visibility, compositionService);
            IsSelected = new BooleanValue(lightEntityModel.IsSelected, compositionService);
            Name = new StringValue(lightEntityModel.Name, compositionService);
        }

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
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }


        public void Dispose()
        {
            
        }
    }
}
