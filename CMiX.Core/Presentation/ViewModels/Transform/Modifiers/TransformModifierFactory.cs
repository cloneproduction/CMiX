// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformModifierFactory : IBeatable, IModifierFactory
    {
        public TransformModifierFactory()
        {

        }

        private MasterBeat MasterBeat { get; set; }

        public IModifier Create(Type modifierType)
        {
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

            if (modifierType == typeof(RandomXY))
                return CreateRandomXY();

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if(modifierModel is RandomXYZModel randomXYZModel)
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

            if (modifierModel is RandomXYModel randomXYModel)
                return CreateRandomXY(randomXYModel);

            return null;
        }



        public void SetMasterBeat(MasterBeat masterBeat)
        {
            MasterBeat = masterBeat;
        }


        private RandomXY CreateRandomXY()
        {
            var randomized = new RandomXY(new RandomXYModel());
            randomized.SetMasterBeat(MasterBeat);
            return randomized;
        }

        private RandomXY CreateRandomXY(RandomXYModel randomXYZModel)
        {
            var randomized = new RandomXY(randomXYZModel);
            randomized.SetMasterBeat(MasterBeat);
            return randomized;
        }

        private Rotation CreateRotation()
        {
            var rotation = new Rotation(new RotationModel());
            //transform.SetMasterBeat(MasterBeat);
            return rotation;
        }

        private Rotation CreateRotation(RotationModel rotationModel)
        {
            var rotation = new Rotation(rotationModel);
            //transform.SetMasterBeat(MasterBeat);
            return rotation;
        }


        private Scale CreateScale()
        {
            var scale = new Scale(new ScaleModel());
            //transform.SetMasterBeat(MasterBeat);
            return scale;
        }

        private Scale CreateScale(ScaleModel scaleModel)
        {
            var scale = new Scale(scaleModel);
            //transform.SetMasterBeat(MasterBeat);
            return scale;
        }


        private Translate CreateTranslate()
        {
            var translate = new Translate(new TranslateModel());
            //transform.SetMasterBeat(MasterBeat);
            return translate;
        }

        private Translate CreateTranslate(TranslateModel translateModel)
        {
            var translate = new Translate(translateModel);
            //transform.SetMasterBeat(MasterBeat);
            return translate;
        }

        private TransformSRT CreateTransformSRT()
        {
            var transform = new TransformSRT(new TransformSRTModel());
            //transform.SetMasterBeat(MasterBeat);
            return transform;
        }

        private TransformSRT CreateTransformSRT(TransformSRTModel transformSRTModel)
        {
            var transformSRT = new TransformSRT(transformSRTModel);
            //transform.SetMasterBeat(MasterBeat);
            return transformSRT;
        }

        private RandomScale CreateRandomScale()
        {
            var randomScale = new RandomScale(new RandomScaleModel());
            randomScale.SetMasterBeat(MasterBeat);
            return randomScale;
        }

        private RandomScale CreateRandomScale(RandomScaleModel randomScaleModel)
        {
            var randomScale = new RandomScale(randomScaleModel);
            randomScale.SetMasterBeat(MasterBeat);
            return randomScale;
        }


        private RandomXYZ CreateRandomXYZ()
        {
            var randomized = new RandomXYZ(new RandomXYZModel());
            randomized.SetMasterBeat(MasterBeat);
            return randomized;
        }

        private RandomXYZ CreateRandomXYZ(RandomXYZModel randomXYZModel)
        {
            var randomized = new RandomXYZ(randomXYZModel);
            randomized.SetMasterBeat(MasterBeat);
            return randomized;
        }



        private LFO CreateLFO()
        {
            var lfo = new LFO(new LFOModel());
            lfo.SetMasterBeat(MasterBeat);
            return lfo;
        }

        private LFO CreateLFO(LFOModel randomXYZModel)
        {
            var lfo = new LFO(randomXYZModel);
            lfo.SetMasterBeat(MasterBeat);
            return lfo;
        }



        private LinearXYZ CreateLinearXYZ()
        {
            var linear = new LinearXYZ(new LinearXYZModel());
            linear.SetMasterBeat(MasterBeat);
            return linear;
        }

        private LinearXYZ CreateLinearXYZ(LinearXYZModel linearXYZModel)
        {
            var linear = new LinearXYZ(linearXYZModel);
            linear.SetMasterBeat(MasterBeat);
            return linear;
        }
    }
}
