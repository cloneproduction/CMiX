// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Colors.Modifiers
{
    public record ColorPaletteModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel ColorManager { get; set; } = new();
        public GenericValueModel<ResamplingMethod> Resample { get; set; } = new(ResamplingMethod.Linear);
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
    }
}
