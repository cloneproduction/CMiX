// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Modulation.Modulators
{
    public abstract partial class ModulatorBase : ReceivableControl, IModulator
    {
        protected ModulatorBase(PrefabService prefabService)
        {
            PrefabService = prefabService;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Guid CompositionID { get; set; }

        [ObservableProperty]
        private bool isHovered;

        [ObservableProperty]
        private bool isExpanded = true;

        public abstract IReadOnlyList<IModulatorOutput> Outputs { get; }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
