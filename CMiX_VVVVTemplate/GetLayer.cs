using CMiX.MVVM.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace CMiX.Nodes
{
    [PluginInfo(Name = "GetLayer", Category = "CMiX_VVVV")]
    public class GetLayer : IPluginEvaluate
    {
        [Import()]
        public ILogger FLogger;

        [Input("Composition")]
        public IDiffSpread<Component> FComponentIn;

        [Output("Layer")]
        public ISpread<ISpread<Layer>> FLayerOut;

        [Output("Fade")]
        public ISpread<ISpread<double>> FFade;

        [Output("BlendMode")]
        public ISpread<ISpread<string>> FBlendMode;

        public void Evaluate(int SpreadMax)
        {
            FLayerOut.SliceCount = FComponentIn.SliceCount;
            FFade.SliceCount = FComponentIn.SliceCount;
            FBlendMode.SliceCount = FComponentIn.SliceCount;

            if (FComponentIn.SliceCount > 0)
            {
                for (int i = 0; i < FComponentIn.SliceCount; i++)
                {
                    if(FComponentIn[i] != null)
                    {
                        FLayerOut[i].AssignFrom(FComponentIn[i].Components.Cast<Layer>());
                        FFade[i].SliceCount = FLayerOut[i].SliceCount;
                        FBlendMode[i].SliceCount = FLayerOut[i].SliceCount;
                        for (int j = 0; j < FLayerOut[i].SliceCount; j++)
                        {
                            FFade[i][j] = FLayerOut[i][j].Fade.Amount;
                            FBlendMode[i][j] = FLayerOut[i][j].BlendMode.Mode;
                        }
                    }
                    else
                    {
                        FLayerOut[i].SliceCount = 0;
                        FFade[i].SliceCount = 0;
                        FBlendMode[i].SliceCount = 0;
                    }
                }
            }
        }
    }
}