// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Linq;
using System.Windows.Media.Media3D;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public class TranslateModifier : ObservableObject
    {
        public TranslateModifier(string name, IControl parentSender, MasterBeat beat)
        {
            Count = 1;

            ModifierType = ModifierMode.ToSpread;

            Location = new Vector3D[1] { new Vector3D(0.0, 0.0, 0.0) };
            Scale = new Vector3D[1] { new Vector3D(1.0, 1.0, 1.0) };
            Rotation = new Vector3D[1] { new Vector3D(0.0, 0.0, 0.0) };
        }

        private ModifierMode _modifierType;
        public ModifierMode ModifierType
        {
            get => _modifierType;
            set => SetProperty(ref _modifierType, value);
        }


        private int _count;
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public void AnimateOnBeatTick()
        {

        }

        public void AnimateOnGameLoop(int objectCount)
        {
            var modifierCount = 0;
            if (ModifierType == ModifierMode.ToSpread)
            {
                modifierCount = objectCount;
                if (objectCount != Location.Length)
                {
                    Location = new Vector3D[objectCount];
                    Scale = new Vector3D[objectCount];
                    Rotation = new Vector3D[objectCount];
                }
            }
            else if (ModifierType == ModifierMode.ToSpread)
            {
                modifierCount = Count;
            }

            var XToAnimate = Location.Select(x => x.X).ToArray();
            var YToAnimate = Location.Select(x => x.Y).ToArray();
            var ZToAnimate = Location.Select(x => x.Z).ToArray();


            for (var i = 0; i < modifierCount; i++)
            {
                Location[i].X = XToAnimate[i];
                Location[i].Y = YToAnimate[i];
                Location[i].Z = ZToAnimate[i];
            }
        }


        //public AnimParameter X { get; set; }
        //public AnimParameter Y { get; set; }
        //public AnimParameter Z { get; set; }

        double[] TranslateX { get; set; }
        double[] TranslateY { get; set; }
        double[] TranslateZ { get; set; }

        public Vector3D[] Location { get; set; }
        public Vector3D[] Scale { get; set; }
        public Vector3D[] Rotation { get; set; }
    }
}
