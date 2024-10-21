// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Compositing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class JobEditor : ObservableObject//, IModalDialogViewModel
    {
        public JobEditor(Project project)
        {
            ToRunType = new ToRunType();
        }

        public ICommand ApplyCommand { get; set; }
        public bool? DialogResult { get; set; }


        private ToRunType _toruntype;
        public ToRunType ToRunType
        {
            get => _toruntype;
            set => SetProperty(ref _toruntype, value);
        }

        private string _jobName;
        public string JobName
        {
            get => _jobName;
            set => SetProperty(ref _jobName, value);
        }
    }
}
