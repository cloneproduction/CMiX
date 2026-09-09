// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modulation;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing
{
    public record TextureTexCoordModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public List<ModulatableValueModel<float>> Bindables { get; set; } = new();
    }
}
