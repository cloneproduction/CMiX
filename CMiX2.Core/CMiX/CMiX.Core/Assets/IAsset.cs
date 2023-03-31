// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public interface IAsset
    {
        bool FileExist { get; set; }
        string Name { get; set; }
        string Ponderation { get; set; }
        bool IsRenaming { get; set; }
        bool IsSelected { get; set; }
        string Path { get; set; }

        IModel GetModel();
    }
}
