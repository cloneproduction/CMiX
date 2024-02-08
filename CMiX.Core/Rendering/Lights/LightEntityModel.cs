// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Entities.Lights
{
    public class LightEntityModel : IPrefabModel
    {
        public LightEntityModel()
        {
            
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;
            LightColor = new GenericValueModel<string>();
            Position = new Vector3Model(0.0f, 2.0f, 0.0f);
            Target = new Vector3Model(0.001f, 0.0f, 0.0f);
            Radius = new GenericValueModel<float>(5.0f);
            Angle = new GenericValueModel<float>(0.25f);
            Softness = new GenericValueModel<float>(0.01f);
            Intensity = new GenericValueModel<float>(1.0f);
            LightTypeSelector = new GenericValueModel<LightType>(LightType.AmbientLight);
            Visibility = new GenericValueModel<bool>();
            IsSelected = new GenericValueModel<bool>(false);
            Name = new GenericValueModel<string>("Light " + ID.ToString());
            IsRenaming = new GenericValueModel<bool>(false);
            ModifierManager = new PrefabManagerModel();
        }

        public Guid ID { get; set; }

        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<string> Name { get; set; }

        public GenericValueModel<string> LightColor { get; set; }
        public Vector3Model Position { get; set; }
        public Vector3Model Target { get; set; }
        public GenericValueModel<float> Radius { get; set; }
        public GenericValueModel<float> Angle { get; set; }
        public GenericValueModel<float> Softness { get; set; }
        public GenericValueModel<float> Intensity { get; set; }
        public GenericValueModel<LightType> LightTypeSelector { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
        public PrefabManagerModel ModifierManager { get; internal set; }
    }
}
