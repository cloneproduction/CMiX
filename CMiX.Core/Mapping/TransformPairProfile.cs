// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Transformation;

namespace CMiX.Core.Mapping
{
    public class TransformPairProfile : ControlPairProfile
    {
        public TransformPairProfile()
        {
            CreatePair<Scale, ScaleModel>();
            CreatePair<Translate, TranslateModel>();
            CreatePair<Rotation, RotationModel>();
            CreatePair<Transform2D, Transform2DModel>();
            CreatePair<TransformSRT, TransformSRTModel>();
        }
    }
}
