// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Materials;

namespace CMiX.Core.Mapping
{
    public class MaterialPairProfile : ControlPairProfile
    {
        public MaterialPairProfile()
        {
            CreatePair<Material, MaterialModel>();
            CreatePair<MaterialSettings, MaterialSettingsModel>();
        }
    }
}
