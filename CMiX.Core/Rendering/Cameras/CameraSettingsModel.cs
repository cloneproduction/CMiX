// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraSettingsModel : IControlModel
    {
        public CameraSettingsModel()
        {
            ID = Guid.NewGuid();
            Distance = new GenericValueModel<float>(10f);
            FOV = new GenericValueModel<float>(0.09f);
            Yaw = new GenericValueModel<float>(0.0f);
            Pitch = new GenericValueModel<float>(0.0f);
            Target = new Vector3Model();
            NearClip = new GenericValueModel<float>(0.05f);
            FarClip = new GenericValueModel<float>(100f);
            Projection = new GenericValueModel<bool>(false);
            IsOrthographic = new GenericValueModel<bool>(false);
            OrthographicSize = new GenericValueModel<float>(5.0f);

        }
        public Guid ID { get; set; }

        public GenericValueModel<float> FOV { get; set; }
        public GenericValueModel<float> Distance { get; set; }
        public GenericValueModel<float> Yaw { get; set; }
        public GenericValueModel<float> Pitch { get; set; }
        public Vector3Model Target { get; set; }
        public GenericValueModel<float> NearClip { get; set; }
        public GenericValueModel<float> FarClip { get; set; }
        public GenericValueModel<bool> Projection { get; set; }
        public GenericValueModel<bool> IsOrthographic { get; set; }
        public GenericValueModel<float> OrthographicSize { get; set; }
    }
}
