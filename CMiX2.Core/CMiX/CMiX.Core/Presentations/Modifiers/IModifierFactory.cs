// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentations.Modifiers;

namespace CMiX.Core.Presentations.ViewModels.Modifiers
{
    public interface IModifierFactory
    {
        IModifier Create(Type modifierType);
        IModifier Create(IModifierModel modifierModel);
        IModifierModel CreateModel(IModifier modifier);
    }
}
