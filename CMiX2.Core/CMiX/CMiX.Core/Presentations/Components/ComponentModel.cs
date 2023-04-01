// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.BaseControl;

namespace CMiX.Core.Presentations.Components
{
    public class ComponentModel : IComponentModel
    {
        public ComponentModel()
        {
            ID = Guid.NewGuid();
            ComponentModels = new ObservableCollection<IComponentModel>();
        }

        public Guid ID { get; set; }
        public StringValueModel Name { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsVisible { get; set; }
        public bool ParentIsVisible { get; set; }
        public bool IsExpanded { get; set; }
        public ComponentModel SelectedComponent { get; set; }
        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
    }
}
