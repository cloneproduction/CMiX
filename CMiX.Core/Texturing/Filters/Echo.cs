// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Echo : ObservableObject, IModifier
    {
        public Echo(EchoModel echoModel)
        {
            ID = echoModel.ID;
            Visible = new BooleanValue(echoModel.Visible);
            Factor = new FloatValue(echoModel.Factor);
            isExpanded = true;
        }

        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }
        public FloatValue Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
