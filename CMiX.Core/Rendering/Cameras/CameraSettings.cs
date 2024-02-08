// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraSettings
    {
        public CameraSettings(GenericValue<float> distance,
                                GenericValue<float> fov,
                                GenericValue<float> yaw,
                                GenericValue<float> pitch,
                                Vector3 target,
                                GenericValue<float> nearClip,
                                GenericValue<float> farClip,
                                GenericValue<bool> projection) 
        {
            Distance = distance; // new GenericValue<float>(10f);
            FOV = fov; // new GenericValue<float>(0.09f);
            Yaw = yaw; // new GenericValue<float>(0.0f);
            Pitch = pitch; // new GenericValue<float>(0.0f);
            Target = target; // new Vector3();
            NearClip = nearClip; // new GenericValue<float>(0.05f);
            FarClip = farClip; // new GenericValue<float>(100f);
            Projection = projection; // new GenericValue<bool>();
        }

        public GenericValue<float> FOV { get; set; }
        public GenericValue<float> Distance { get; set; }
        public GenericValue<float> Yaw { get; set; }
        public GenericValue<float> Pitch { get; set; }
        public Vector3 Target { get; set; }
        public GenericValue<float> NearClip { get; set; }
        public GenericValue<float> FarClip { get; set; }
        public GenericValue<bool> Projection { get; set; }
    }
}
