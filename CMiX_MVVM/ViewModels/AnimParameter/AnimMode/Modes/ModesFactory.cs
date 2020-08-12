﻿namespace CMiX.MVVM.ViewModels
{
    public static class ModesFactory
    {
        public static IAnimMode CreateMode(ModeType modeType)
        {
            IAnimMode animMode = null;

            if (modeType == ModeType.Steady)
                animMode = CreateSteady();
            else if (modeType == ModeType.LFO)
                animMode = CreateLFO();
            else if (modeType == ModeType.Random)
                animMode = CreateRandomized();
            else if (modeType == ModeType.Stepper)
                animMode = CreateStepper();
            else if (modeType == ModeType.None)
                animMode = CreateNone();

            return animMode;
        }

        private static None CreateNone()
        {
            return new None();
        }

        private static LFO CreateLFO()
        {
            return new LFO();
        }

        private static Steady CreateSteady()
        {
            return new Steady();
        }

        private static Stepper CreateStepper()
        {
            return new Stepper();
        }

        private static Randomized CreateRandomized()
        {
            return new Randomized();
        }
    }
}