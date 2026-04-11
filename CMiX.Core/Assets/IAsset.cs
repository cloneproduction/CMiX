// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets;

namespace CMiX.Core.ViewModels.Assets
{
    public interface IAsset
    {
        bool FileExist { get; }
        string Name { get; }
        bool IsSelected { get; set; }
        string FilePath { get; set; }
    }
}
