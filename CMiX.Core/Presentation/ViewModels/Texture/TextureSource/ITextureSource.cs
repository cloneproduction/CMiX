// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface ITextureSource : IControl, IDisposable
    {
        Integer2 Resolution { get; set; }
    }
}
