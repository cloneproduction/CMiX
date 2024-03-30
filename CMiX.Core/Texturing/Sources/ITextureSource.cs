// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.ViewModels
{
    public interface ITextureSource : IControl
    {
        Integer2 Resolution { get; set; }
        PrefabManager FilterManager { get; set; }
    }
}
