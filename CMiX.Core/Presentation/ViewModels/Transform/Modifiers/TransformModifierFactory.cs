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

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if(modifierModel is RandomXYZModel randomXYZModel)
                return CreateRandomXYZ(randomXYZModel);

            if (modifierModel is LinearXYZModel linearXYZModel)
                return CreateLinearXYZ(linearXYZModel);

            return null;
        }



        public void SetMasterBeat(MasterBeat masterBeat)
        {
            MasterBeat = masterBeat;
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
