// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Xml.Linq;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace CMiX.Core.Rendering
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings(Integer2 resolution, 
                              GenericValue<string> backgroundColor)
        {
            Resolution = resolution;
            BackgroundColor = backgroundColor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
    }
}
