// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ColorModifierFactory : IModifierFactory
    {
        public ColorModifierFactory()
        {

        }

        private MasterBeat MasterBeat { get; set; }

        public IModifier Create(Type modifierType)
        {
            if (modifierType == typeof(RandomHSV))
                return CreateRandomHSV();

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is RandomHSVModel randomHSVModel)
                return CreateRandomHSV(randomHSVModel);

            return null;
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            MasterBeat = masterBeat;
        }


        private RandomHSV CreateRandomHSV()
        {
            var randomized = new RandomHSV(new RandomHSVModel());
            randomized.SetMasterBeat(MasterBeat);
            return randomized;
        }

        private RandomHSV CreateRandomHSV(RandomHSVModel randomHSVModel)
        {
            var randomized = new RandomHSV(randomHSVModel);
            randomized.SetMasterBeat(MasterBeat);
            return randomized;
        }
    }
}
