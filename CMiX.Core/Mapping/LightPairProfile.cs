// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Mapping
{
    public class LightPairProfile : ControlPairProfile
    {
        public LightPairProfile()
        {
            CreatePair<LightEntity, LightEntityModel>();
            CreatePair<LightSettings, LightSettingsModel>();
            CreatePair<RandomPosition, RandomPositionModel>();
            CreatePair<RandomHSV, RandomHSVModel>();
        }
    }
}
