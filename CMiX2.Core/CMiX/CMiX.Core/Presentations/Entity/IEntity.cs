// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.ViewModels
{
    public interface IEntity : IControl, IDisposable
    {
        bool IsRenaming { get; set; }
        string Name { get; set; }
        bool IsSelected { get; set; }
        BooleanValue Visibility { get; set; }
    }
}
