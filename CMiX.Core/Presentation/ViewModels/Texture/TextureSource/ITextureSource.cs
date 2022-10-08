// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Presentation.ViewModels.BaseControl;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface ITextureSource : IControl, IDisposable
    {
        Counter ResolutionX { get; set; }
        Counter ResolutionY { get; set; }
    }
}
