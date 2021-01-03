﻿using CMiX.MVVM.Models;
using CMiX.MVVM.ViewModels.MessageService;

namespace CMiX.MVVM.ViewModels
{
    public class TransformModifierFactory
    {
        public TransformModifierFactory(MasterBeat masterBeat)
        {
            //this.MessengerTerminal = messengerTerminal;
            this.MasterBeat = masterBeat;
        }


        private static int ID = 0;
        private MessengerTerminal MessengerTerminal { get; set; }

        public MasterBeat MasterBeat { get; set; }
        public TransformModifier CreateTransformModifier(TransformModifierNames transformModifierNames, Sender parentSender)
        {
            TransformModifier transformModifier = null;

            switch (transformModifierNames)
            {
                case TransformModifierNames.Randomized:
                    transformModifier = CreateRandomized(parentSender);
                    break;
            }

            return transformModifier;
        }

        public TransformModifier CreateTransformModifier(TransformModifierNames transformModifierNames, ITransformModifierModel transformModifierModel, Sender parentSender)
        {
            TransformModifier transformModifier = null;

            switch (transformModifierNames)
            {
                case TransformModifierNames.Randomized:
                    transformModifier = CreateRandomized(parentSender);
                    break;
            }

            return transformModifier;
        }

        private RandomXYZ CreateRandomized(Sender parentSender)
        {
            ID++;
            return new RandomXYZ(nameof(TranslateModifier), parentSender, ID, this.MasterBeat);
        }
    }
}