// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public class Echo : ObservableObject, ITextureFilter
    {
        public Echo(EchoModel echoModel)
        {
            ID = echoModel.ID;
            Visible = new BooleanValue(echoModel.Visible);
            Name = echoModel.Name;
            Factor = new FloatValue(echoModel.Factor);

            IsExpanded = true;
        }

        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }
        public FloatValue Factor { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public void Dispose()
        {

        }
    }
}
