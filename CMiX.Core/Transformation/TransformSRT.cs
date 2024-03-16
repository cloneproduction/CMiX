// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class TransformSRT : ObservableObject, IPrefab, IControl, IModifier
    {
        public TransformSRT(PrefabService prefabService, GenericValue<float> uniform, Translate translate, Scale scale, Rotation rotation, GenericValue<bool> visible, GenericValue<ModifierMode> mode)
        {
            PrefabService = prefabService;
            Uniform = uniform;
            Translate = translate;
            Scale = scale;
            Rotation = rotation;
            Visible = visible;
            Mode = mode;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Uniform { get; set; }
        public Translate Translate { get; set; }
        public Scale Scale { get; set; }
        public Rotation Rotation { get; set; }
        public GenericValue<bool> Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
