// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraSettings
    {
        public CameraSettings() 
        {
            Distance = new FloatValue(10f);
            FOV = new FloatValue(0.09f);
            Yaw = new FloatValue(0.0f);
            Pitch = new FloatValue(0.0f);
            Target = new Vector3();
            NearClip = new FloatValue(0.05f);
            FarClip = new FloatValue(100f);
            Projection = new BooleanValue();
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
