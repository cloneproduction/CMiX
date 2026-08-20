// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public record ProjectModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel PrefabManager { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
        public PrefabManagerModel CompositionManager {  get; set; } = new();
        public MasterBeatModel MasterBeat { get; set; } = new();
        public OutputMappingManagerModel OutputMappingManager { get; set; } = new();

    }
}
