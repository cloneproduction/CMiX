// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class Integer3 : ObservableRecipient, IControl
    {
        public Integer3(GenericValue<int> x, 
                        GenericValue<int> y,
                        GenericValue<int> z)
        {
            ID = Guid.NewGuid();
            X = x;
            Y = y;
            Z = z;
            IsActive = true;
        }

        public Guid ID { get; set; }
        public GenericValue<int> X { get; set; }
        public GenericValue<int> Y { get; set; }
        public GenericValue<int> Z { get; set; }
    }
}
