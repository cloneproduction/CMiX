// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Entities.Lights;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Mapping
{
    public class LightConfiguration : ControlPairProfile
    {
        public LightConfiguration()
        {
            //controlConfigurator.RegisterMessage<LightType>();
            CreatePair(typeof(LightEntity), typeof(LightEntityModel));
        }
    }
}
