// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ModifierFactory : IModifierFactory
    {
        public ModifierFactory(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        private CompositionService CompositionService { get; set; }

        public IModifier Create(Type modifierType)
        {
            if (modifierType == typeof(RandomHSV))
                return CreateRandomHSV();

            if (modifierType == typeof(RandomXYZ))
                return CreateRandomXYZ();

            if (modifierType == typeof(LinearXYZ))
                return CreateLinearXYZ();

            if (modifierType == typeof(LFO))
                return CreateLFO();

            if (modifierType == typeof(RandomScale))
                return CreateRandomScale();

            if (modifierType == typeof(TransformSRT))
                return CreateTransformSRT();

            if (modifierType == typeof(Translate))
                return CreateTranslate();

            if (modifierType == typeof(Scale))
                return CreateScale();

            if (modifierType == typeof(Rotation))
                return CreateRotation();

            if (modifierType == typeof(Stepper))
                return CreateStepper();

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is RandomHSVModel randomHSVModel)
                return CreateRandomHSV(randomHSVModel);

            if (modifierModel is RandomXYZModel randomXYZModel)
                return CreateRandomXYZ(randomXYZModel);

            if (modifierModel is LinearXYZModel linearXYZModel)
                return CreateLinearXYZ(linearXYZModel);

            if (modifierModel is LFOModel lfoModel)
                return CreateLFO(lfoModel);

            if (modifierModel is RandomScaleModel randomScaleModel)
                return CreateRandomScale(randomScaleModel);

            if (modifierModel is TransformSRTModel transformSRTModel)
                return CreateTransformSRT(transformSRTModel);

            if (modifierModel is TranslateModel translateModel)
                return CreateTranslate(translateModel);

            if (modifierModel is ScaleModel scaleModel)
                return CreateScale(scaleModel);

            if (modifierModel is RotationModel rotationModel)
                return CreateRotation(rotationModel);

            if (modifierModel is StepperModel stepperModel)
                return CreateStepper(stepperModel);

            return null;
        }

        private RandomHSV CreateRandomHSV()
        {
            return new RandomHSV(new RandomHSVModel(), CompositionService);
        }

        private RandomHSV CreateRandomHSV(RandomHSVModel randomHSVModel)
        {
            return new RandomHSV(randomHSVModel, CompositionService);
        }



        private RandomUV CreateRandomXY()
        {
            return new RandomUV(new RandomUVModel(), CompositionService);
        }

        private RandomUV CreateRandomXY(RandomUVModel randomXYZModel)
        {
            return new RandomUV(randomXYZModel, CompositionService);
        }

        private Rotation CreateRotation()
        {
            return new Rotation(new RotationModel());
        }

        private Rotation CreateRotation(RotationModel rotationModel)
        {
            return new Rotation(rotationModel);
        }


        private Scale CreateScale()
        {
            return new Scale(new ScaleModel());
        }

        private Scale CreateScale(ScaleModel scaleModel)
        {
            return new Scale(scaleModel);
        }


        private Translate CreateTranslate()
        {
            return new Translate(new TranslateModel());
        }

        private Translate CreateTranslate(TranslateModel translateModel)
        {
            return new Translate(translateModel);
        }

        private TransformSRT CreateTransformSRT()
        {
            return new TransformSRT(new TransformSRTModel());
        }

        private TransformSRT CreateTransformSRT(TransformSRTModel transformSRTModel)
        {
            return new TransformSRT(transformSRTModel);
        }

        private RandomScale CreateRandomScale()
        {
            return new RandomScale(new RandomScaleModel(), CompositionService);
        }

        private RandomScale CreateRandomScale(RandomScaleModel randomScaleModel)
        {
            return new RandomScale(randomScaleModel, CompositionService);
        }


        private RandomXYZ CreateRandomXYZ()
        {
            return new RandomXYZ(new RandomXYZModel(), CompositionService);
        }

        private RandomXYZ CreateRandomXYZ(RandomXYZModel randomXYZModel)
        {
            return new RandomXYZ(randomXYZModel, CompositionService);
        }



        private LFO CreateLFO()
        {
            return new LFO(new LFOModel(), CompositionService);
        }

        private LFO CreateLFO(LFOModel randomXYZModel)
        {
            return new LFO(randomXYZModel, CompositionService);
        }

        private LinearXYZ CreateLinearXYZ()
        {
            return new LinearXYZ(new LinearXYZModel(), CompositionService);
        }

        private LinearXYZ CreateLinearXYZ(LinearXYZModel linearXYZModel)
        {
            return new LinearXYZ(linearXYZModel, CompositionService);
        }


        private Stepper CreateStepper()
        {
            return new Stepper(new StepperModel(), CompositionService);
        }

        private Stepper CreateStepper(StepperModel stepperModel)
        {
            return new Stepper(stepperModel, CompositionService);
        }
    }
}
