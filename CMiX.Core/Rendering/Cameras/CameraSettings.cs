// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraSettings : IControl
    {
        public CameraSettings(GenericValue<float> distance,
                              GenericValue<float> fov,
                              GenericValue<float> yaw,
                              GenericValue<float> pitch,
                              Vector3 target,
                              GenericValue<float> nearClip,
                              GenericValue<float> farClip,
                              GenericValue<bool> projection,
                              GenericValue<bool> isOrthographic,
                              GenericValue<float> orthographicSize) 
        {
            Distance = distance;
            FOV = fov;
            Yaw = yaw;
            Pitch = pitch;
            Target = target;
            NearClip = nearClip;
            FarClip = farClip;
            Projection = projection;
            IsOrthographic = isOrthographic;
            OrthographicSize = orthographicSize;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> FOV { get; set; }
        public GenericValue<float> Distance { get; set; }
        public GenericValue<float> Yaw { get; set; }
        public GenericValue<float> Pitch { get; set; }
        public Vector3 Target { get; set; }
        public GenericValue<float> NearClip { get; set; }
        public GenericValue<float> FarClip { get; set; }
        public GenericValue<bool> Projection { get; set; }
        public GenericValue<bool> IsOrthographic { get; set; }
        public GenericValue<float> OrthographicSize { get; set; }

        public IControlModel ToModel() => new CameraSettingsModel
        {
            ID = ID,
            FOV = (GenericValueModel<float>)FOV.ToModel(),
            Distance = (GenericValueModel<float>)Distance.ToModel(),
            Yaw = (GenericValueModel<float>)Yaw.ToModel(),
            Pitch = (GenericValueModel<float>)Pitch.ToModel(),
            Target = (Vector3Model)Target.ToModel(),
            NearClip = (GenericValueModel<float>)NearClip.ToModel(),
            FarClip = (GenericValueModel<float>)FarClip.ToModel(),
            Projection = (GenericValueModel<bool>)Projection.ToModel(),
            IsOrthographic = (GenericValueModel<bool>)IsOrthographic.ToModel(),
            OrthographicSize = (GenericValueModel<float>)OrthographicSize.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CameraSettingsModel)model;
            ID = m.ID;
            FOV.FromModel(m.FOV);
            Distance.FromModel(m.Distance);
            Yaw.FromModel(m.Yaw);
            Pitch.FromModel(m.Pitch);
            Target.FromModel(m.Target);
            NearClip.FromModel(m.NearClip);
            FarClip.FromModel(m.FarClip);
            Projection.FromModel(m.Projection);
            IsOrthographic.FromModel(m.IsOrthographic);
            OrthographicSize.FromModel(m.OrthographicSize);
        }
    }
}
