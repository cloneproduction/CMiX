// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraSettings
    {
        public CameraSettings(FloatValue distance,
                                FloatValue fov,
                                FloatValue yaw,
                                FloatValue pitch,
                                Vector3 target,
                                FloatValue nearClip,
                                FloatValue farClip,
                                BooleanValue projection) 
        {
            Distance = distance; // new FloatValue(10f);
            FOV = fov; // new FloatValue(0.09f);
            Yaw = yaw; // new FloatValue(0.0f);
            Pitch = pitch; // new FloatValue(0.0f);
            Target = target; // new Vector3();
            NearClip = nearClip; // new FloatValue(0.05f);
            FarClip = farClip; // new FloatValue(100f);
            Projection = projection; // new BooleanValue();
        }

        public FloatValue FOV { get; set; }
        public FloatValue Distance { get; set; }
        public FloatValue Yaw { get; set; }
        public FloatValue Pitch { get; set; }
        public Vector3 Target { get; set; }
        public FloatValue NearClip { get; set; }
        public FloatValue FarClip { get; set; }
        public BooleanValue Projection { get; set; }
    }
}
