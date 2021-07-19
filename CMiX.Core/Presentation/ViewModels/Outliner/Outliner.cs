// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Outliner : ViewModel
    {
        public Outliner(IProject project)
        {
            Project = project;
            OutlinerDragDropManager = new OutlinerDragDropManager();
        }

        public ObservableCollection<Component> Components
        {
            get => this.Project.Components;
        }

        public IProject Project { get; set; }

        private OutlinerDragDropManager _outlinerDragDropManager;
        public OutlinerDragDropManager OutlinerDragDropManager
        {
            get => _outlinerDragDropManager;
            set => SetAndNotify(ref _outlinerDragDropManager, value);
        }
    }
}
