// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras
{
    public partial class Camera : ObservableObject, IPrefab, IModifiable
    {
        public Camera(CompositionService compositionService)
        {
            IsSelected = new BooleanValue(false);
            IsRenaming = new BooleanValue(false);
            Name = new StringValue("Camera");
            FOV = new FloatValue(0.09f);
            Distance = new FloatValue(10f);
            Yaw = new FloatValue(0.0f);
            Pitch = new FloatValue(0.0f);
            Target = new Vector3();
            NearClip = new FloatValue(0.05f);
            FarClip = new FloatValue(100f);
            Projection = new BooleanValue();
            Visibility = new BooleanValue(false);

            ModifierManager = compositionService.GetModifierManager<Camera>();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visibility { get; set; }
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
