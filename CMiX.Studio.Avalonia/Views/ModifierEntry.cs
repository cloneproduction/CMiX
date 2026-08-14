// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Prefabs;

namespace CMiX.Studio.Avalonia.Views
{
    public class ModifierEntry
    {
        public Type Type { get; set; }
        public string Label => ControlFactory.StringHelper.PascalCaseToDisplay(Type?.Name ?? "");
    }
}
