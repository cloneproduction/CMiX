// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Texturing.Sources
{
    public interface IAssetTextureSource : ITextureSource
    {
        AssetSelector AssetSelector { get; set; }
    }
}
