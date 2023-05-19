// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Assets
{
    public interface IAssetModel : IModel
    {
        bool FileExist { get; set; }
        string Name { get; set; }
        bool IsSelected { get; set; }
        string FilePath { get; set; }
    }
}
