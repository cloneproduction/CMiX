// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public interface ITransformModifier : IModifier
    {
        GenericValue<ModifierMode> Mode { get; set; }
    }
}
