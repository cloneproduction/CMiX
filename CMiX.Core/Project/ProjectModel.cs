// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public record ProjectModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel CompositionManager {  get; set; } = new();
        public MasterBeatModel MasterBeat { get; set; } = new();
        public OutputMappingManagerModel OutputMappingManager { get; set; } = new();
        public AssetSelectorModel Model { get; set; } = new();

    }
}
