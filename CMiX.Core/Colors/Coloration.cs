// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Colors
{
    public partial class Coloration : IControl
    {
        public Coloration(PrefabManager colorManager)
        {
            ColorManager = colorManager;
        }
        public IControlModel ToModel() => this.ToModel();
        public Guid ID { get; set; }
        public PrefabManager ColorManager { get; set; }
    }
}
