// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class TextureFilterFactory : IModifierFactory
    {
        public TextureFilterFactory()
        {

        }

        public IModifier Create(Type modifierType)
        {
            if (modifierType == typeof(HSCB))
                return ControlMessenger.Mapper.Map<HSCB>(new HSCBModel());

            if (modifierType == typeof(Invert))
                return ControlMessenger.Mapper.Map<Invert>(new InvertModel());

            if (modifierType == typeof(Blur))
                return ControlMessenger.Mapper.Map<Blur>(new BlurModel());

            if (modifierType == typeof(Edge))
                return ControlMessenger.Mapper.Map<Edge>(new EdgeModel());

            if (modifierType == typeof(TransformTexture))
                return ControlMessenger.Mapper.Map<TransformTexture>(new TransformTextureModel());

            if (modifierType == typeof(Pixelate))
                return ControlMessenger.Mapper.Map<Pixelate>(new PixelateModel());

            if (modifierType == typeof(Echo))
                return ControlMessenger.Mapper.Map<Echo>(new EchoModel());

            if (modifierType == typeof(Feedback))
                return ControlMessenger.Mapper.Map<Feedback>(new FeedbackModel());

            if (modifierType == typeof(TriColor))
                return ControlMessenger.Mapper.Map<TriColor>(new TriColorModel());

            if (modifierType == typeof(RandomUV))
                return ControlMessenger.Mapper.Map<RandomUV>(new RandomUVModel());

            if (modifierType == typeof(LFOUV))
                return ControlMessenger.Mapper.Map<LFOUV>(new LFOUVModel());

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is HSCBModel hSCBModel)
                return ControlMessenger.Mapper.Map<HSCB>(hSCBModel);

            if (modifierModel is InvertModel invertModel)
                return ControlMessenger.Mapper.Map<Invert>(invertModel);

            if (modifierModel is BlurModel blurModel)
                return ControlMessenger.Mapper.Map<Blur>(blurModel);

            if (modifierModel is EdgeModel edgeModel)
                return ControlMessenger.Mapper.Map<Edge>(edgeModel);

            if (modifierModel is TransformTextureModel modelTexture)
                return ControlMessenger.Mapper.Map<TransformTexture>(modelTexture);

            if (modifierModel is PixelateModel pixelateModel)
                return ControlMessenger.Mapper.Map<Pixelate>(pixelateModel);

            if (modifierModel is EchoModel echoModel)
                return ControlMessenger.Mapper.Map<Echo>(echoModel);

            if (modifierModel is FeedbackModel feedbackModel)
                return ControlMessenger.Mapper.Map<Feedback>(feedbackModel);

            if (modifierModel is TriColorModel triColorModel)
                return ControlMessenger.Mapper.Map<TriColor>(triColorModel);

            if (modifierModel is RandomUVModel randomUVModel)
                return ControlMessenger.Mapper.Map<RandomUV>(randomUVModel);

            if (modifierModel is LFOUVModel lfoUVModel)
                return ControlMessenger.Mapper.Map<LFOUV>(lfoUVModel);

            return null;
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            var type = modifier.GetType();

            if (type == typeof(HSCB))
                return ControlMessenger.Mapper.Map<HSCBModel>(modifier);

            if (type == typeof(Invert))
                return ControlMessenger.Mapper.Map<InvertModel>(modifier);

            if (type == typeof(Blur))
                return ControlMessenger.Mapper.Map<BlurModel>(modifier);

            if (type == typeof(Edge))
                return ControlMessenger.Mapper.Map<EdgeModel>(modifier);

            if (type == typeof(TransformTexture))
                return ControlMessenger.Mapper.Map<TransformTextureModel>(modifier);

            if (type == typeof(Pixelate))
                return ControlMessenger.Mapper.Map<PixelateModel>(modifier);

            if (type == typeof(Echo))
                return ControlMessenger.Mapper.Map<EchoModel>(modifier);

            if (type == typeof(Feedback))
                return ControlMessenger.Mapper.Map<FeedbackModel>(modifier);

            if (type == typeof(TriColor))
                return ControlMessenger.Mapper.Map<TriColorModel>(modifier);

            if (type == typeof(RandomUV))
                return ControlMessenger.Mapper.Map<RandomUVModel>(modifier);

            if (type == typeof(LFOUV))
                return ControlMessenger.Mapper.Map<LFOUVModel>(modifier);

            return null;
        }
    }
}
