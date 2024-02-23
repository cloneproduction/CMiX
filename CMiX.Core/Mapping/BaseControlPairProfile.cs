// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Mapping
{
    public class BaseControlPairProfile : ControlPairProfile
    {
        public BaseControlPairProfile()
        {
            CreatePair(typeof(GenericValue<>), typeof(GenericValueModel<>));
            CreatePair<Button, ButtonModel>();
            CreatePair<Integer2, Integer2Model>();
            CreatePair<Vector2, Vector2Model>();
            CreatePair<Vector3, Vector3Model>();
            CreatePair<DirectionXYZ, DirectionXYZModel>();
        }
    }
}
