// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class Integer2 : ObservableRecipient, IControl
    {
        public Integer2()
        {
            X = new IntegerValue();
            Y = new IntegerValue();
            IsActive = true;
        }

        public Integer2(int x, int y) : this()
        {
            X.Value = x;
            Y.Value = y;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public IntegerValue X { get; set; }
        public IntegerValue Y { get; set; }
    }
}
