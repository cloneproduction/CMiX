// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public class ModulatableVector3 : IControl
    {
        public ModulatableVector3(ModulatableValue<float> x, ModulatableValue<float> y, ModulatableValue<float> z)
        {
            x.Label = "X";
            y.Label = "Y";
            z.Label = "Z";
            X = x;
            Y = y;
            Z = z;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ModulatableValue<float> X { get; }
        public ModulatableValue<float> Y { get; }
        public ModulatableValue<float> Z { get; }

        public IControlModel ToModel() => new ModulatableVector3Model
        {
            ID = ID,
            X = (ModulatableValueModel<float>)X.ToModel(),
            Y = (ModulatableValueModel<float>)Y.ToModel(),
            Z = (ModulatableValueModel<float>)Z.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableVector3Model)model;
            ID = m.ID;
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Z.FromModel(m.Z);
        }
    }
}
