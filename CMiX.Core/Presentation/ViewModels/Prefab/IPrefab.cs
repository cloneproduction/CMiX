// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public interface IPrefab : IControl, IBeatable
    {
        bool IsSelected { get; set; }
        bool IsRenaming { get; set; }

        string Name { get; set; }
    }
}
