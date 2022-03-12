// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface ITextureFilter : IModifier, IDisposable
    {
        TextureFilterName Name { get; set; }
        bool Enabled { get; set; }
        ToggleButton Visible { get; set; }
    }
}
