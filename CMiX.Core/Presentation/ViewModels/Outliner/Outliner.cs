// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentation.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Outliner : ObservableObject
    {
        public Outliner(IProject project)
        {
            Project = project;
            OutlinerDragDropManager = new OutlinerDragDropManager();
        }

        public ObservableCollection<IComponent> Components
        {
            get => this.Project.Components;
        }

        private IProject _project;
        public IProject Project
        {
            get => _project;
            set => SetProperty(ref _project, value);
        }

        private OutlinerDragDropManager _outlinerDragDropManager;
        public OutlinerDragDropManager OutlinerDragDropManager
        {
            get => _outlinerDragDropManager;
            set => SetProperty(ref _outlinerDragDropManager, value);
        }
    }
}
