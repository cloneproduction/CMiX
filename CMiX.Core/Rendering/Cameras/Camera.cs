// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras
{
    public partial class Camera : ObservableObject, IPrefab, IModifiable
    {
        public Camera()
        {
            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();
            Name = new StringValue();
            FOV = new FloatValue();
            Distance = new FloatValue();
            Yaw = new FloatValue();
            Pitch = new FloatValue();
            Target = new Vector3();
            NearClip = new FloatValue();
            FarClip = new FloatValue();
            Projection = new BooleanValue();
            ModifierManager = new ModifierManager(new CameraTransformModifierFactory());
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ModifierManager ModifierManager { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public FloatValue FOV { get; set; }
        public FloatValue Distance { get; set; }
        public FloatValue Yaw { get; set; }
        public FloatValue Pitch { get; set; }
        public Vector3 Target { get; set; }
        public FloatValue NearClip { get; set; }
        public FloatValue FarClip { get; set; }
        public BooleanValue Projection { get; set; }

        [ObservableProperty]
        bool isExpanded = false;
    }
}
