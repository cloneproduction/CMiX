// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface IEntity : IControl, IDisposable
    {
        bool IsRenaming { get; set; }
        string Name { get; set; }
        bool IsSelected { get; set; }
        ToggleButton Visibility { get; set; }
    }
}
