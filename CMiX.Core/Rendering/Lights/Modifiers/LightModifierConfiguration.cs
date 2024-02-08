// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Mapping;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Rendering.Lights.Modifiers
{
    public class LightModifierConfiguration : ControlPairProfile
    {
        public LightModifierConfiguration()
        {
            CreatePair<RandomPosition, RandomPositionModel>();
            CreatePair<RandomHSV, RandomHSVModel>();
        }
    }
}
