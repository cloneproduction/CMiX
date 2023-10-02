// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Modifiers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Rendering.Lights.Modifiers
{
    public class LightModifierFactory : ModifierFactory
    {
        public LightModifierFactory()
        {
            TypePairs.Add(typeof(RandomPosition), typeof(RandomPositionModel));
            TypePairs.Add(typeof(RandomHSV), typeof(RandomHSVModel));
        }
    }
}
