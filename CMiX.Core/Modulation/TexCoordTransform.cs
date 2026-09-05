// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    // The Location/Scale/Rotation/Uniform shape shared by anything that transforms texture
    // coordinates - TexCoordModifier (an Entity) and TextureTexCoord (a Texture's own mandatory
    // transform). Pure data: whoever owns an instance also owns the ModulatorManager and decides
    // how Bindables gets saved and loaded.
    public class TexCoordTransform
    {
        public TexCoordTransform(ModulatableFloat locationX, ModulatableFloat locationY,
                                 ModulatableFloat scaleX, ModulatableFloat scaleY,
                                 ModulatableFloat rotation, ModulatableFloat uniform)
        {
            Location = new ModulatableVector2(locationX, locationY);
            Scale = new ModulatableVector2(scaleX, scaleY);
            rotation.Label = "Rotation";
            uniform.Label = "Uniform";
            Rotation = rotation;
            Uniform = uniform;
        }

        public ModulatableVector2 Location { get; }
        public ModulatableVector2 Scale { get; }
        public ModulatableFloat Rotation { get; }
        public ModulatableFloat Uniform { get; }

        public List<ModulatableFloat> Bindables =>
            new() { Location.X, Location.Y, Scale.X, Scale.Y, Rotation, Uniform };
    }
}
