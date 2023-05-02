// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public class Integer2 : ObservableRecipient, IControl
    {
        public Integer2(Integer2Model integer2Model)
        {
            ID = integer2Model.ID;
            X = new IntegerValue(integer2Model.X);
            Y = new IntegerValue(integer2Model.Y);
            IsActive = true;
        }

        public Guid ID { get; set; }
        public IntegerValue X { get; set; }
        public IntegerValue Y { get; set; }
    }
}
