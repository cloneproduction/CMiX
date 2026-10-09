// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Modulation
{
    // The Location, Scale, Rotation and Uniform values that TexCoordModifier uses to transform
    // texture coordinates. Pure data: whoever owns an instance also owns the ModulatorManager
    // and decides how Bindables gets saved and loaded.
    public class TexCoordTransform
    {
        public TexCoordTransform(ModulatableValue<float> locationX, ModulatableValue<float> locationY,
                                 ModulatableValue<float> scaleX, ModulatableValue<float> scaleY,
                                 ModulatableValue<float> rotation, ModulatableValue<float> uniform)
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
        public ModulatableValue<float> Rotation { get; }
        public ModulatableValue<float> Uniform { get; }

        public List<ModulatableValue<float>> Bindables =>
            new() { Location.X, Location.Y, Scale.X, Scale.Y, Rotation, Uniform };
    }
}
