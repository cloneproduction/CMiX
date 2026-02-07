// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
    }
}
