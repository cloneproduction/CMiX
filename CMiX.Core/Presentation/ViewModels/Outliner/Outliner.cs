// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentation.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Outliner : ObservableObject
    {
        public Outliner(IComponent component)
        {
            Component = component;
            OutlinerDragDropManager = new OutlinerDragDropManager();
        }

        public ObservableCollection<Component> Components
        {
            get => this.Component.Components;
        }

        public IComponent Component { get; set; }

        private OutlinerDragDropManager _outlinerDragDropManager;
        public OutlinerDragDropManager OutlinerDragDropManager
        {
            get => _outlinerDragDropManager;
            set => SetProperty(ref _outlinerDragDropManager, value);
        }
    }
}
