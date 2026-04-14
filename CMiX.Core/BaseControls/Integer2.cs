// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class Integer2 : ObservableRecipient, IControl
    {
        public Integer2(GenericValue<int> x, GenericValue<int> y)
        {
            ID = Guid.NewGuid();
            X = x;
            Y = y;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<int> X { get; set; }
        public GenericValue<int> Y { get; set; }

        public IControlModel ToModel() => new Integer2Model
        {
            ID = ID,
            X = (GenericValueModel<int>)X.ToModel(),
            Y = (GenericValueModel<int>)Y.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (Integer2Model)model;
            Debug.WriteLine($"Integer2.FromModel X={m.X.Value} Y={m.Y.Value}");
            ID = m.ID;
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Debug.WriteLine($"Integer2.FromModel after X={X.Value} Y={Y.Value}");
        }
    }
}
