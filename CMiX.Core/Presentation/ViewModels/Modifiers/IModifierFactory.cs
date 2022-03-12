// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels.Modifiers
{
    public interface IModifierFactory : IBeatable
    {
        IModifier Create(Type modifierType);
        IModifier Create(IModifierModel modifierModel);
    }
}
