// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public interface IModifier : IControl, IDisposable
    {
        BooleanValue Visible { get; set; }
    }
}
