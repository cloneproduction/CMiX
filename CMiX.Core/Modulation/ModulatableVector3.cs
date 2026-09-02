// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    // A plain grouping of three channels, named to match the ModulatableVectorXYZ view it's built
    // for - same "domain class / view class" split the old system already used (Vector3 the data,
    // VectorXYZ the view). Used when a Modifier owns more than one XYZ group (e.g. GridModifier's separate
    // Width and Phase), where the Modifier itself can only directly expose one X/Y/Z. Built
    // manually from three already-DI-injected ModulatableFloat instances (not itself DI-resolved), the same
    // way RandomPosition already builds its own Vector3 from individually-injected
    // GenericValue<float> instances - so it doesn't need to be IControl, only its three Modulatables
    // do, and they already are.
    public class ModulatableVector3
    {
        public ModulatableVector3(ModulatableFloat x, ModulatableFloat y, ModulatableFloat z)
        {
            x.Label = "X";
            y.Label = "Y";
            z.Label = "Z";
            X = x;
            Y = y;
            Z = z;
        }

        public ModulatableFloat X { get; }
        public ModulatableFloat Y { get; }
        public ModulatableFloat Z { get; }
    }
}
