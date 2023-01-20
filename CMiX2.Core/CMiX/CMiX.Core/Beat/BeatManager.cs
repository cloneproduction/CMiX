// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Presentation.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class BeatManager : ObservableRecipient
    {
        public BeatManager(IProject project)
        {
            Project = project;

            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
            TapCommand = new RelayCommand(Tap);
            ResyncCommand = new RelayCommand(Resync);
        }


        public IProject Project { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public ICommand TapCommand { get; }
        public ICommand ResyncCommand { get; }


        public ObservableCollection<IComponent> Components
        {
            get => Project.Components;
        }

        private Component _selectedComponent;
        public Component SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }

        public void Reset()
        {
            //SelectedComponent?.MasterBeat.Reset();
        }

        public void Multiply()
        {
            //SelectedComponent?.MasterBeat.Multiply();
        }

        public void Divide()
        {
            //SelectedComponent?.MasterBeat.Divide();
        }

        public void Tap()
        {
            //SelectedComponent?.MasterBeat.Tap();
        }

        public void Resync()
        {
            //SelectedComponent?.MasterBeat.Resync.DoResync();
        }
    }
}
