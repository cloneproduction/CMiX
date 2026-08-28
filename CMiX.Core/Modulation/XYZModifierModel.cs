// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    public record XYZModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public List<ChannelModel> Channels { get; set; } = new();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new();
        public GenericValueModel<bool> Gaussian { get; set; } = new(false);
        public GenericValueModel<bool> RandomizeLocation { get; set; } = new(true);
        public GenericValueModel<bool> RandomizeScale { get; set; } = new(true);
        public GenericValueModel<bool> RandomizeRotation { get; set; } = new(true);
    }
}
