// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Compositing
{
    public record class ColorModel : GenericValueModel<string>, IPrefabModel
    {
        public ColorModel()
        {
            Value = "#FFFFFFFF";
        }
        public PrefabServiceModel PrefabService { get; init; } = new();
    }
}
