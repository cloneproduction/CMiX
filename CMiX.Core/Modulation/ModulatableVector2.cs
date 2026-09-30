// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    public class ModulatableVector2 : IControl
    {
        public ModulatableVector2(ModulatableValue<float> x, ModulatableValue<float> y)
        {
            x.Label = "X";
            y.Label = "Y";
            X = x;
            Y = y;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ModulatableValue<float> X { get; }
        public ModulatableValue<float> Y { get; }

        public IControlModel ToModel() => new ModulatableVector2Model
        {
            ID = ID,
            X = (ModulatableValueModel<float>)X.ToModel(),
            Y = (ModulatableValueModel<float>)Y.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableVector2Model)model;
            ID = m.ID;
            X.FromModel(m.X);
            Y.FromModel(m.Y);
        }
    }
}
