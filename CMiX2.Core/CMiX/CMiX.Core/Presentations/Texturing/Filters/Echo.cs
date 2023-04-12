// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class Echo : ObservableObject, ITextureFilter
    {
        public Echo(EchoModel echoModel, CompositionService compositionService)
        {
            ID = echoModel.ID;
            Name = echoModel.Name;
            Visible = new BooleanValue(echoModel.Visible, compositionService);
            Factor = new FloatValue(echoModel.Factor, compositionService);
            isExpanded = true;
        }

        public TextureFilterName Name { get; set; }
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
