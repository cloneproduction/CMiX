// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControls;

namespace CMiX.Core.ViewModels
{
    public interface ITextureSource : IControl, IDisposable
    {
        Integer2 Resolution { get; set; }
    }
}
