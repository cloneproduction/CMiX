// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Mapping;

namespace CMiX.Core.Transformation.Modifiers
{
    public class EntityModifierPairProfile : ControlPairProfile
    {
        public EntityModifierPairProfile()
        {
            CreatePair<TransformSRT, TransformSRTModel>();
            CreatePair<Scale, ScaleModel>();
            CreatePair<Rotation, RotationModel>();
            CreatePair<Translate, TranslateModel>();
            CreatePair<RandomXYZ, RandomXYZModel>();
            CreatePair<GaussianXYZ, GaussianXYZModel>();
            CreatePair<LinearXYZ, LinearXYZModel>();
            CreatePair<LFO, LFOModel>();

            CreatePair<RandomRotation, RandomRotationModel>();
            CreatePair<RandomScale, RandomScaleModel>();

            CreatePair<Stepper, StepperModel>();
            CreatePair<RandomHSV, RandomHSVModel>();
        }
    }
}
