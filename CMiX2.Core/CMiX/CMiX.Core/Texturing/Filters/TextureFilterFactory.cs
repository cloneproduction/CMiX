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
                return new HSCB(new HSCBModel());

            if (modifierType == typeof(Invert))
                return new Invert(new InvertModel());

            if (modifierType == typeof(Blur))
                return new Blur(new BlurModel());

            if (modifierType == typeof(Edge))
                return new Edge(new EdgeModel());

            if (modifierType == typeof(TransformTexture))
                return new TransformTexture(new TransformTextureModel());

            if (modifierType == typeof(Pixelate))
                return new Pixelate(new PixelateModel());

            if (modifierType == typeof(Echo))
                return new Echo(new EchoModel());

            if (modifierType == typeof(Feedback))
                return new Feedback(new FeedbackModel());

            if (modifierType == typeof(TriColor))
                return new TriColor(new TriColorModel());

            if (modifierType == typeof(RandomUV))
                return new RandomUV(new RandomUVModel());

            if (modifierType == typeof(LFOUV))
                return new LFOUV(new LFOUVModel());

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is HSCBModel hSCBModel)
                return new HSCB(hSCBModel);

            if (modifierModel is InvertModel invertModel)
                return new Invert(invertModel);

            if (modifierModel is BlurModel blurModel)
                return new Blur(blurModel);

            if (modifierModel is EdgeModel edgeModel)
                return new Edge(edgeModel);

            if (modifierModel is TransformTextureModel modelTexture)
                return new TransformTexture(modelTexture);

            if (modifierModel is PixelateModel pixelateModel)
                return new Pixelate(pixelateModel);

            if (modifierModel is EchoModel echoModel)
                return new Echo(echoModel);

            if (modifierModel is FeedbackModel feedbackModel)
                return new Feedback(feedbackModel);

            if (modifierModel is TriColorModel triColorModel)
                return new TriColor(triColorModel);

            if (modifierModel is RandomUVModel randomUVModel)
                return new RandomUV(randomUVModel);

            if (modifierModel is LFOUVModel lfoUVModel)
                return new LFOUV(lfoUVModel);

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
